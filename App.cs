using PWManager.interfaces;
using PWManager.services;
using TextCopy;

namespace PWManager;

internal class App(
    ILoggingService loggingService, 
    ICommunicationService com, 
    ICypherService cypher,
    IPersistenceService persistenceService,
    SaltPersistenceService saltPersistenceService,
    IContextService ctxService,
    IAuthenticationService authService,
    IValidationService validationService)
{    
    private IContextService _ctxService = ctxService;

    public void Run()
    {
        loggingService.Log("Starte PWManager...");
        var isLogin = StartUp();
        loggingService.Log(isLogin ? "Login ausgeführt." : "Registrierung ausgeführt.");
        
        loggingService.Log("Lade Daten aus der SaveFile...");
        var hasLoaded = LoadData();
        loggingService.Log(hasLoaded ? "Daten aus SaveFile geladen." : "Es wurden keine Daten aus SaveFile geladen.");
        
        if (!hasLoaded && isLogin)
        {
            Console.WriteLine("Die Logindaten sind ungültig!\nBeende die Anwendung...");
            return;
        }
        
        loggingService.Log("State den Menu Loop...");
        ShowMenu();
    }

    private void ShowMenu()
    {
        while (true)
        {
                var state = com.WriteMenu();

                switch (state.Action)
                {
                    case MenuAction.ViewAccounts:
                        com.WriteDump(_ctxService);
                        break;
                    case MenuAction.AddAccount:
                        var result = com.WriteAdd();
                        if (result is { Success: true, dataContext: not null })
                        {
                            _ctxService.Add(result.dataContext);
                            SaveData();
                        }
                        break;
                    case MenuAction.RemoveAccount:
                        var name = com.WriteRemove();
                        if (_ctxService.ContextsList != null && _ctxService.ContextsList.Any(x => x.Name == name))
                        {
                            _ctxService.Remove(name);
                        }
                        else
                        {
                            loggingService.Error($"Kein Account mit dem Namen {name} vorhanden.");
                        }
                        break;
                    case MenuAction.Exit:
                        com.WriteExit();
                        return;
                    case MenuAction.GetAccount:
                        try
                        {
                            com.WriteDump(_ctxService.GetContext(state.Args));
                        }
                        catch (ArgumentNullException e)
                        {
                            loggingService.Log(e.Message);
                        }
                        break;
                    case MenuAction.CopyAccount:
                        try
                        {
                            ClipboardService.SetText(_ctxService.GetContext(state.Args).Password);
                        }
                        catch (ArgumentNullException e)
                        {
                            ClipboardService.SetText("");
                            loggingService.Log(e.Message);
                        }
                        break;
                    case MenuAction.ChangeUserData:
                            com.WriteChangeUserData(
                            validationService.ValidateUserAndPassword, // Validate - impl fehlt noch
                            authService.CreateRandomToken); // Create Token
                        SaveData();
                        break;
                    default:
                        var exception = new ArgumentOutOfRangeException
                        {
                            Source = "Für eine MenuAction wurde kein case definiert."
                        };
                        exception.Data.Add("Action", state.Action);
                        throw exception;
                }
        }
    }
    private bool StartUp()
    {
        bool isLogin;
        loggingService.Log("App gestartet");

        com.WriteWelcome();
        
        if (File.Exists(persistenceService.GetPath()) && File.Exists(saltPersistenceService.GetPath()))
        {
            loggingService.Log("Starte Login.");  
            
            isLogin = true;
            com.WriteLogin(validationService.ValidateUserAndPassword, 
                (u, p) => authService.RecreateToken(u, p, saltPersistenceService.LoadData())); // Create Token
        }
        else 
        {
            isLogin = false;
            com.WriteRegister(
                validationService.ValidateUserAndPassword, 
                authService.CreateRandomToken);
        } 
        return isLogin;
    }
    private void SaveData()
    {
        persistenceService.SaveData(cypher.Encrypt(_ctxService, authService.CreateSaveToken()));
        saltPersistenceService.SaveData(authService.Salt);
    }
    
    /// <summary>
    /// Lädt die Daten aus der SaveFile
    /// </summary>
    /// <returns>Sind Daten vorhanden, lade diese, return true.
    /// Sind keine Daten vorhanden, return true → Erster Login ohne Daten.
    /// Sind Daten vorhanden, aber Fehler beim Laden, return false → ungültige Login Daten.
    /// </returns>
    private bool LoadData()
    {
        try
        {
            var saveFileData = persistenceService.LoadData();
                       
            _ctxService = cypher.Decrypt<ContextService>(saveFileData, authService.Token);                
            loggingService.Log("Daten aus SaveFile geladen.");
        }
        catch (Exception e)
        {
            loggingService.Error(e.Message);
            return false;
        }
        return true;
    }
}