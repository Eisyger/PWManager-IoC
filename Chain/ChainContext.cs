using PWManager.Interfaces;
using PWManager.Model;
using PWManager.Services;

namespace PWManager.Chain;

public class ChainContext
{
    public StartupDataStruct Ctx = new StartupDataStruct();
}

public struct StartupDataStruct
{
    public string RawData { get; set; }
    public string Salt { get; set; }
    public ContextService ContextService { get; set; }
    public IAuthenticationService Auth { get; set; }
}