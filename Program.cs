using Microsoft.Extensions.DependencyInjection;
using PWManager.Entity;
using PWManager.Interfaces;
using PWManager.Services;

namespace PWManager;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0 && Path.Exists(args[0]))
        {
            if (args[0] == "-register") ;
            //TODO Implementiere Start mit Registrierung
        }
        
        var serviceProvider = new ServiceCollection()
            .AddDbContext<AccountContext>()
            .AddSingleton<ILoggingService, LoggingService>()  
            .AddSingleton<ICommunicationService, ConsoleCommunicationService>()
            .AddSingleton<ICypherService, CypherService>()
            .AddSingleton<IContextService, AccountService>()
            .AddSingleton<IAuthenticationService, AuthenticationService>()
            .AddSingleton<IValidationService, ValidationService>()
            .AddSingleton<App>()
            .BuildServiceProvider();
        
        var app = serviceProvider.GetRequiredService<App>();
        app.Run();
    }
}

