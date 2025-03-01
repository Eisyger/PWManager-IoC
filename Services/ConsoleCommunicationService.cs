using PWManager.Context;
using PWManager.Interfaces;
using PWManager.Data;

namespace PWManager.Services;

public class ConsoleCommunicationService(AccountContext accCtx,IAppKeyService appKeyService, IValidationService validation, IAuthenticationService auth, IContextService context, ICypherService cypher) : ICommunicationService       
{

    public void Welcome()
    {
        Console.WriteLine("Willkommen!");
    }

    private (string User, char[] Pwd) GetUserAndPassword(string title, bool doubleCheckPassword = false)
    {
        while (true)
        {
            Console.WriteLine(title);
            Console.WriteLine("Gib einen Usernamen an:");
            var user = Console.ReadLine()?.Trim() ?? "";
        
            Console.WriteLine("Gib ein Masterpasswort an:");
            var pwd = PasswordReader.ReadMaskedPassword();
            
            if (doubleCheckPassword)
            {
                Console.WriteLine("Wiederhole dein Masterpasswort:");
                var pwd2 = PasswordReader.ReadMaskedPassword();
                if (pwd.SequenceEqual(pwd2)) return (user, pwd);
                
                Console.WriteLine("Die Passwörter stimmen nicht überein.");
                continue;
            }

            return (user, pwd);
        }
    }
    public bool Register()
    {
        const int maxAttempts = 3;
        for (var i = 0; i < maxAttempts; i++)
        {
#if DEBUG
            Console.WriteLine("##############################################");
#else
                Console.Clear();
#endif
            var input = GetUserAndPassword("REGISTRIERUNG", doubleCheckPassword: true);
              
            var s = accCtx.Accounts.FirstOrDefault(x => x.User == input.User);
            if (s != null)
            {
#if DEBUG
                Console.WriteLine("Username existiert bereits.");
#endif
                Console.WriteLine("Username oder Passwort falsch.");
                Thread.Sleep(1000);
                continue;
            }
            
            var result = validation.ValidateUserAndPassword(input.User, input.Pwd);
            if (result.Valid)
            {
                appKeyService.Key = auth.GenerateAppKey(input.User,input.Pwd);
                var salt = auth.GenerateSalt();
                var key = auth.GenerateKey(appKeyService.Key, salt);
                accCtx.Accounts.Add(new AccountEntity()
                {
                    User = input.User,
                    Salt = salt,
                    EncryptedAccount = cypher.Encrypt(new AccountService()
                    {
                        User = input.User,
                    }, key)
                });
                // Schreibt einen Eintrag in die DB
                accCtx.SaveChanges();
                
                // Lösche das Passwort
                Array.Clear(input.Pwd, 0, input.Pwd.Length);
                
                Console.WriteLine("Registrierung erfolgreich. Starte das programm erneut...");
                Console.ReadKey();
                return true;
            }
            Console.WriteLine(result.Message);
        }
        Console.WriteLine("Zu viele Fehlversuche. Registrierung abgebrochen.");
        return false;
    }
    
    public bool Login()
    {
        const int maxAttempts = 3;
        for (var i = 0; i < maxAttempts; i++)
        {
#if DEBUG
            Console.WriteLine("##############################################");
#else
        Console.Clear();
#endif
            var input = GetUserAndPassword("LOGIN");
       
            var s = accCtx.Accounts.FirstOrDefault(x => x.User == input.User);
            if (s == null)
            {
                Console.WriteLine($"Username oder Passwort Falsch.");
                continue;
            }
            
            var key = auth.GenerateKey(auth.GenerateAppKey(input.User, input.Pwd), s.Salt);
            AccountService ctx;
            try
            {
                // Tritt beim Laden des ContextService ein Fehler auf, so sind die Logindaten falsch, oder die DB ist defekt. Letzteres wird ignoriert.
                ctx = cypher.Decrypt<AccountService>(s.EncryptedAccount, key);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Fehler beim entschlüsseln");
                Console.WriteLine(ex.Message);
#endif
                Console.WriteLine("Username oder Passwort falsch.");
                Thread.Sleep(1000);
                continue;
            }
        
            accCtx.CurrentAccEntity = s;
        
            // AppKey für die Verschlüsselung während der Laufzeit, ohne erneuten Zugriff auf Username und Passwort. 
            appKeyService.Key = auth.GenerateAppKey(input.User, input.Pwd);
            
            Array.Clear(input.Pwd, 0, input.Pwd.Length);
            
            context.User = ctx.User;
            context.AccountList = ctx.AccountList;
        
            // Lösche das Passwort
            Array.Clear(input.Pwd, 0, input.Pwd.Length);
            
            Console.WriteLine("Login Erfolgreich!");
            return true;
        }
        Console.WriteLine("Zu viele Fehlversuche. Login abgebrochen.");
        return false;
    }

