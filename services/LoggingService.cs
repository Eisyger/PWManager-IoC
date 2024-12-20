using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWManager.Services;
    public class LoggingService : ILoggingService
    {
    public LoggingService()
    {
    }

    public virtual void Error(string message)
        {
            Console.WriteLine("Error: " + message);
        }

    public virtual void Log(string message)
        {
            Console.WriteLine(">>: " + message);
        }

    public virtual void Warning(string message)
        {
            Console.WriteLine("Warning: " + message);
        }
    }
