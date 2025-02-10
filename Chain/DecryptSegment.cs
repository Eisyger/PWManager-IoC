using PWManager.Entity;
using PWManager.Interfaces;
using PWManager.Model;
using PWManager.Services;

namespace PWManager.Chain;

public class DecryptSegment(ICypherService cypher) : Handler
{
    protected override void Process(IContextService session)
    {/*
        if (session.Key == "default")
        {
            session.CurrentContext =
                cypher.Decrypt<ContextService>(session.CurrentSession.EncryptedAccoundService, session.Key);
        }*/
    }
}