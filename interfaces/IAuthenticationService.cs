namespace PWManager.interfaces
{
    public interface IAuthenticationService
    {
        string Salt {get;}
        string Token {get;}        
        string CreateSaveToken();
        string CreateRandomToken(string user, string pwd);
        string RecreateToken(string user, string pwd, string salt);
    }
}