using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using PWManager.Context;
using PWManager.Interfaces;
using PWManager.Services;

namespace PWManager;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        builder.Services.AddDbContext<IAccountContext, AccountContext>();
        builder.Services.AddSingleton<IAppKeyService, AppKeyService>();
        builder.Services.AddSingleton<ILoggingService, LoggingService>();
        builder.Services.AddSingleton<ICommunicationService, ConsoleCommunicationService>();
        builder.Services.AddSingleton<ICypherService, CypherService>();
        builder.Services.AddSingleton<IContextService, AccountService>();
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<IValidationService, ValidationService>();
        builder.Services.AddSingleton<ConsoleApp>();
        
        var app = builder.Build();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        Task.Run(() => app.Run());
        
        app.Services.GetRequiredService<ConsoleApp>().Run(ArgsParser.Register(args));
    }
}


