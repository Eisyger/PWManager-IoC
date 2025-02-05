using PWManager.Interfaces;
using PWManager.Services;

namespace PWManager.Chain;

public class DecryptSegment(ICypherService cypher) : HandlerAsync
{
    protected override async Task Process(ChainContext data)
    {
        data.Ctx.ContextService = cypher.DecryptAsync<ContextService>(data.Ctx.RawData, data.Ctx.Auth.Key).Result;
    }
}