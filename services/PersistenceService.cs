using PWManager.interfaces;

namespace PWManager.services;

public class PersistenceService(string path) : IPersistenceService
{
    /// <summary>
    /// Lädt den Inhalt einer Datei und gibt ihn als String zurück.
    /// </summary>
    /// <returns>Der Inhalt der Datei als String.</returns>
    /// <exception cref="FileNotFoundException">Wird ausgelöst, wenn die Datei nicht gefunden wurde.</exception>
    /// <exception cref="UnauthorizedAccessException">Wird ausgelöst, wenn der Zugriff auf die Datei nicht erlaubt ist.</exception>
    /// <exception cref="IOException">Wird ausgelöst, wenn ein Fehler beim Lesen der Datei auftritt.</exception>
    /// <exception cref="Exception">Wird ausgelöst, wenn ein unerwarteter Fehler auftritt.</exception>
    public string LoadData()
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Datei wurde nicht gefunden.");
        try
        {
            return File.ReadAllText(path);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Zugriff nicht erlaubt: {ex.Message}");
            throw;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Datei konnte nicht gelesen werden: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Lesen der Datei: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Speichert den angegebenen Text in einer Datei.
    /// </summary>
    /// <param name="data">Der zu speichernde Text.</param>
    /// <exception cref="UnauthorizedAccessException">Wird ausgelöst, wenn der Zugriff auf die Datei nicht erlaubt ist.</exception>
    /// <exception cref="IOException">Wird ausgelöst, wenn ein Fehler beim Schreiben in die Datei auftritt.</exception>
    /// <exception cref="Exception">Wird ausgelöst, wenn ein unerwarteter Fehler auftritt.</exception>
    public void SaveData(string data)
    {
        try
        {
            File.WriteAllText(path, data);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Zugriff nicht erlaubt: {ex.Message}");
            throw;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Datei konnte nicht gespeichert werden: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Speichern der Datei: {ex.Message}");
            throw;
        }
    }

    public string GetPath()
    {
        return path;
    }
}
    
