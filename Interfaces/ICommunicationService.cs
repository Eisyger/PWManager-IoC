using PWManager.Model;

namespace PWManager.Interfaces;
    public interface ICommunicationService
    {
        void Welcome();
        bool Register();
        bool Login();
        bool ChangeUserData();
        (bool Success, AccountData? dataContext) WriteAdd();
        string Remove();
        void WriteExit();
        (MenuAction Action, string Args) Menu();
        void Dump(IContextService contextService);
        void Dump(IDataContext context);
    }
    public enum MenuAction
    {
        CopyAccount,   // Passwort kopieren (c)
        GetAccount,    // Ausgewählten Account anzeigen (g)
        ViewAccounts,  // Accounts ansehen (s)
        AddAccount,    // Account hinzufügen (a)
        RemoveAccount, // Account löschen (r)
        ChangeUserData,// Änder die Logindaten (pwd)
        Exit           // Exit (e)
    }