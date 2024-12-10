using System.Data;
using System.Runtime.Serialization;
using System.Text.Json;

namespace PWManager;

class App(
    ILoggingService loggingService, 
    ICommunicationService com, 
    ICypherService cypher,
    IPersistenceService persistenceService,
    IContextService context)
{
    private readonly ILoggingService _logger = loggingService;
    private readonly ICommunicationService _com = com;
    private readonly ICypherService _cypher = cypher;
    private readonly IPersistenceService _persistenceService = persistenceService;
    private readonly IContextService _context = context;
    public void Run()
    {
            // Das Token steht über die Laufzeit zur Verfügung, entweder durch das Registrieren,
            // oder durch den Login. Der Context wird mit dem Token ver- oder entschlüsselt.
            string? token = null;

            var startUp = StartUp();
            token = startUp.Token;
            
            // Lade Daten aus Savefile in _context
            var saveFileData = _persistenceService.LoadData();
            if (saveFileData.Success && startUp.IsLogin)
            {
                _logger.Log("Daten in SaveFile vorhanden.");

                try
                {
                    var result = _cypher.Decrypt(saveFileData.data, token);
                    _context.SetAll(JsonSerializer.Deserialize<List<DataContext>>(result.DecryptedText) 
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
                
                _logger.Error("Keine Daten in SaveFile vorhanden.");
            }

            // Zeige Menu
            while (true)
            {
                var state = _com.WriteMenu();

                switch (state)
                {
                    case MenuAction.ViewAccounts:
                        _com.WriteDump(_context);
                        break;
                    case MenuAction.AddAccount:
                    case MenuAction.RemoveAccount:
                    case MenuAction.Exit:
                        _com.WriteExit();
                        return;
                }
            }
            





            /*var newAccount = com.WriteAdd();
              _context.Add(newAccount);
              var encryptedContext = _cypher.Encrypt(JsonSerializer.Serialize(_context.GetAll()), token);
              _persistenceService.SaveData(encryptedContext);*/

            /*else
            {
                var result = _cypher.Decrypt(encryptedData, token);
                if (result.Success && result.DecryptedText != null)
                {
                    Console.WriteLine("Success");
                    _context.SetAll(JsonSerializer.Deserialize<List<DataContext>>(result.DecryptedText) ?? throw new InvalidOperationException());
                }
                else
                {
                    Console.WriteLine("Login Fehlgeschlagen");
                    Thread.Sleep(5000);
                    return;
                }

                foreach (var c in _context.GetAll())
                {
                    Console.WriteLine(c);
                }

                // Register kann man nur neu starten, wenn mann der App einen Startparameter gibt, oder
                // es noch kein SaveFile gibt.
            }*/
    }
    private (bool IsLogin, string Token) StartUp()
    {
        string? token = null;
        var isLogin = false;
        _logger.Log("App gestartet");

        _com.WriteWelcome();
        
        if (File.Exists(_persistenceService.GetPath()))
        {
            _logger.Log("Starte Login.");  
            
            isLogin = true;
            token = _com.WriteLogin(
                (u, p) => u==p, // Validate - impl fehlt noch
                _cypher.CreateToken); // Create Token
           
            _logger.Warning("Username und Passwort sind gleich! " +
                            "Es ist noch keine Implementierung zur validierung der Eingaben vorhanden.");   
        }
        else 
        {
            isLogin = false;
            token = _com.WriteRegister(
                (u, p) => u==p, // Validate - impl fehlt noch
                _cypher.CreateToken); // Create Token
            _logger.Warning("Username und Passwort sind gleich! " +
                            "Es ist noch keine Implementierung zur validierung der Eingaben vorhanden.");  
        }
        _logger.Log($"Token wurde erstellt, mit einer Länge von {token.Length}");
        
        return (isLogin, token ?? throw new NoNullAllowedException("Token ist null."));   
    }
}