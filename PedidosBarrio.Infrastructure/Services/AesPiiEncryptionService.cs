using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Infrastructure.Services
{
    public class AesPiiEncryptionService : IPiiEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesPiiEncryptionService(IConfiguration configuration)
        {
            var secret = configuration["PiiEncryption:Key"] ?? "32CharactersLongHardcodedDefaultKey!";
            _key = Encoding.UTF8.GetBytes(secret.PadRight(32).Substring(0, 32));
            _iv = Encoding.UTF8.GetBytes((configuration["PiiEncryption:IV"] ?? "16CharLongIV!!!!").PadRight(16).Substring(0, 16));
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

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

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _key;
                    aesAlg.IV = _iv;

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
                        catch
                        {
                            // Return original if decryption fails (might not be encrypted yet)
                            return cipherText;
                        }
                    }
                }
            }
