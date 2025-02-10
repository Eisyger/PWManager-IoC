using PWManager.Entity;
using PWManager.Interfaces;
using TextCopy;

namespace PWManager;

internal class App(
    ILoggingService loggingService,
    ICommunicationService com,
    ICypherService cypher,
    IContextService ctxService,
    IAuthenticationService authService,
    AccountContext account)
{
    public void Run()
    {
        if (account.Accounts.Any())
        {
            if(!com.Login())
                return;
        }
        else
        {
            // Nach einer Registrierung Neustart der App 
            com.Register();
            return;
        }
        
        ShowMenu();
    }
    
    private void ShowMenu()
    {
        while (true)
        {
            var state = com.Menu();

            switch (state.Action)
            {
                case MenuAction.ViewAccounts: HandleViewAccounts(); break;
                case MenuAction.AddAccount: HandleAddAccount();break;
                case MenuAction.RemoveAccount: HandleRemoveAccount(); break;
                case MenuAction.Exit: com.WriteExit(); SaveData(); return;
                case MenuAction.GetAccount: HandleGetAccount(state); break;
                case MenuAction.CopyAccount: HandleCopyToClipboard(state); break;
                case MenuAction.ChangeUserData: HandleChangeUserData(); break;
                default: throw new ArgumentOutOfRangeException(nameof(state.Action), state.Action, "Undefinierte MenuAction.");
            }
        }
    }
    private void HandleChangeUserData()
    {
        /*com.ChangeUserData(
            validationService.ValidateUserAndPassword,
            (u,p)=> authService.RecreateKey(u, p, "default"));
        SaveData();*/
    }
    private void HandleViewAccounts() => com.Dump(ctxService);
    private void HandleAddAccount()
    {
        var result = com.WriteAdd();
        if (result is not { Success: true, dataContext: not null }) return;
        ctxService.Add(result.dataContext);
        SaveData();
    }
    private void HandleRemoveAccount()
    {
        var name = com.Remove();
        if (ctxService.ContextsList?.Any(x => x.Name == name) == true)
        {
            ctxService.Remove(name);
            SaveData();
        }
        else
        {
            loggingService.Error($"Kein Account mit dem Namen {name} vorhanden.");
        }
    }
    private void HandleGetAccount((MenuAction action, string Args)state)
    {
        try
        {
            com.Dump(ctxService.GetContext(state.Args));
            Console.ReadLine();
        }
        catch (ArgumentNullException e)
        {
            loggingService.Log(e.Message);
        }
    }
    private void HandleCopyToClipboard((MenuAction action, string Args)state)
    {
        try
        {
            ClipboardService.SetText(ctxService.GetContext(state.Args).Password);
        }
        catch (ArgumentNullException e)
        {
            ClipboardService.SetText("");
            loggingService.Log(e.Message);
        }
    }
    private void SaveData()
    {
        if (account.Current == null) return;
        account.Current.Salt = authService.GenerateSalt();
        account.Current.EncryptedAccount = cypher.Encrypt(ctxService, authService.GenerateKey(account.AppKey, account.Current.Salt));
        account.SaveChanges();
    }
}
