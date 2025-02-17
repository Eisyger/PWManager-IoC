using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using PWManager.Builder;
using PWManager.Entity;
using PWManager.Interfaces;
using PWManager.Services;

namespace PWManager;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<ConsoleAppBuilder>();
        builder.Services.AddControllers();

        var app = builder.Build();
        app.MapControllers();
        
        var consoleApp = app.Services.GetRequiredService<ConsoleAppBuilder>();
        Task.Run(() => consoleApp.Run(args));
        
        app.Run();
    }
}


