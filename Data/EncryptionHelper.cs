using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UserAuthApp.Helpers
{
    public class EncryptionHelper
    {
        // AES Encryption with fixed-size key (32 bytes = 256-bit)
        public static string Encrypt(string plainText, string key)
        {
            // Ensure the key is exactly 32 bytes long (256-bit)
            byte[] keyBytes = GetKeyBytes(key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes; // Set the AES key
                aesAlg.IV = new byte[16]; // 16-byte IV for simplicity (in production, use a random IV)

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // AES Decryption with fixed-size key (32 bytes = 256-bit)
        public static string Decrypt(string cipherText, string key)
        {
            // Ensure the key is exactly 32 bytes long (256-bit)
            byte[] keyBytes = GetKeyBytes(key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes; // Set the AES key
                aesAlg.IV = new byte[16]; // 16-byte IV for simplicity (in production, use a random IV)

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        // Helper function to get key bytes from the provided key
        private static byte[] GetKeyBytes(string key)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Hash the key to ensure it's exactly 32 bytes
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }
    }
}
