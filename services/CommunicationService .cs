using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWManager;
public sealed class CommunicationService : ICommunicationService
{
    public string WriteWelcome()
    {
        Console.WriteLine("Willkommen!");
        return "";
    }
    
    public string WriteRegister(Func<string, string, bool> validate, Func<string, string, string> token)
    {
        var invalidInput = false;
        string? username;
        string? password;
        do{
            Console.Clear();
            Console.WriteLine(invalidInput ? "UNGÜLTIGE EINGABE" : "REGISTRIERUNG");
            Console.WriteLine("Es ist noch kein Account Registriert.");
            Console.WriteLine("Starte registrierung...");
            Console.WriteLine("Gib einen Usernamen an:");
            username = Console.ReadLine() ?? "";
            Console.WriteLine("Gib ein Masterpasswort an:");
            password = ValidationHelper.ReadPassword();
            invalidInput = true;
        } while (string.IsNullOrWhiteSpace(username)|| 
                 string.IsNullOrWhiteSpace(password) || 
                 !validate.Invoke(username, password));

        return token.Invoke(username, password);
    }
    public string WriteLogin(Func<string, string, bool> validate, Func<string, string, string> token)
    {
        var invalidInput = false;
        string? username;
        string? password;
        do{
            Console.Clear();
            Console.WriteLine(invalidInput ? "UNGÜLTIGE EINGABE" : "LOGIN");
            Console.WriteLine("Starte Login...");
            Console.WriteLine("Username:");
            username = Console.ReadLine();
            Console.WriteLine("Passwort:");
            password = ValidationHelper.ReadPassword();
            invalidInput = true;
        }while (string.IsNullOrWhiteSpace(username)|| 
                string.IsNullOrWhiteSpace(password) || 
                !validate.Invoke(username, password));

        return token.Invoke(username, password);
    }
    public (bool Success, IDataContext? dataContext) WriteAdd()
    {
        Console.WriteLine("ACCOUNT HINZUFÜGEN");
        Console.WriteLine("Accountname:");
        var name = Console.ReadLine();
        if (string.IsNullOrEmpty(name))
        {   
            Console.WriteLine("FEHLER: Es wurde kein Accountname angegeben.");
            Console.WriteLine("Der Accountname ist ein Pflichtfeld.");
            return (false, null);
        }
        Console.WriteLine("Benutzername:");
        var user = Console.ReadLine();
        Console.WriteLine("Password:");
        var pwd = ValidationHelper.ReadPassword();
        Console.WriteLine("URL:");
        var url = Console.ReadLine();
        Console.WriteLine("Beschreibung:");
        var description = Console.ReadLine();

        return (true, new DataContext()
        {
            Name = name ?? "-",
            User = user ?? "-",
            Password = pwd,
            Website = url ?? "-",
            Description = description ?? "-"
        });
    }
    public string WriteRemove()
    {
        throw new NotImplementedException();
    }
    public void WriteExit()
    {
        Console.WriteLine("Anwendung beendet.");
    }
    public MenuAction WriteMenu()
    {
        var validInput = true;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("""
                              ===========================
                                 PASSWORT MANAGEMENT
                              ===========================
                              [s] Accounts ansehen
                              [a] Account hinzufügen
                              [r] Account löschen
                              [e] Beenden
                              ===========================
                              """);
Console.WriteLine(validInput ? "Bitte eine Option wählen:" : "Ungültige Eingabe. Bitte [s], [a], [r] oder [e] eingeben.");
validInput = true;


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
                    validInput = false;
                    break;
            }
        }
    }
    public void WriteDump(IContextService contextService)
    {
        var contexts = contextService.GetAll();
        if (contexts.Count == 0)
        {
            Console.WriteLine("Keine Accountdaten vorhanden.");
        }
        else
        {
            foreach (var c in contextService.GetAll())
            {
                Console.WriteLine(c.ToString());
            }
        }
        Console.ReadLine();
    }
}