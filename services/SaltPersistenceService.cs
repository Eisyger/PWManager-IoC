using PWManager.interfaces;

namespace PWManager.services;

public class SaltPersistenceService : PersistenceService
{
    public SaltPersistenceService(string path) : PersistenceService(path)
    {
        
    }
}