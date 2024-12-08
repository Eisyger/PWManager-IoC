using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWManager;
public class CommunicationService : ICommunicationService
{
    public string WriteWelcome()
    {
        Console.WriteLine("Willkommen!");
        return "";
    }
    
    public string WriteLogin()
    {
        throw new NotImplementedException();
    }

    public virtual string WriteRegister(Func<string, string, bool> validate, Func<string, string, string> token)
    {
        string username;
        string password;
        do{
            Console.WriteLine("Es ist noch kein Account Registriert.");
            Console.WriteLine("Starte registrierung...");
            Console.WriteLine("Gib einen Usernamen an:");
            username = Console.ReadLine() ?? "";
            Console.WriteLine("Gib ein Masterpasswort an:");
            password = ReadPassword();
        } while (username == "" || password == "" || !validate.Invoke(username, password));

        return token.Invoke(username, password);
    }
    private string ReadPassword()
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

    public virtual string WriteLogin(Func<string, string, bool> validate, Func<string, string, string> token)
    {
        string? username;
        string? password;
        var isValidInput = true;
        do{
            if (isValidInput == false)
            {
                Console.WriteLine("Falscher Username oder Passwort.");
            }
            Console.WriteLine("Login");
            Console.WriteLine("Username:");
            username = Console.ReadLine();
            Console.WriteLine("Passwort:");
            password = ReadPassword();
            isValidInput = false;
        }while (username is null || password == string.Empty || !validate.Invoke(username, password));

        return token.Invoke(username, password);
    }
    
    public bool IsRegister(bool startSignUp)
    {
        Console.WriteLine(startSignUp ? "Starte Login." : "Starte Registrierung.");
        return startSignUp;
    }
    
    public DataContext WriteAdd()
    {
        Console.WriteLine("ACCOUNT HINZUFÜGEN");
        Console.WriteLine("Name des Accounts:");
        var name = Console.ReadLine();
        Console.WriteLine("Username:");
        var user = Console.ReadLine();
        Console.WriteLine("Password:");
        var pwd = ReadPassword();
        Console.WriteLine("URL:");
        var url = Console.ReadLine();
        Console.WriteLine("Beschreibung:");
        var description = Console.ReadLine();

        return new DataContext()
        {
            Name = name,
            User = user,
            Password = pwd,
            Website = url,
            Description = description
        };
    }

    public string WriteRemove()
    {
        throw new NotImplementedException();
    }

    public string WriteExit()
    {
        throw new NotImplementedException();
    }
    public MenuAction WriteShow()
    {
        while (true)
        {
            Console.WriteLine(@"
===========================
   PASSWORT MANAGEMENT
===========================
[s] Accounts ansehen
[a] Account hinzufügen
[r] Account löschen
[e] Beenden
===========================
Bitte eine Option wählen:");

            var input = Console.ReadLine()?.Trim().ToLower();

            switch (input)
            {
                case "s":
                    return MenuAction.ViewAccounts;
                case "a":
                    return MenuAction.AddAccount;
                case "r":
                    return MenuAction.RemoveAccount;
                case "e":
                    return MenuAction.Exit;
                default:
                    Console.WriteLine("Ungültige Eingabe. Bitte [s], [a], [r] oder [e] eingeben.");
                    break;
            }
        }
    }

}