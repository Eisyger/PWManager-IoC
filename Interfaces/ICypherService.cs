namespace PWManager.Interfaces
{
    public interface ICypherService
    {
        string Encrypt<T>(T data, string token);
        Task<string> EncryptAsync<T>(T data, string token);
        T Decrypt<T>(string plaintext, string token); 
        Task<T> DecryptAsync<T>(string plaintext, string token); 
    } 
}