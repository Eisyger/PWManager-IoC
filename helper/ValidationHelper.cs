namespace PWManager;

public static class ValidationHelper
{
    /// <summary>
    /// Gibt die, nach dem letzte Aufruf von ValidateUserAndPasswort entstandene Fehlernachricht zurück.
    /// Ist Message null, war kein Fehler im letzten Aufruf.
    /// </summary>
    public static string ValidationMessage {get;private set;} = string.Empty;
    
    /// <summary>
    /// Startet eine Konsolen ReadLine Abfrage, wo die Eingabe verschlüsselt mit '*' dargestellt wird.
    /// ESC zum Abbrechen der Eingabe.
    /// </summary>
    /// <returns>Die manuelle Eingabe.</returns>
    public static string ReadMaskedPassword()
    {
        var password = string.Empty;
        ConsoleKey key;

        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
            else if (key == ConsoleKey.Escape)
            {
                Console.WriteLine("\nPassword input cancelled.");
                return string.Empty;
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                password += keyInfo.KeyChar;
                Console.Write("*");
            }
        } while (key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }

    
    /// <summary>
    /// Prüft, ob ein Username und ein Passwort den entsprechenden Kriterien entspricht.
    /// Gibt die Fehlernachricht in der Console aus.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool ValidateUserAndPassword(string username, string password)
    {
        ValidationMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(username))
            ValidationMessage += "Ein Benutzername ist erforderlich.\n";

        if (username.Length < 3)
            ValidationMessage += "Der Benutzername muss mindestens 3 Zeichen lang sein.\n";

        if (username.Length > 20)
            ValidationMessage += "Der Benutzername muss zwischen 3 und 20 Zeichen lang sein.\n";

        if (password.Length < 8)
            ValidationMessage += "Das Passwort muss mindestens 8 Zeichen lang sein.\n";

        if (password.Length > 64)
            ValidationMessage += "Das Passwort muss zwischen 8 und 64 Zeichen lang sein.\n";

        if (!password.Any(char.IsUpper))
            ValidationMessage += "Das Passwort muss mindestens einen Großbuchstaben enthalten.\n";

        if (!password.Any(char.IsLower))
            ValidationMessage += "Das Passwort muss mindestens einen Kleinbuchstaben enthalten.\n";

        if (!password.Any(char.IsDigit))
            ValidationMessage += "Das Passwort muss mindestens eine Zahl enthalten.\n";

        if (!password.Any(ch => "!|@#$%^&*()".Contains(ch)))
            ValidationMessage += "Das Passwort muss mindestens ein Sonderzeichen enthalten: '!|@#$%^&*()'\n";

        if (ValidationMessage.Length == 0)
        {
            return true;
        }
        return false;
    }
}