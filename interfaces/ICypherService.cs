namespace PWManager.interfaces
{
    public interface ICypherService
    {
        string Encrypt(IContextService contextService, string token);
        (bool Success, IContextService? contextService) Decrypt(string plaintext, string token); 

        string HashPassword(string username, string password);

        string CreateToken(string username, string password);
    } 
}