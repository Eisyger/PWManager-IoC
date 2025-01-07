namespace PWManager

{
    public interface ILoggingService
    {
        void Log(string message);
        void Error(string message);
        void Warning(string message);
    }
}