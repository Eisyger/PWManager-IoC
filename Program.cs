using PWManager.Services;

namespace PWManager;
class Programm
{
    public static void Main(string[] args)
    { 
       var app = new App(
        new LoggingService(), 
        new CommunicationService(), 
        new CypherService(), 
        new PersistenceService(Path.Combine(Environment.CurrentDirectory, "data.txt")),
        new ContextService());
        
       app.Run();
    }
}

