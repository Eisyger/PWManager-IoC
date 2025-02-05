using PWManager.Chain;
using PWManager.Interfaces;
using PWManager.Services;
using TextCopy;

namespace PWManager;

internal class AppChainLogic(
    ILoggingService loggingService,
    ICommunicationService com,
    ICypherService cypher,
    IPersistenceService persistenceService,
    SaltPersistenceService saltPersistenceService,
    IContextService ctxService,
    IAuthenticationService authService,
    IValidationService validationService)
{
    private IContextService _ctxService = ctxService;

    private readonly LoadSegment _load = new LoadSegment(persistenceService, saltPersistenceService);
    private readonly InputSegment _input = new InputSegment(com, validationService, authService);
    private readonly DecryptSegment _decrypt = new DecryptSegment(cypher);
    private readonly ChainContext _segment = new ChainContext();

    public void Run()
    {
        // StartUp Chain
        // 1. Lade SaveFile und SaltFile
        // 2. Benutzereingabe für Username und Passwort
        // 3. Entschlüssele die SaveFile mit den Benutzereingaben und dem SaltFile
        _load.SetNext(_input);
        _input.SetNext(_decrypt);
        _load.Handle(_segment);
        // _segment hat nun den Kompletten ContextService aus der SaveFile
        _ctxService = _segment.Ctx.ContextService;
        
        ShowMenu();
    }
    
    private void ShowMenu()
    {
        while (true)
        {
            var state = com.WriteMenu();

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
        com.WriteChangeUserData(
            validationService.ValidateUserAndPassword,
            (u,p)=> authService.RecreateKey(u, p, "default"));
        SaveData();
    }
    private void HandleViewAccounts() => com.WriteDump(_ctxService);
    private void HandleAddAccount()
    {
        var result = com.WriteAdd();
        if (result is not { Success: true, dataContext: not null }) return;
        _ctxService.Add(result.dataContext);
        SaveData();
    }
    private void HandleRemoveAccount()
    {
        var name = com.WriteRemove();
        if (_ctxService.ContextsList?.Any(x => x.Name == name) == true)
        {
            _ctxService.Remove(name);
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
            com.WriteDump(_ctxService.GetContext(state.Args));
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
            ClipboardService.SetText(_ctxService.GetContext(state.Args).Password);
        }
        catch (ArgumentNullException e)
        {
            ClipboardService.SetText("");
            loggingService.Log(e.Message);
        }
    }
    private void SaveData()
    {
        persistenceService.SaveData(cypher.Encrypt(_ctxService, authService.CreateRandomKey()));
        saltPersistenceService.SaveData(authService.Salt);
    }
}
