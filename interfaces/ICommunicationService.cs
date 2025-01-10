using PWManager.interfaces;
using PWManager.model;

namespace PWManager

{
    public interface ICommunicationService
    {
        string WriteWelcome();
        string WriteRegister(Func<string, string, bool> validate, Func<string, string, string> token);
        string WriteLogin(Func<string, string, bool> validate, Func<string, string, string> token);
        (bool Success, DataContext? dataContext) WriteAdd();
        string WriteRemove();
        void WriteExit();
        (MenuAction Action, string Args) WriteMenu();
        void WriteDump(IContextService contextService);
        void WriteDump(IDataContext context);
    }
    public enum MenuAction
    {
        CopyAccount,   // Passwort kopieren (c)
        GetAccount,    // Ausgewählten Account anzeigen (g)
        ViewAccounts,  // Accounts ansehen (s)
        AddAccount,    // Account hinzufügen (a)
        RemoveAccount, // Account löschen (r)
        Exit           // Exit (e)
    }
}