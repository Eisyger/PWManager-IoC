using PWManager.Interfaces;

namespace PWManager.Services;

public class ValidationService : IValidationService
{
    
    /// <summary>
    /// Prüft, ob ein Username und ein Passwort den entsprechenden Kriterien entspricht.
    /// Gibt true/false bei richtiger oder falscher Eingabe zurück, sowie eine detaillierte Fehlernachricht.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public (bool Valid, string Message) ValidateUserAndPassword(string username, string password)
    {
        var msg = string.Empty;

        if (string.IsNullOrWhiteSpace(username))
            msg += "Ein Benutzername ist erforderlich.\n";

        if (username.Length < 3)
            msg += "Der Benutzername muss mindestens 3 Zeichen lang sein.\n";

        if (username.Length > 20)
            msg += "Der Benutzername muss zwischen 3 und 20 Zeichen lang sein.\n";

        if (password.Length < 8)
            msg += "Das Passwort muss mindestens 8 Zeichen lang sein.\n";

        if (password.Length > 64)
            msg += "Das Passwort muss zwischen 8 und 64 Zeichen lang sein.\n";

        if (!password.Any(char.IsUpper))
            msg += "Das Passwort muss mindestens einen Großbuchstaben enthalten.\n";

        if (!password.Any(char.IsLower))
            msg += "Das Passwort muss mindestens einen Kleinbuchstaben enthalten.\n";

        if (!password.Any(char.IsDigit))
            msg += "Das Passwort muss mindestens eine Zahl enthalten.\n";

        if (!password.Any(ch => "!|@#$%^&*()".Contains(ch)))
            msg += "Das Passwort muss mindestens ein Sonderzeichen enthalten: '!|@#$%^&*()'\n";

        if (msg.Length == 0)
        {
            return (true, string.Empty);
        }
        return (false, msg);
    }
}