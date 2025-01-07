using Microsoft.Extensions.DependencyInjection;
using PWManager.Services;

namespace PWManager;

class Program
{
    public static void Main(string[] args)
    {
        // Erstelle IoC-Container
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ILoggingService, LoggingService>()  
            .AddSingleton<ICommunicationService, CommunicationService>()
            .AddSingleton<ICypherService, CypherService>()
            .AddSingleton<IPersistenceService, PersistenceService>(serviceProvider => 
                new PersistenceService(Path.Combine(Environment.CurrentDirectory, "data.txt")))
            .AddSingleton<IContextService, ContextService>()
            .AddSingleton<App>()  
            .BuildServiceProvider();
        
        var app = serviceProvider.GetRequiredService<App>();
        app.Run();
    }
}

