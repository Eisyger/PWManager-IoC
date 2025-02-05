using PWManager.Interfaces;

namespace PWManager.Chain;

public class LoadSegment(IPersistenceService perService,IPersistenceService perSalt) : HandlerAsync
{
    protected override Task Process(ChainContext data)
    {
        data.Ctx.RawData = perService.LoadData();
        data.Ctx.Salt = perSalt.LoadData();
        
        return Task.CompletedTask;
    }
}