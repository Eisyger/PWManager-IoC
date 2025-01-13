using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PWManager.interfaces;
using PWManager.model;

namespace PWManager.services;
public class CypherService : ICypherService
{
    private (byte[] key, byte[] iv) GenerateKeyAndIV(string token)
    {
        try
        {
            // Erzeuge SHA256-Hash aus der Kombination       
            var hash = SHA512.HashData(Encoding.UTF8.GetBytes(token));

            // Verwende die ersten 16 Bytes als Schlüssel und die nächsten 16 Bytes als IV
            byte[] key = new byte[16];
            byte[] iv = new byte[16];
            Array.Copy(hash, 0, key, 0, 16);
            Array.Copy(hash, 16, iv, 0, 16);
            return (key, iv);
        }
        catch (Exception e)
        {
            Console.WriteLine("Fehler umwandeln des Tokens in Key ind IV -> GenerateKeyAndIV. " + e.Message);
            throw;
        }
    }
    public string Encrypt(IContextService ctxService, string token)
    {
        
        using var aes = Aes.Create();
        (aes.Key, aes.IV) = GenerateKeyAndIV(token);

        // Serialisiere den Kontext und Konvertiere den Text in ein Byte-Array
        var serializedData = JsonSerializer.Serialize(ctxService.ContextsList);
        var plaintextBytes = Encoding.UTF8.GetBytes(serializedData);

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var encryptedBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        // Verschlüsselten Text als Hexadezimal-String zurückgeben
        return Convert.ToHexString(encryptedBytes);
    }

    public (bool Success, IContextService? contextService) Decrypt(string ciphertext, string token)
    {
        using var aes = Aes.Create();
        (aes.Key, aes.IV) = GenerateKeyAndIV(token);
       
            // Konvertiere den Hexadezimal-verschlüsselten Text zurück in Bytes
            var encryptedBytes = ConvertHexStringToByteArray(ciphertext);
            try
            {
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            
            // Entschlüsselten Text in UTF8-String umwandeln
            var decryptedText = Encoding.UTF8.GetString(decryptedBytes);

            var ctx = new ContextService
            {
                ContextsList = JsonSerializer.Deserialize<List<DataContext>>(decryptedText)
            };
            return (true, ctx);
        }
        catch (Exception e)
        {
            Console.WriteLine("Fehler beim Entschlüsseln der Daten. Ungültiges Token, der Username oder das Passwort sind falsch.\n" + e.Message);
            return (false, null);
        }
    }

    // Hilfsmethode: Konvertiere Hexadezimal-String in Byte-Array
    private static byte[] ConvertHexStringToByteArray(string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Ungültiger Hexadezimal-String.");

        var byteArray = new byte[hex.Length / 2];
        for (var i = 0; i < byteArray.Length; i++)
        {
            byteArray[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return byteArray;
    }


    public string HashPassword(string username, string password)
    {
        const string salt = "2ex4|198$";
        var combined = username + salt + password;
        
        // Hash erstellen und in Hex-Format umwandeln
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(combined));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    public string CreateToken(string username, string password)
    {
        const string salt = "984!489///>(8)";
        var combined = username + salt + password;
        
        // Hash erstellen und in Hex-Format umwandeln
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(combined));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
