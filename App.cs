using System.Data;
using System.Runtime.Serialization;
using System.Text.Json;
using PWManager.interfaces;
using PWManager.model;

namespace PWManager;

internal class App(
    ILoggingService loggingService, 
    ICommunicationService com, 
    ICypherService cypher,
    IPersistenceService persistenceService,
    IContextService context)
{
    public void Run()
    {
            // Das Token steht über die Laufzeit zur Verfügung, entweder durch das Registrieren,
            // oder durch den Login. Der Context wird mit dem Token ver- oder entschlüsselt.
            string? token = null;

            var startUp = StartUp();
            token = startUp.Token;
            
            // Lade Daten aus Savefile in _context
            var saveFileData = persistenceService.LoadData();
            switch (saveFileData.Success)
            {
                case true when startUp.IsLogin:
                    try
                    {
                        var result = cypher.Decrypt(saveFileData.data, token);
                        context.SetAll(JsonSerializer.Deserialize<List<DataContext>>(result.DecryptedText) 
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

            // Zeige Menu
            while (true)
            {
                var state = com.WriteMenu();

                switch (state)
                {
                    case MenuAction.ViewAccounts:
                        com.WriteDump(context);
                        break;
                    case MenuAction.AddAccount:
                        var result = com.WriteAdd();
                        if (result is { Success: true, dataContext: not null })
                        {
                            context.Add(result.dataContext);
                            Save(token);
                        }
                        break;
                    case MenuAction.RemoveAccount:
                        var name = com.WriteRemove();
                        if (context.GetAll().Any(x => x.Name == name))
                        {
                            context.Remove(name);
                        }
                        else
                        {
                            loggingService.Error($"Kein Account mit dem Namen {name} vorhanden.");
                        }
                        break;
                    case MenuAction.Exit:
                        com.WriteExit();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
    }
    
    private (bool IsLogin, string Token) StartUp()
    {
        string? token = null;
        var isLogin = false;
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
        var encryptedContext = cypher.Encrypt(JsonSerializer.Serialize(context.GetAll()), token);
        persistenceService.SaveData(encryptedContext);
    }
}