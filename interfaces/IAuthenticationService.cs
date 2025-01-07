namespace PWManager
{
    public interface IAuthenticationService
    {
        bool Authenticate(string masterPassword);
        bool IsAuthenticated { get; }
    }
}