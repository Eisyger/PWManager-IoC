using PWManager.interfaces;

namespace PWManager.Services;
    public class LoggingService : ILoggingService
    {
    public LoggingService()
    {
    }

    public virtual void Error(string message)
        {
            Console.WriteLine("Error>>: " + message);
        }

    public virtual void Log(string message)
        {
            Console.WriteLine("Log>>: " + message);
        }

    public virtual void Warning(string message)
        {
            Console.WriteLine("Warning>>: " + message);
        }
    }
