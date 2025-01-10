using PWManager.interfaces;

namespace PWManager.services;

public class PersistenceService(string path) : IPersistenceService
{
    public (bool Success, string data) LoadData()
    {
        if (!File.Exists(path))
            return (false, string.Empty);
        
        using var file = new FileStream(path, FileMode.OpenOrCreate);   
        var buffer = new byte[file.Length];
        var numBytes = file.Read(buffer, 0, buffer.Length);
        return (numBytes > 0, System.Text.Encoding.UTF8.GetString(buffer));
    }

    public void SaveData(string data)
    {
        using var file = new FileStream(path, FileMode.OpenOrCreate);       
        file.Write(System.Text.Encoding.UTF8.GetBytes(data));
    }

    public string GetPath()
    {
        return path;
    }
}
    
