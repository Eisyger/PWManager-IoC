namespace PWManager.interfaces;
    public interface IPersistenceService
{
    void SaveData(string data);
    string LoadData();
    string GetPath();
}
