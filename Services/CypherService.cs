using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PWManager.Interfaces;
using PWManager.Model;

namespace PWManager.Services;
public class CypherService : ICypherService
{
    /// <summary>
    /// Verschlüsselt ein Objekt vom Typ <typeparamref name="T"/> und gibt einen verschlüsselten Hexadezimal-String zurück.
    /// </summary>
    /// <typeparam name="T">Der Typ des zu verschlüsselnden Objekts.</typeparam>
    /// <param name="context">Das Objekt, das verschlüsselt werden soll.</param>
    /// <param name="token">Ein geheimer Schlüssel, der für die Verschlüsselung verwendet wird.</param>
    /// <returns>Ein Hexadezimal-String, der die verschlüsselten Daten darstellt.</returns>
    /// <exception cref="ArgumentNullException">
    /// Wird ausgelöst, wenn <paramref name="context"/> oder <paramref name="token"/> null, leer oder ungültig ist.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Wird ausgelöst, wenn <paramref name="token"/> leer oder ungültig ist.
    /// </exception>
    /// <remarks>
    /// Diese Methode serialisiert das Eingabeobjekt in JSON, verschlüsselt es mit dem AES-Algorithmus und gibt 
    /// den verschlüsselten Text als Hexadezimal-String zurück. Bei Fehlern in der Serialisierung oder Verschlüsselung 
    /// wird eine entsprechende Ausnahme ausgelöst.
    /// </remarks>
    public string Encrypt<T>(T context, string token)
    {
        // Überprüfen, ob der Kontext null ist (nur relevant für Referenztypen)
        if (context == null)
            throw new ArgumentNullException(nameof(context), "Das zu verschlüsselnde Objekt darf nicht null sein.");
        
        // Überprüfen, ob der Token null oder leer ist
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Der Verschlüsselungstoken darf nicht null, leer oder nur Leerzeichen sein.", nameof(token));
        
        // AES erstellen und Schlüssel sowie Initialisierungsvektor generieren
        using var aes = Aes.Create();
        (aes.Key, aes.IV) = GenerateKeyAndIv(token);

        // Serialisiere den Kontext und konvertiere den Text in ein Byte-Array
        try
        {
            var serializedData = JsonSerializer.Serialize(context);
            var plaintextBytes = Encoding.UTF8.GetBytes(serializedData);

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var encryptedBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

            // Verschlüsselten Text als Hexadezimal-String
            return Convert.ToHexString(encryptedBytes);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Fehler bei der Serialisierung des Kontextes.", ex);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(
                $"Ein unerwarteter Fehler ist aufgetreten. {e.GetType()} {e.Message}", e);
        }
    }
    
    /// <summary>
    /// Verschlüsselt ein Objekt vom Typ <typeparamref name="T"/> und gibt einen verschlüsselten Hexadezimal-String zurück.
    /// </summary>
    /// <typeparam name="T">Der Typ des zu verschlüsselnden Objekts.</typeparam>
    /// <param name="context">Das Objekt, das verschlüsselt werden soll.</param>
    /// <param name="token">Ein geheimer Schlüssel, der für die Verschlüsselung verwendet wird.</param>
    /// <returns>Ein Hexadezimal-String, der die verschlüsselten Daten darstellt.</returns>
    /// <exception cref="ArgumentNullException">
    /// Wird ausgelöst, wenn <paramref name="context"/> oder <paramref name="token"/> null, leer oder ungültig ist.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Wird ausgelöst, wenn <paramref name="token"/> leer oder ungültig ist.
    /// </exception>
    /// <remarks>
    /// Diese Methode serialisiert das Eingabeobjekt in JSON, verschlüsselt es mit dem AES-Algorithmus und gibt 
    /// den verschlüsselten Text als Hexadezimal-String zurück. Bei Fehlern in der Serialisierung oder Verschlüsselung 
    /// wird eine entsprechende Ausnahme ausgelöst.
    /// </remarks>
    public async Task<string> EncryptAsync<T>(T context, string token)
    {
        // Überprüfen, ob der Kontext null ist (nur relevant für Referenztypen)
        if (context == null)
            throw new ArgumentNullException(nameof(context), "Das zu verschlüsselnde Objekt darf nicht null sein.");
        
        // Überprüfen, ob der Token null oder leer ist
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Der Verschlüsselungstoken darf nicht null, leer oder nur Leerzeichen sein.", nameof(token));

        return await Task.Run(() =>
        {
            // AES erstellen und Schlüssel sowie Initialisierungsvektor generieren
            using var aes = Aes.Create();
            (aes.Key, aes.IV) = GenerateKeyAndIv(token);

            // Serialisiere den Kontext und konvertiere den Text in ein Byte-Array
            try
            {
                var serializedData = JsonSerializer.Serialize(context);
                var plaintextBytes = Encoding.UTF8.GetBytes(serializedData);

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                var encryptedBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

                // Verschlüsselten Text als Hexadezimal-String
                return Convert.ToHexString(encryptedBytes);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Fehler bei der Serialisierung des Kontextes.", ex);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"Ein unerwarteter Fehler ist aufgetreten. {e.GetType()} {e.Message}", e);
            }
        });
    }

    
    /// <summary>
    /// Entschlüsselt einen Hexadezimal-String, der mit der <see cref="Encrypt{T}"/>-Methode verschlüsselt wurde, 
    /// und gibt das ursprüngliche Objekt des Typs <typeparamref name="T"/> zurück.
    /// </summary>
    /// <typeparam name="T">Der Typ des Objekts, das entschlüsselt werden soll.</typeparam>
    /// <param name="ciphertext">Der verschlüsselte Hexadezimal-String.</param>
    /// <param name="token">Der geheime Schlüssel, der für die Entschlüsselung verwendet wird.</param>
    /// <returns>Eine Instanz des Typs <typeparamref name="T"/>, die die entschlüsselten Daten enthält.</returns>
    /// <exception cref="ArgumentException">
    /// Wird ausgelöst, wenn <paramref name="ciphertext"/> oder <paramref name="token"/> null, leer oder ungültig ist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Wird ausgelöst, wenn ein Fehler bei der Entschlüsselung oder der Deserialisierung auftritt.
    /// </exception>
    /// <remarks>
    /// Diese Methode wandelt den Hexadezimal-String in ein Byte-Array um, entschlüsselt es mit dem AES-Algorithmus 
    /// und deserialisiert den JSON-String zurück in ein Objekt des Typs <typeparamref name="T"/>.
    /// </remarks>
    public T Decrypt<T>(string ciphertext, string token)
    {
        if (string.IsNullOrWhiteSpace(ciphertext))
            throw new ArgumentException("Der verschlüsselte Text (ciphertext) darf nicht null oder leer sein.", nameof(ciphertext));
        if (ciphertext.Length % 2 != 0)
            throw new ArgumentException("Ungültiger Hexadezimal-String.");
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Das Token darf nicht null oder leer sein.", nameof(token));

        using var aes = Aes.Create();
        (aes.Key, aes.IV) = GenerateKeyAndIv(token);

        // Konvertiere den Hexadezimal-verschlüsselten Text zurück in Bytes
        var encryptedBytes = Convert.FromHexString(ciphertext);

        try
        {
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            
            return JsonSerializer.Deserialize<T>(decryptedText)
                   ?? throw new InvalidOperationException("Die Deserialisierung ergab ein null-Wert.");
        }
        catch (CryptographicException e)
        {
            throw new InvalidOperationException("Fehler bei der Entschlüsselung: Überprüfe Token und Eingabe.", e);
        }
        catch (JsonException e)
        {
            throw new InvalidOperationException($"Fehler bei der Deserialisierung in den Typ {typeof(T)}.", e);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Ein unerwarteter Fehler ist aufgetreten. {e.GetType()} {e.Message}", e);
        }
    }

    public async Task<T> DecryptAsync<T>(string plaintext, string token)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
            throw new ArgumentException("Der verschlüsselte Text (ciphertext) darf nicht null oder leer sein.", nameof(plaintext));
        if (plaintext.Length % 2 != 0)
            throw new ArgumentException("Ungültiger Hexadezimal-String.");
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Das Token darf nicht null oder leer sein.", nameof(token));

        return await Task.Run(() =>
        {
            using var aes = Aes.Create();
            (aes.Key, aes.IV) = GenerateKeyAndIv(token);

            // Konvertiere den Hexadezimal-verschlüsselten Text zurück in Bytes
            var encryptedBytes = Convert.FromHexString(plaintext);

            try
            {
                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                var decryptedText = Encoding.UTF8.GetString(decryptedBytes);

                return JsonSerializer.Deserialize<T>(decryptedText)
                       ?? throw new InvalidOperationException("Die Deserialisierung ergab ein null-Wert.");
            }
            catch (CryptographicException e)
            {
                throw new InvalidOperationException("Fehler bei der Entschlüsselung: Überprüfe Token und Eingabe.", e);
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException($"Fehler bei der Deserialisierung in den Typ {typeof(T)}.", e);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"Ein unerwarteter Fehler ist aufgetreten. {e.GetType()} {e.Message}", e);
            }
        });
    }

    private static (byte[] key, byte[] iv) GenerateKeyAndIv(string token)
    {        
        var key = new byte[16];
        var iv = new byte[16];
        
        // Erzeuge SHA256-Hash aus dem token       
        var hash = SHA512.HashData(Encoding.UTF8.GetBytes(token));

        // Verwende die ersten 16 Bytes als Schlüssel und die nächsten 16 Bytes als IV
        Array.Copy(hash, 0, key, 0, 16);
        Array.Copy(hash, 16, iv, 0, 16);
       
        return (key, iv);
    }
}
