namespace PWManager
{
    public interface IConfigurationService
    {
       void SendData(string endpoint, string data);
        string ReceiveData(string endpoint); 
    }
}