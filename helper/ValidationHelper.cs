namespace PWManager;

public static class ValidationHelper
{
    public static string ReadPassword()
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
                Console.Write("\b \b"); // Entferne letzten Charakter
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                password += keyInfo.KeyChar;
                Console.Write("*");
            }
        } while (key != ConsoleKey.Enter);

        Console.WriteLine(); // Neue Zeile nach Eingabe
        return password;
    }
    
}