namespace PWManager;
    public interface IPersistenceService
{
    void SaveData(string data);
    (bool Success, string data) LoadData();
    string GetPath();
}
