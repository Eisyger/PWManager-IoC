using PWManager.Entity;
using PWManager.Interfaces;
using PWManager.Model;

namespace PWManager.Services;

public class ConsoleCommunicationService(AccountContext accCtx,IAppKeyService appKeyService, IValidationService validation, IAuthenticationService auth, IContextService context, ICypherService cypher) : ICommunicationService       
{

    public void Welcome()
    {
        Console.WriteLine("Willkommen!");
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
                Console.WriteLine("REGISTRIERUNG");
                Console.WriteLine("Gib einen Usernamen an:");
                var user = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine("Gib ein Masterpasswort an:");
                var pwd = PasswordReader.ReadMaskedPassword();
                
                var s = accCtx.Accounts.FirstOrDefault(x => x.User == user);
                if (s != null)
                {
#if DEBUG
                    Console.WriteLine("Username existiert bereits.");
#endif
                    Console.WriteLine("Username oder Passwort falsch.");
                    Thread.Sleep(1000);
                    continue;
                }
                
                var result = validation.ValidateUserAndPassword(user, pwd);
                if (result.Valid)
                {
                    appKeyService.Key = auth.GenerateAppKey(user,pwd);
                    var salt = auth.GenerateSalt();
                    var key = auth.GenerateKey(appKeyService.Key, salt);
                    accCtx.Accounts.Add(new AccountEntity()
                    {
                        User = user,
                        Salt = salt,
                        EncryptedAccount = cypher.Encrypt(new AccountService()
                        {
                            User = user
                        }, key)
                    });
                    // Schreibt einen Eintrag in die DB
                    accCtx.SaveChanges();
                    
                    Console.WriteLine("Registrierung erfolgreich. Starte das programm erneut...");
                    Console.ReadLine();
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
                Console.WriteLine("LOGIN");
                Console.WriteLine("Gib deinen Usernamen an:");
                var user = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine("Gib dein Masterpasswort an:");
                var pwd = PasswordReader.ReadMaskedPassword();
                
                var s = accCtx.Accounts.FirstOrDefault(x => x.User == user);
                if (s == null)
                {
                    Console.WriteLine($"Username oder Passwort Falsch.");
                    continue;
                }
                
                
                var key = auth.GenerateKey(auth.GenerateAppKey(user, pwd), s.Salt);
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
                appKeyService.Key = auth.GenerateAppKey(user, pwd);
                    
                context.User = ctx.User;
                context.ContextsList = ctx.ContextsList;
                
                Console.WriteLine("Login Erfolgreich!");
                return true;
        }
        Console.WriteLine("Zu viele Fehlversuche. Registrierung abgebrochen.");
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
            Console.WriteLine("ÄNDERE LOGINDATEN");
            Console.WriteLine("Gib einen neuen Usernamen an:");
            var user = Console.ReadLine()?.Trim() ?? "";
            Console.WriteLine("Gib ein neues Masterpasswort an:");
            var pwd = PasswordReader.ReadMaskedPassword();
                
            var s = accCtx.Accounts.FirstOrDefault(x => x.User == user);
            if (s != null)
            {
#if DEBUG
                Console.WriteLine("Username existiert bereits.");
#endif
                Console.WriteLine("Username oder Passwort falsch.");
                Thread.Sleep(1000);
                continue;
            }
                
            var result = validation.ValidateUserAndPassword(user, pwd);
            if (result.Valid)
            {
                appKeyService.Key = auth.GenerateAppKey(user,pwd);
                var salt = auth.GenerateSalt();
                var key = auth.GenerateKey(appKeyService.Key, salt);
                
                var newAcc =  new AccountEntity()
                {
                    User = user,
                    Salt = salt,
                    EncryptedAccount = cypher.Encrypt(new AccountService()
                    {
                        User = user
                    }, key)
                };
                
                // Fügen neuen Account hinzu, lösche alten und mache den neuen zum aktuellen
                accCtx.Accounts.Add(newAcc);
                if (accCtx.CurrentAccEntity != null) accCtx.Accounts.Remove(accCtx.CurrentAccEntity);
                accCtx.CurrentAccEntity = newAcc;
                
                // Schreibt einen Eintrag in die DB
                accCtx.SaveChanges();
                
                Console.WriteLine("Daten wurden geändert.");
                Console.ReadLine();
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
            Password = pwd,
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
    public void Dump(IContextService contextService)
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
    public void Dump(IDataContext dataContext)
    {
        Console.WriteLine(dataContext.ToString());
        Console.ReadLine();
    }   
}