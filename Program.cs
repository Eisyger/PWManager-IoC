using Microsoft.Extensions.DependencyInjection;
using PWManager.interfaces;
using PWManager.services;
using PWManager.Services;

namespace PWManager;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        
        var path = "data.txt";
        var saltPath = "salt.txt";
        if (args.Length > 0 && Path.Exists(args[0]))
        {
            path = Path.Combine(args[0], path);
            saltPath = Path.Combine(args[0], path);
        }
        
        // Erstelle IoC-Container
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ILoggingService, LoggingService>()  
            .AddSingleton<ICommunicationService, CommunicationService>()
            .AddSingleton<ICypherService, CypherService>()
            .AddSingleton<IPersistenceService, PersistenceService>(serviceProvider => 
                new PersistenceService(Path.Combine(Environment.CurrentDirectory, path)))
            .AddSingleton<IPersistenceService, SaltPersistenceService>(serviceProvider => 
                new SaltPersistenceService(Path.Combine(Environment.CurrentDirectory, saltPath)))
            .AddSingleton<IContextService, ContextService>()
            .AddSingleton<IAuthenticationService, AuthenticationService>()
            .AddSingleton<App>()  
            .BuildServiceProvider();
        
        var app = serviceProvider.GetRequiredService<App>();
        app.Run();
    }
}

