namespace PWManager.interfaces
{
    public interface IAuthenticationService
    {
        string Salt {get;}
        string Token {get;}        
        string CreateToken();
        string CreateToken(string user, string pwd);
        string CreateToken(string user, string pwd, string salt);
    }
}