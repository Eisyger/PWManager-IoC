namespace PWManager.interfaces
{
    public interface IAuthenticationService
    {
        string Token {get;}
        string Salt {get;}
        string GenerateToken(string user, string pwd);
        bool Authenticate(string user, string pwd, string salt);
    }
}