using PWManager.Data;

namespace PWManager.Interfaces;

public interface IAuthenticationService
{
    public string GenerateSalt();
    public string GenerateKey(AppKey appKey, string salt);
    public AppKey GenerateAppKey(string username, char[] password);
}