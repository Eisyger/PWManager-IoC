namespace PWManager.Interfaces;

public interface IAuthenticationService
{
    public string GenerateSalt();
    public string GenerateKey(string appKey, string salt);
    public string GenerateAppKey(string username, string password);
}