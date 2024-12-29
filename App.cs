using System.Data;
using System.Runtime.Serialization;
using System.Text.Json;

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
            if (saveFileData.Success && startUp.IsLogin)
            {
                loggingService.Log("Daten in SaveFile vorhanden.");

                try
                {
                    var result = cypher.Decrypt(saveFileData.data, token);
                    context.SetAll(JsonSerializer.Deserialize<List<DataContext>>(result.DecryptedText) 
                                    ?? throw new SerializationException("Es konnten aus dem Savefile keine Daten geladen werden."));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else if(!saveFileData.Success && startUp.IsLogin)
            {
                
                loggingService.Error("Keine Daten in SaveFile vorhanden.");
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
                    case MenuAction.Exit:
                        com.WriteExit();
                        return;
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