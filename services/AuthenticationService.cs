using System.Security.Cryptography;
using System.Text;
using PWManager.interfaces;

namespace PWManager.services;

public sealed class AuthenticationService : IAuthenticationService
{
    private string? _username;
    private string? _password;
    private string _salt = string.Empty;
    private string _token = string.Empty;

    // Properties mit Validierung
    public string Username
    {
        get => _username ?? throw new InvalidOperationException("Der Benutzername wurde nicht gesetzt.");
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Der Benutzername darf nicht leer sein.");
            _username = value;
        }
    }

    public string Password
    {
        get => _password ?? throw new InvalidOperationException("Das Passwort wurde nicht gesetzt.");
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Das Passwort darf nicht leer sein.");
            _password = value;
        }
    }

    public string Token => _token;
    public string Salt => _salt;

    private string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var byteSalt = new byte[16];
        rng.GetBytes(byteSalt);
        _salt = Convert.ToBase64String(byteSalt);
        return _salt;
    }

    private string GenerateToken(string username, string password, string salt)
    {
        var combined = username + salt password;
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(combined));
        _token = Convert.ToHexString(hashBytes);
        return _token;
    }

    /// <summary>
    /// Erstellt ein Token basierend auf bereits gesetztem Benutzernamen und Passwort.
    /// </summary>
    /// <returns>Das generierte Token als hexadezimaler String.</returns>
    public string CreateToken()
    {        
        return GenerateToken(Username, Password, GenerateSalt());
    }

    /// <summary>
    /// Erstellt ein Token basierend auf Benutzernamen und Passwort.
    /// </summary>
    /// <param name="username">Der Benutzername für die Token-Erstellung.</param>
    /// <param name="password">Das Passwort für die Token-Erstellung.</param>
    /// <returns>Das generierte Token als hexadezimaler String.</returns>
    public string CreateToken(string username, string password)
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
    public string CreateToken(string username, string password, string salt)
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
