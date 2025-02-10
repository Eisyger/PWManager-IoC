using System.Security.Cryptography;
using System.Text;
using PWManager.Interfaces;

namespace PWManager.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    /// <summary>
    /// Erstellt einen Zufälligen Salt.
    /// </summary>
    /// <returns>Salt</returns>
    public string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var byteSalt = new byte[16];
        rng.GetBytes(byteSalt);
        return Convert.ToBase64String(byteSalt);
    }
    
    /// <summary>
    /// Erstellt einen Key zum Entschlüsseln von Daten.
    /// </summary>
    /// <param name="appKey"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    public string GenerateKey(string appKey, string salt)
    {
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(appKey + salt));
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Erstellt einen AppKey, welcher zur Laufzeit Username und Passwort darstellt. Das Passwort
    /// sollte nicht im Program als Klartext gespeichert, sowie einer Variablen zugewiesen werden.
    /// Bis der GarbageCollector die Variable beseitigt hat, kann zur Laufzeit das Passwort ausgelesen werden.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public string GenerateAppKey(string user, string pw)
    {
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(user+pw));
        return Convert.ToHexString(hashBytes);
    }
}

