namespace PWManager.interfaces
{
    public interface ICypherService
    {
        string Encrypt(string plaintext, string token);
        (bool Success, string DecryptedText) Decrypt(string plaintext, string token); 

        string HashPassword(string username, string password);

        string CreateToken(string username, string password);
        
    } 
}