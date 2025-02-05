using Microsoft.Extensions.DependencyInjection;
using PWManager.Interfaces;
using PWManager.Services;

namespace PWManager;

class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        var path = "data.txt";
        var saltPath = "salt.txt";
        if (args.Length > 0 && Path.Exists(args[0]))
        {
            path = Path.Combine(args[0], path);
            saltPath = Path.Combine(args[0], path);
        }
        
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ILoggingService, LoggingService>()  
            .AddSingleton<ICommunicationService, CommunicationService>()
            .AddSingleton<ICypherService, CypherService>()
            .AddSingleton<IPersistenceService, PersistenceService>(serviceProvider => 
                new PersistenceService(Path.Combine(Environment.CurrentDirectory, path)))
            .AddSingleton(new SaltPersistenceService(Path.Combine(Environment.CurrentDirectory, saltPath)))
            .AddSingleton<IContextService, ContextService>()
            .AddSingleton<IAuthenticationService, AuthenticationService>()
            .AddSingleton<IValidationService, ValidationService>()
            .AddSingleton<AppChainLogic>()
            .BuildServiceProvider();
        
        var app = serviceProvider.GetRequiredService<AppChainLogic>();
        await app.Run();
    }
}

