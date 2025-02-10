using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace PWManager.Config;

public static class Configuration
{
    public static IConfigurationRoot GetConfig()
    {
        // Basisverzeichnis automatisch finden
        string basePath = AppContext.BaseDirectory;

        return new ConfigurationBuilder()
            .SetBasePath(basePath) // Automatische Basis
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
    }
}