using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWManager

{
    public interface ICommunicationService
    {
        string WriteWelcome();
        string WriteRegister(Func<string, string, bool> validate, Func<string, string, string> token);
        bool IsRegister(bool isStartUp);
        string WriteLogin(Func<string, string, bool> validate, Func<string, string, string> token);
        DataContext WriteAdd();
        string WriteRemove();
        string WriteExit();
        MenuAction WriteShow();
    }
    public enum MenuAction
    {
        ViewAccounts,  // Accounts ansehen (s)
        AddAccount,    // Account hinzufügen (a)
        RemoveAccount, // Account löschen (r)
        Exit           // Exit (e)
    }
}