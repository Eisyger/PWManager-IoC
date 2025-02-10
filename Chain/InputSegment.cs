using PWManager.Entity;
using PWManager.Interfaces;
using PWManager.Services;

namespace PWManager.Chain;

public class InputSegment(ICommunicationService comService, IValidationService validate, IAuthenticationService auth, AccountContext account, ICypherService cypher) : Handler
{
    protected override void Process(IContextService context)
    {
        /*
        if (session.Sessions.Any())
        {
            auth.Key = comController.Login(validate.ValidateUserAndPassword,
                (u, p) =>
                {
                    var key = "";
                    session.CurrentSession = session.Sessions.FirstOrDefault(x => x.User == u);
                    
                    if (session.CurrentSession  != null)
                    {
                        // Key generieren
                        key = auth.RecreateKey(u, p,   session.CurrentSession.Salt);
                        // Auslesen der Daten mithilfe des Keys
                        context = cypher.Decrypt<ContextService>(session.CurrentSession.EncryptedAccoundService, key);
                        // Erstellen eines neuen Keys für die weitere Verschlüsselung
                        key = auth.CreateRandomKey();
                    }
                    return key;
                });
        }
        else
        {
            var user = "";
            var salt = "DefaulByRegister";
            // Ermittle Usernamen
            auth.Key = comController.Register(validate.ValidateUserAndPassword, 
                (u, p) =>
                {
                    user = u;
                    return auth.RecreateKey(u, p, salt);
                });

            session.CurrentSession = new SessionEntity()
            {
                User = user,
                Salt = salt,
                EncryptedAccoundService = cypher.Encrypt(context, auth.Key)
            };
            
            session.Sessions.Add(session.CurrentSession);
            session.SaveChanges();
        }*/
    }
}