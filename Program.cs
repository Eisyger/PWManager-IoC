using Microsoft.Extensions.Configuration;
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
        bool isRegister = false;
        if (args.Length > 0)
        {
            if (args[0] == "-register") 
                isRegister = true;
        }
        
        var config = new ConfigurationBuilder().
            SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<IAppKeyService, AppKeyService>()
            .AddDbContext<AccountContext>()
            .AddSingleton<ILoggingService, LoggingService>()  
            .AddSingleton<ICommunicationService, ConsoleCommunicationService>()
            .AddSingleton<ICypherService, CypherService>()
            .AddSingleton<IContextService, AccountService>()
            .AddSingleton<IAuthenticationService, AuthenticationService>()
            .AddSingleton<IValidationService, ValidationService>()
            .AddSingleton<ConsoleApp>()
            .BuildServiceProvider();
        
        var app = serviceProvider.GetRequiredService<ConsoleApp>();
        app.Run(isRegister);
    }
}

