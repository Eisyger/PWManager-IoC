using System;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;

namespace PWManager;
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
    public string Encrypt(string plaintext, string token)
    {
        using var aes = Aes.Create();
        (aes.Key, aes.IV) = GenerateKeyAndIV(token);

        // Konvertiere den Text in ein Byte-Array
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var encryptedBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        // Verschlüsselten Text als Hexadezimal-String zurückgeben
        return BitConverter.ToString(encryptedBytes).Replace("-", string.Empty);
    }

    public (bool Success, string? DecryptedText) Decrypt(string ciphertext, string token)
    {
        using var aes = Aes.Create();
        (aes.Key, aes.IV) = GenerateKeyAndIV(token);

       
            // Konvertiere den Hexadezimal-verschlüsselten Text zurück in Bytes
            var encryptedBytes = ConvertHexStringToByteArray(ciphertext);
            try
            {
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            // Entschlüsselten Text als UTF8-String zurückgeben
            return (true, Encoding.UTF8.GetString(decryptedBytes));
        }
        catch (Exception e)
        {
            Console.WriteLine("Fehler beim Entschlüsseln der Daten. Ungültiges Token, der Username oder das Passwort sind falsch. " + e.Message);
            return (false, string.Empty);
        }
    }

// Hilfsmethode: Konvertiere Hexadezimal-String in Byte-Array
    private static byte[] ConvertHexStringToByteArray(string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Ungültiger Hexadezimal-String.");

        var byteArray = new byte[hex.Length / 2];
        for (int i = 0; i < byteArray.Length; i++)
        {
            byteArray[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return byteArray;
    }


    public string HashPassword(string username, string password)
    {
        var salt = "2ex4|198$";
        var combined = username + salt + password;
        
        // Hash erstellen und in Hex-Format umwandeln
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(combined));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    public static string CreateToken(string username, string password)
    {
        var salt = "984!489///>(8)";
        var combined = username + salt + password;
        
        // Hash erstellen und in Hex-Format umwandeln
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(combined));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
