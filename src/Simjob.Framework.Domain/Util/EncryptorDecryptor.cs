using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Simjob.Framework.Domain.Util
{
    public class EncryptorDecryptor
    {
        private const string pwd = "BkLVpwfInPMx4U8YcHIKMH/ez6G9/5Te6yCWbAhftYM=";

        private const string EncryptionKey = "b41d0d9ec06e49b2d7c92f0dce5c6c6d8df0800f06e61beff0e93d3621dd07e3"; // Replace with your secret key
        private const string InitializationVector = "7c9ce96897b845e6a47f5d71d76ec293"; // Replace with your initialization vector

        public static string EncryptApiKey(string plainText)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = HexStringToByteArray(EncryptionKey);
            aesAlg.IV = HexStringToByteArray(InitializationVector);

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        private static byte[] HexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
        public static string DecryptApiKey(string cipherText)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = HexStringToByteArray(EncryptionKey);
            aesAlg.IV = HexStringToByteArray(InitializationVector);

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }


        private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;


            byte[] saltBytes = passwordBytes;


            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (CryptoStream cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            try
            {
                byte[] decryptedBytes = null;

                byte[] saltBytes = passwordBytes;


                using (MemoryStream ms = new MemoryStream())
                {
                    using (RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        //AES.Mode = CipherMode.CBC;  

                        using (CryptoStream cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                            //If(cs.Length = ""  
                            cs.Close();
                        }
                        decryptedBytes = ms.ToArray();
                    }
                }
                return decryptedBytes;
            }
            catch (Exception Ex)
            {
                return null;
            }
        }
        public string Encrypt(string text, string pwd = pwd)
        {
            byte[] originalBytes = Encoding.UTF8.GetBytes(text);
            byte[] encryptedBytes = null;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(pwd);

            // Hash the password with SHA256  
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            // Getting the salt size  
            int saltSize = GetSaltSize(passwordBytes);
            // Generating salt bytes  
            byte[] saltBytes = GetRandomBytes(saltSize);

            // Appending salt bytes to original bytes  
            byte[] bytesToBeEncrypted = new byte[saltBytes.Length + originalBytes.Length];
            for (int i = 0; i < saltBytes.Length; i++)
            {
                bytesToBeEncrypted[i] = saltBytes[i];
            }
            for (int i = 0; i < originalBytes.Length; i++)
            {
                bytesToBeEncrypted[i + saltBytes.Length] = originalBytes[i];
            }

            encryptedBytes = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(encryptedBytes);
        }
        public string Decrypt(string encryptedText, string pwd = pwd)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(pwd);

            // Hash the password with SHA256  
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] decryptedBytes = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            if (decryptedBytes != null)
            {
                // Getting the size of salt  
                int saltSize = GetSaltSize(passwordBytes);

                // Removing salt bytes, retrieving original bytes  
                byte[] originalBytes = new byte[decryptedBytes.Length - saltSize];
                for (int i = saltSize; i < decryptedBytes.Length; i++)
                {
                    originalBytes[i - saltSize] = decryptedBytes[i];
                }
                return Encoding.UTF8.GetString(originalBytes);
            }
            else
            {
                return null;
            }
        }
        private int GetSaltSize(byte[] passwordBytes)
        {
            var key = new Rfc2898DeriveBytes(passwordBytes, passwordBytes, 1000);
            byte[] ba = key.GetBytes(2);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ba.Length; i++)
            {
                sb.Append(Convert.ToInt32(ba[i]).ToString());
            }
            int saltSize = 0;
            string s = sb.ToString();
            foreach (char c in s)
            {
                int intc = Convert.ToInt32(c.ToString());
                saltSize = saltSize + intc;
            }

            return saltSize;
        }
        public byte[] GetRandomBytes(int length)
        {
            byte[] ba = new byte[length];
            RNGCryptoServiceProvider.Create().GetBytes(ba);
            return ba;
        }


        public static string GenerateApiKeyFromToken(string userToken)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[32]; // 32 bytes for a 256-bit key

                rng.GetBytes(randomBytes);

                // Convert the random bytes to a hexadecimal string
                string apiKey = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();

                return apiKey;
            }
        }
    }
}
