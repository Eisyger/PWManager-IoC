using System.Data;
using System.Runtime.Serialization;
using System.Text.Json;
using PWManager.interfaces;
using PWManager.model;
using TextCopy;

namespace PWManager;

internal class App(
    ILoggingService loggingService, 
    ICommunicationService com, 
    ICypherService cypher,
    IPersistenceService persistenceService,
    IContextService ctxService)
{
    public void Run()
    {
            // Das Token steht über die Laufzeit zur Verfügung, entweder durch das Registrieren,
            // oder durch den Login. Der Context wird mit dem Token ver- oder entschlüsselt.
            var startUp = StartUp();

            // Lade Daten aus Savefile in _context
            LoadData(startUp);

            // Zeige Menu
            ShowMenu(startUp);
            
    }

    private void ShowMenu((bool IsLogin, string Token) startUp)
    {
        while (true)
        {
                var state = com.WriteMenu();

                switch (state.Action)
                {
                    case MenuAction.ViewAccounts:
                        com.WriteDump(ctxService);
                        break;
                    case MenuAction.AddAccount:
                        var result = com.WriteAdd();
                        if (result is { Success: true, dataContext: not null })
                        {
                            ctxService.Add(result.dataContext);
                            Save(startUp.Token);
                        }
                        break;
                    case MenuAction.RemoveAccount:
                        var name = com.WriteRemove();
                        if (ctxService.GetAll().Any(x => x.Name == name))
                        {
                            ctxService.Remove(name);
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
                            com.WriteDump(ctxService.GetContext(state.Args));
                        }
                        catch (ArgumentNullException e)
                        {
                            loggingService.Log(e.Message);
                        }
                        break;
                    case MenuAction.CopyAccount:
                        try
                        {
                            ClipboardService.SetText(ctxService.GetContext(state.Args).Password);
                        }
                        catch (ArgumentNullException e)
                        {
                            ClipboardService.SetText("");
                            loggingService.Log(e.Message);
                        }
                        break;
                    default:
                        var exception = new ArgumentOutOfRangeException
                        {
                            Source = "Für eine MenuAction wurde keine case Definiert."
                        };
                        exception.Data.Add("Action", state.Action);
                        throw exception;
                }
        }
    }

    private void LoadData((bool IsLogin, string Token) startUp)
    {
        var saveFileData = persistenceService.LoadData();
        switch (saveFileData.Success)
        {
            case true when startUp.IsLogin:
                try
                {
                    var result = cypher.Decrypt(saveFileData.data, startUp.Token);
                    ctxService.SetAll(JsonSerializer.Deserialize<List<DataContext>>(result.DecryptedText) 
                                      ?? throw new SerializationException("Es konnten keine Daten aus dem SaveFile geladen werden."));
                    
                    loggingService.Log("Daten aus SaveFile geladen.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                break;
            case false when startUp.IsLogin:
                loggingService.Error("Keine Daten in SaveFile vorhanden.");
                return;
        }
    }

    private (bool IsLogin, string Token) StartUp()
    {
        string? token;
        bool isLogin;
        loggingService.Log("App gestartet");

        com.WriteWelcome();
        
        if (File.Exists(persistenceService.GetPath()))
        {
            loggingService.Log("Starte Login.");  
            
            isLogin = true;
            token = com.WriteLogin(
                (u, p) => u==p, // Validate - impl fehlt noch
                cypher.CreateToken); // Create Token
           
            loggingService.Warning("Username und Passwort sind gleich! " +
                            "Es ist noch keine Implementierung zur validierung der Eingaben vorhanden.");   
        }
        else 
        {
            isLogin = false;
            token = com.WriteRegister(
                (u, p) => u==p, // Validate - impl fehlt noch
                cypher.CreateToken); // Create Token
            loggingService.Warning("Username und Passwort sind gleich! " +
                            "Es ist noch keine Implementierung zur validierung der Eingaben vorhanden.");  
        }
        loggingService.Log($"Token wurde erstellt, mit einer Länge von {token.Length}");
        
        return (isLogin, token ?? throw new NoNullAllowedException("Token ist null."));   
    }

    private void Save(string token)
    {
        var encryptedContext = cypher.Encrypt(JsonSerializer.Serialize(ctxService.GetAll()), token);
        persistenceService.SaveData(encryptedContext);
    }
}