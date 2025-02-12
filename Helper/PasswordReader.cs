namespace PWManager;

public static class PasswordReader
{
    /// <summary>
    /// Startet eine Konsolen ReadLine Abfrage, wo die Eingabe verschlüsselt mit '*' dargestellt wird.
    /// ESC zum Abbrechen der Eingabe.
    /// </summary>
    /// <returns>Die manuelle Eingabe.</returns>
    public static char[] ReadMaskedPassword()
    {
        var password = new char[256];
        var cursor = 0;
        ConsoleKey key;

        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            switch (key)
            {
                case ConsoleKey.Backspace when cursor > 0:
                    cursor--;
                    Console.Write("\b \b");
                    break;
                case ConsoleKey.Escape:
                    Console.WriteLine("\nPassword input cancelled.");
                    return [];
                default:
                {
                    if (!char.IsControl(keyInfo.KeyChar))
                    {
                        password[cursor++] = keyInfo.KeyChar;
                        Console.Write("*");
                    }
                    break;
                }
            }
        } while (key != ConsoleKey.Enter);

        Console.WriteLine();
        return password[..cursor];
    }
}