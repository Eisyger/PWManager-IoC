namespace PWManager.interfaces;
    public interface IPersistenceService
{
    void SaveData(string data);
    // TODO Entferne das Tuple und füge stattdessen Exceptions hinzu.
    (bool Success, string data) LoadData();
    string GetPath();
}
