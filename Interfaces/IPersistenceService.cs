namespace PWManager.Interfaces;
    public interface IPersistenceService
{
    void SaveData(string data);
    string LoadData();
    string GetPath();
}
