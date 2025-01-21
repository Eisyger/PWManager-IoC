using System.Security.Cryptography;
using System.Text;
using PWManager.interfaces;

namespace PWManager.services;
public sealed class AuthenticationService : IAuthenticationService
{
    public string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var byteSalt = new byte[16];
        rng.GetBytes(byteSalt);        
        return Convert.ToBase64String(byteSalt);       
    }

    /// <summary>
    /// Erstellt ein eindeutiges Token auf Basis von Benutzername und Passwort.
    /// </summary>
    /// <param name="username">Der Benutzername, der für die Token-Erstellung verwendet wird.</param>
    /// <param name="password">Das Passwort, das für die Token-Erstellung verwendet wird.</param>
    /// <returns>Ein hexadezimaler String, der das generierte Token darstellt.</returns>
    /// <exception cref="ArgumentException">
    /// Wird ausgelöst, wenn <paramref name="username"/> oder <paramref name="password"/> null, leer oder ungültig ist.
    /// </exception>
    /// <remarks>
    /// Die Methode kombiniert den Benutzernamen und das Passwort mit einem statischen Salt, um eine Eingabe 
    /// für die Token-Erstellung zu erzeugen. Anschließend wird ein SHA-512-Hash berechnet und als Hexadezimal-String zurückgegeben.
    /// 
    /// **Hinweis:** Der statische Salt-Wert macht das Verfahren weniger sicher, da er leicht erraten werden kann. 
    /// Für höhere Sicherheit sollte ein dynamischer Salt-Wert verwendet werden.
    /// </remarks>
    public string GenerateToken(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Der Username darf nicht null oder leer sein.");
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Das Passwort darf nicht null oder leer sein.");
              
        var combined = username + GenerateSalt() + password;
        
        // Hash erstellen und in Hex-Format umwandeln
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(combined));
        return Convert.ToHexStringLower(hashBytes);
    }

    public string GetToken(string user, string pwd, string salt)
    {
        return GenerateCompareToken(user, pwd, salt);
    }
    
    private static string GenerateCompareToken(string username, string password, string salt)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Der Username darf nicht null oder leer sein.");
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Das Passwort darf nicht null oder leer sein.");
              
        var combined = username + salt + password;
        
        // Hash erstellen und in Hex-Format umwandeln
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(combined));
        return Convert.ToHexStringLower(hashBytes);
    }
}