namespace PWManager;

public static class PasswordReader
{
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
    
}