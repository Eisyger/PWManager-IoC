using PWManager.Interfaces;
using PWManager.Model;

namespace PWManager.Services;
public sealed class CommunicationService(IValidationService validationService) : ICommunicationService
{
    public string WriteWelcome()
    {
        Console.WriteLine("Willkommen!");
        return "";
    }

    private string HandleUserInput(
        string title, 
        string successMessage, 
        Func<string, string, bool> validate, 
        Func<string, string, string> token)
    {
        var invalidInput = false;
        string? username;
        string? password;

        do
        {
            Console.Clear();
            Console.WriteLine(invalidInput ? "UNGÜLTIGE EINGABE" : title);
            Console.WriteLine(validationService.ValidationMessage);
            Console.WriteLine(successMessage);
            Console.WriteLine("Gib einen Usernamen an:");
            username = Console.ReadLine() ?? "";
            Console.WriteLine("Gib ein Masterpasswort an:");
            password = PasswordReader.ReadMaskedPassword();
            invalidInput = true;
        } while (string.IsNullOrWhiteSpace(username) || 
                 string.IsNullOrWhiteSpace(password) || 
                 !validate.Invoke(username, password));

        return token.Invoke(username, password);
    }

    public string WriteRegister(Func<string, string, bool> validate, Func<string, string, string> token)
    {
        return HandleUserInput(
            title: "REGISTRIERUNG",
            successMessage: "Es ist noch kein Account registriert.\nStarte Registrierung...",
            validate,
            token
        );
    }

    public string WriteLogin(Func<string, string, bool> validate, Func<string, string, string> token)
    {
        return HandleUserInput(
            title: "LOGIN",
            successMessage: "Starte Login...",
            validate,
            token
        );
    }

    public string WriteChangeUserData(Func<string, string, bool> validate, Func<string, string, string> token)
    {
        return HandleUserInput(
            title: "ÄNDERE LOGIN-DATEN",
            successMessage: "Merke dir die neuen Logindaten gut!",
            validate,
            token
        );
    }


    public (bool Success, DataContext? dataContext) WriteAdd()
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
        var pwd = PasswordReader.ReadMaskedPassword();
        Console.WriteLine("URL:");
        var url = Console.ReadLine();
        Console.WriteLine("Beschreibung:");
        var description = Console.ReadLine();

        var ctx = new DataContext()
        {
            Name = name,
            User = user ?? "-",
            Password = pwd,
            Website = url ?? "-",
            Description = description ?? "-"
        };
        
        return (true, ctx);
    }

    public string WriteRemove()
    {
        Console.WriteLine("ACCOUNT LÖSCHEN");
        Console.WriteLine("Accountname:");
        var account = Console.ReadLine();
        if (!string.IsNullOrEmpty(account))
        {
            return account;
        }
        Console.WriteLine("FEHLER: Es wurde kein Accountname angegeben.");
        Console.WriteLine("Der Accountname ist ein Pflichtfeld.");
        return "";
    }

    public void WriteExit()
    {
        Console.WriteLine("Anwendung beendet.");
    }

    public (MenuAction Action, string Args) WriteMenu()
    {
        var validInput = true;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("""
                              ===========================
                                 PASSWORT MANAGEMENT
                              ===========================
                              [l] Accounts auflisten
                              [g <account name>] Ausgewählten Account ansehen
                              [c <account name>] Passwort des ausgewählten Accounts kopieren
                              [a] Account hinzufügen
                              [r] Account löschen
                              [pwd] Logindaten ändern
                              [e] Beenden
                              ===========================
                              """);
            Console.WriteLine(validInput ? "Bitte eine Option wählen:" : "Ungültige Eingabe!");
            
            var input = Console.ReadLine()?.Trim().ToLower().Split(" ");
            
            if (input is null || input.Length == 0)
            {
                validInput = false;
                continue;
            }
            
            var arg = input[0];
            var msg = input.Length > 1 ? input[1] : string.Empty;

            switch (arg)
            {
                case "l":
                    return (MenuAction.ViewAccounts, string.Empty);
                case "g":
                    return (MenuAction.GetAccount, msg);
                case "c":
                    return (MenuAction.CopyAccount, msg);
                case "a":
                    return (MenuAction.AddAccount, string.Empty);
                case "r":
                    return (MenuAction.RemoveAccount, string.Empty);
                case "pwd":
                    return (MenuAction.ChangeUserData, string.Empty);
                case "e":
                    return (MenuAction.Exit, string.Empty);
                default:
                    validInput = false;
                    break;
            }
        }
    }

    public void WriteDump(IContextService contextService)
    {
        var contexts = contextService.ContextsList;
        var counter = 0;
        if (contexts == null || contexts.Count == 0)
        {
            Console.WriteLine("Keine Accountdaten vorhanden.");
        }
        else
        {
            foreach (var c in contexts)
            {
                counter++;
                Console.WriteLine($"    | {counter}: {c.Name}");
            }
        }
        Console.ReadLine();
    }

    public void WriteDump(IDataContext context)
    {
        Console.WriteLine(context.ToString());
        Console.ReadLine();
    }   
}