    public bool ChangeUserData()
    {
        const int maxAttempts = 3;
        for (var i = 0; i < maxAttempts; i++)
        {
#if DEBUG
            Console.WriteLine("##############################################");
#else
                Console.Clear();
#endif
            var input = GetUserAndPassword("BENUTZERDATEN ÄNDERN");
                
            var s = accCtx.Accounts.FirstOrDefault(x => x.User == input.User);
            if (s != null)
            {
#if DEBUG
                Console.WriteLine("Username existiert bereits.");
#endif
                Console.WriteLine("Username oder Passwort falsch.");
                Thread.Sleep(1000);
                continue;
            }
                
            var result = validation.ValidateUserAndPassword(input.User, input.Pwd);
            if (result.Valid)
            {
                appKeyService.Key = auth.GenerateAppKey(input.User,input.Pwd);
                var salt = auth.GenerateSalt();
                var key = auth.GenerateKey(appKeyService.Key, salt);
                
                var newAcc =  new AccountEntity()
                {
                    User = input.User,
                    Salt = salt,
                    EncryptedAccount = cypher.Encrypt(new AccountService()
                    {
                        User = input.User
                    }, key)
                };
                
                // Fügen neuen Account hinzu, lösche alten und mache den neuen zum aktuellen
                accCtx.Accounts.Add(newAcc);
                if (accCtx.CurrentAccEntity != null) accCtx.Accounts.Remove(accCtx.CurrentAccEntity);
                accCtx.CurrentAccEntity = newAcc;
                
                // Schreibt einen Eintrag in die DB
                accCtx.SaveChanges();
                
                // Lösche das Passwort
                Array.Clear(input.Pwd, 0, input.Pwd.Length);
                
                Console.WriteLine("Daten wurden geändert.");
                Console.ReadKey();
                return true;
            }
            Console.WriteLine(result.Message);
        }
        Console.WriteLine("Zu viele Fehlversuche. Umbenennung abgebrochen.");
        return false;
    }
    
    public (bool Success, AccountData? dataContext) WriteAdd()
    {
        Console.WriteLine("ACCOUNT HINZUFÜGEN");
        Console.WriteLine("Accountname:");
        var name = Console.ReadLine()?.Trim();
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

        var ctx = new AccountData()
        {
            Name = name,
            User = user ?? "-",
            Password = new string(pwd),
            Website = url ?? "-",
            Description = description ?? "-"
        };
        
        return (true, ctx);
    }
    public string Remove()
    {
        Console.WriteLine("ACCOUNT LÖSCHEN");
        Console.WriteLine("Accountname:");
        var acc = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(acc))
        {
            return acc;
        }
        Console.WriteLine("FEHLER: Es wurde kein Accountname angegeben.");
        Console.WriteLine("Der Accountname ist ein Pflichtfeld.");
        return "";
    }
    public void WriteExit()
    {
        Console.WriteLine("Anwendung beendet.");
    }
    public (MenuAction Action, string Args) Menu()
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
            
            var input = Console.ReadLine()?.Trim().ToLower()?.Split(" ");
            
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
    public void Dump(IContextService contextService)
    {
        var contexts = contextService.AccountList;
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
        Console.ReadKey();
    }
    public void Dump(IDataContext dataContext)
    {
        Console.WriteLine(dataContext.ToString());
        Console.ReadKey();
    }   
}