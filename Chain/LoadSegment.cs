using PWManager.Interfaces;

namespace PWManager.Chain;

public class LoadSegment(IPersistenceService perService,IPersistenceService perSalt) : Handler
{
    protected override void Process(ChainContext data)
    {
        data.Ctx.RawData = perService.LoadData();
        data.Ctx.Salt = perSalt.LoadData();
    }
}