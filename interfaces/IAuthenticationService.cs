namespace PWManager.interfaces
{
    public interface IAuthenticationService
    {
        string GenerateSalt();
        string GenerateToken(string user, string pwd);
        string GetToken(string user, string pwd, string salt);
    }
}