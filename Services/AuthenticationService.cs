using System.Security.Cryptography;
using System.Text;
using PWManager.Interfaces;

namespace PWManager.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private string? _username;
    private string? _password;
    
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

    public string Key { get; private set; } = string.Empty;

    public string Salt { get; private set; } = string.Empty;

    private string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var byteSalt = new byte[16];
        rng.GetBytes(byteSalt);
        Salt = Convert.ToBase64String(byteSalt);
        return Salt;
    }

    private string GenerateKey(string username, string password, string salt)
    {
        Username = username;
        Password = password;
        Salt = salt;

        var combined = username + salt + password;
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(combined));
        Key = Convert.ToHexString(hashBytes);
        return Key;
    }

    /// <summary>
    /// Erstellt einen Key basierend auf Benutzername und Passwort mit einem zufälligen Salt.
    /// Kann nur ausgeführt werden, wenn RecreateKey erfolgreich ausgeführt wurde,
    /// da somit erst der Username und das Passwort zur Erstellung des Keys zur Verfügung stehen.
    /// </summary>
    /// <exception cref="NullReferenceException"> Wenn RecreateKey()noch nicht aufgerufen wurde.</exception>
    /// <returns>Key als hexadezimaler String.</returns>
    public string CreateRandomKey()
    {
        if (string.IsNullOrWhiteSpace(Username))
            throw new NullReferenceException("Der Benutzername darf nicht null oder leer sein. " +
                                             "Es wurde die Methode Recreate noch nicht aufgerufen.");
        if (string.IsNullOrWhiteSpace(Password))
            throw new NullReferenceException("Das Passwort darf nicht null oder leer sein. " +
                                             "Es wurde die Methode Recreate noch nicht aufgerufen.");
        
        return GenerateKey(Username, Password, GenerateSalt());
    }

    /// <summary>
    /// Erstellt einen Key basierend auf Benutzername, Passwort und einem vorgegebenen Salt.
    /// </summary>
    /// <param name="username">Der Benutzername für die Erstellung des Keys.</param>
    /// <param name="password">Das Passwort für die Erstellung des Keys.</param>
    /// <param name="salt">Das Salt für die Erstellung des Keys.</param>
    /// <returns>Key als hexadezimaler String.</returns>
    public string RecreateKey(string username, string password, string salt)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Der Benutzername darf nicht null oder leer sein.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Das Passwort darf nicht null oder leer sein.");
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Der Salt darf nicht null oder leer sein.");

        return GenerateKey(username, password, salt);
    }
}
