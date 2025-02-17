using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PWManager.Entity;
using PWManager.Interfaces;
using PWManager.Services;

namespace PWManager.Builder;

public class ConsoleAppBuilder
{
    public void Run(string[] args)
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
        
        MigrateAndUpdateDatabase(serviceProvider);
        
    
        var app = serviceProvider.GetRequiredService<ConsoleApp>();
        app.Run(isRegister);
    }

    private void MigrateAndUpdateDatabase(ServiceProvider serviceProvider)
    {
        using var db = serviceProvider.GetRequiredService<AccountContext>();
        
        // Falls die DB nicht existiert, erstelle sie und wende Migrationen an
        if (db.Database.CanConnect()) return;
        
        var logger = serviceProvider.GetRequiredService<ILoggingService>();
        logger.Log("Keine Datenbank gefunden. Erstelle eine Migration.");
        db.Database.Migrate();
    }
}