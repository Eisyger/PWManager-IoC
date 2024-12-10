using System.Data;

namespace PWManager;

public class PersistenceService(string path) : IPersistenceService
{
    private readonly string _path = path;
    
    public (bool Success, string data) LoadData()
    {
        using var file = new FileStream(_path, FileMode.OpenOrCreate);   
        var buffer = new Span<byte>();
        var numBytes = file.Read(buffer);
        return (numBytes > 0, System.Text.Encoding.UTF8.GetString(buffer));
    }

    public void SaveData(string data)
    {
        using var file = new FileStream(_path, FileMode.OpenOrCreate);       
        file.Write(System.Text.Encoding.UTF8.GetBytes(data));
    }

    public string GetPath()
    {
        return _path;
    }
}
    
