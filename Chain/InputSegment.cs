using PWManager.Interfaces;
using PWManager.Services;

namespace PWManager.Chain;

public class InputSegment(ICommunicationService comService, IValidationService validate, IAuthenticationService auth) : Handler
{
    protected override void Process(ChainContext data)
    {
        if (data.Ctx.RawData.Length != 0 && data.Ctx.Salt.Length != 0)
        {
            comService.WriteLogin(validate.ValidateUserAndPassword, (u, p) => auth.RecreateKey(u,p, data.Ctx.Salt));
        }
        else
        {
            comService.WriteRegister(validate.ValidateUserAndPassword, (u, p) => auth.RecreateKey(u, p, "default"));
        }
        data.Ctx.Auth = auth;
    }
}