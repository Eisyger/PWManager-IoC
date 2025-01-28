namespace PWManager.Interfaces;

public interface IAuthenticationService
{
    string Salt {get;}
    string Key {get;} 
    string CreateRandomKey();
    string RecreateKey(string user, string pwd, string salt);
}