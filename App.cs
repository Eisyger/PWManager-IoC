using System.Data;
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
            // Das Token steht über die Laufzeit zur verfügung, entweder durch das Registrieren,
            // oder durch den Login. Der Context wird mit dem Token ver- oder entschlüsselt.
            string? token = null;

            token = StartUp();
            _logger.Log($"Token wurde erstellt, mit Länge {token.Length}.");

            // Lade Daten aus SaveFile. 
            // Prüfe, ob sie entschlüsselt werden können, wenn ja weiter, wenn zurück zum StartUp.
            var encryptedData = _persistenceService.LoadData();
            _logger.Log(encryptedData.Succsess
                ? "Daten in SaveFile vorhanden."
                : "Keine Daten in SaveFile nicht vorhanden.");


            // TO-DO ShowMenu
            
            
            
            
            
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
    private string StartUp()
    {
        string? token = null;
        
        _logger.Log("App gestartet");

        _com.WriteWelcome();
        
        // REGISTER
        // wird nur gestartet beim StartUp wenn kein SaveFile existiert.
        
        if (_com.IsRegister(!File.Exists(_persistenceService.GetPath())))
        {
            _logger.Log("Starte Registrierung");     
            
            
            token = _com.WriteRegister(
                (u, p) => u==p, // Validate - impl fehlt noch
                CypherService.CreateToken); // Create Token
            
            
            _logger.Log("Register: Token wurde erstellt");
            _logger.Warning("Username und Passwort sind gleich! Wird aus Testzwecken trotzdem gespeichert.");             
            
                
        }
        // LOGIN
        else 
        {
            token = _com.WriteLogin(
                (u, p) => u==p, // Validate - impl fehlt noch
                CypherService.CreateToken); // Create Token
            
            
            _logger.Log("Login: Token wurde erstellt.");
        }

        return token ?? throw new NoNullAllowedException("Token ist null.");   
    }
}