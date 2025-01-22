using System.Data;
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
    IAuthenticationService authService)
{
    private string _token = string.Empty;
    private IContextService _ctxService = ctxService;

    public void Run()
    {
        // Das Token steht über die Laufzeit zur Verfügung, entweder durch das Registrieren,
        // oder durch den Login. Der Context wird mit dem Token ver- oder entschlüsselt.
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
                            Save();
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
                        _token = com.WriteChangeUserData(
                            (u, p) => u==p, // Validate - impl fehlt noch
                            authService.GenerateToken); // Create Token
                        Save();
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
                       
            _ctxService = cypher.Decrypt<ContextService>(saveFileData, _token);                
            loggingService.Log("Daten aus SaveFile geladen.");
        }
        catch (Exception e)
        {
            loggingService.Error(e.Message);
            return false;
        }
        return true;
    }
    private bool StartUp()
    {
        string? token;
        bool isLogin;
        loggingService.Log("App gestartet");

        com.WriteWelcome();
        
        if (File.Exists(persistenceService.GetPath()) && File.Exists(saltPersistenceService.GetPath()))
        {
            loggingService.Log("Starte Login.");  
            
            isLogin = true;
            token = com.WriteLogin((u, p) => u == p, // Validate - impl fehlt noch
                (u, p) => authService.GetToken(u, p, saltPersistenceService.LoadData())); // Create Token
           
            loggingService.Warning("Username und Passwort sind gleich! " +
                            "Es ist noch keine Implementierung zur validierung der Eingaben vorhanden.");   
        }
        else 
        {
            isLogin = false;
            token = com.WriteRegister(
                (u, p) => u == p, // Validate - impl fehlt noch
                authService.GenerateToken);
            loggingService.Warning("Username und Passwort sind gleich! " +
                            "Es ist noch keine Implementierung zur validierung der Eingaben vorhanden.");  
        }
        loggingService.Log($"Token wurde erstellt, mit einer Länge von {token.Length}");
        _token = token ?? throw new NoNullAllowedException("Token ist null.");
        return isLogin;
    }
    private void Save()
    {
        // TODO Das _token wurde erstellt mit dem Salt der aus der Datei gelesen wurde
        // TODO wird nun der Salt neu erstellt, dann muss auch das Token neu erstellt werden, dies geht jedoch nur 
        // TODO durch den Usernamen und das PW
        persistenceService.SaveData(cypher.Encrypt(_ctxService, authService.GenerateToken()));
        saltPersistenceService.SaveData(authService.Salt);
    }
}