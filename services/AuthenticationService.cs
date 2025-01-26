using System.Security.Cryptography;
using System.Text;
using PWManager.interfaces;

namespace PWManager.services;

public sealed class AuthenticationService : IAuthenticationService
{
    private string? _username;
    private string? _password;

    // Properties mit Validierung
    private string Username
    {
        get => _username ?? throw new InvalidOperationException("Der Benutzername wurde noch nicht gesetzt.");
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Der Benutzername darf nicht leer sein.");
            _username = value;
        }
    }

    private string Password
    {
        get => _password ?? throw new InvalidOperationException("Das Passwort wurde noch nicht gesetzt.");
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Das Passwort darf nicht leer sein.");
            _password = value;
        }
    }

    public string Token { get; private set; } = string.Empty;

    public string Salt { get; private set; } = string.Empty;

    private string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var byteSalt = new byte[16];
        rng.GetBytes(byteSalt);
        Salt = Convert.ToBase64String(byteSalt);
        return Salt;
    }

    private string GenerateToken(string username, string password, string salt)
    {
        Username = username;
        Password = password;
        Salt = salt;

        var combined = username + salt + password;
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(combined));
        Token = Convert.ToHexString(hashBytes);
        return Token;
    }

    /// <summary>
    /// Erstellt ein Token basierend auf bereits gesetztem Benutzernamen und Passwort und zufälligen Salt.
    /// </summary>
    /// <returns>Das generierte Token als hexadezimaler String.</returns>
    public string CreateSaveToken()
    {        
        return GenerateToken(Username, Password, GenerateSalt());
    }

    /// <summary>
    /// Erstellt ein Token basierend auf Benutzernamen und Passwort.
    /// </summary>
    /// <param name="username">Der Benutzername für die Token-Erstellung.</param>
    /// <param name="password">Das Passwort für die Token-Erstellung.</param>
    /// <returns>Das generierte Token als hexadezimaler String.</returns>
    public string CreateRandomToken(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Der Benutzername darf nicht null oder leer sein.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Das Passwort darf nicht null oder leer sein.");

        return GenerateToken(username, password, GenerateSalt());
    }

    /// <summary>
    /// Erstellt ein Token basierend auf Benutzernamen, Passwort und einem vorgegebenen Salt.
    /// </summary>
    /// <param name="username">Der Benutzername für die Token-Erstellung.</param>
    /// <param name="password">Das Passwort für die Token-Erstellung.</param>
    /// <param name="salt">Das Salt für die Token-Erstellung.</param>
    /// <returns>Das generierte Token als hexadezimaler String.</returns>
    public string RecreateToken(string username, string password, string salt)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Der Benutzername darf nicht null oder leer sein.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Das Passwort darf nicht null oder leer sein.");
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Der Salt darf nicht null oder leer sein.");

        return GenerateToken(username, password, salt);
    }
}
