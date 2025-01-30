using PWManager.Chain;
using PWManager.Interfaces;
using PWManager.Services;

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
        _load.SetNext(_input);
        _input.SetNext(_decrypt);
        
        _load.Handle(_segment);

        _ctxService = _segment.Ctx.ContextService;
       
        foreach (var c in _ctxService.ContextsList)
        {
            Console.WriteLine(c);
        }
    }
}
