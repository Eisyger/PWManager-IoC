using PWManager.Interfaces;
using PWManager.Model;
using PWManager.Services;

namespace PWManager.Chain;

public class DecryptSegment(ICypherService cypher) : Handler
{
    protected override void Process(ChainContext data)
    {
        data.Ctx.ContextService = cypher.Decrypt<ContextService>(data.Ctx.RawData, data.Ctx.Auth.Key);
    }
}