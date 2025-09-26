using System;
using System.Security.Cryptography;
using System.Text;

namespace Simjob.Framework.Domain.Util
{
    public class HashHelper
    {
        public static string HashGeneration(string password)
        {
            const int workfactor = 10;

            string salt = BCrypt.Net.BCrypt.GenerateSalt(workfactor);
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hash;
        }

        public static bool PasswordCompare(string hash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public static Guid GenerateKeyFromToken(string userToken)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] tokenBytes = Encoding.UTF8.GetBytes(userToken);
                byte[] hashBytes = sha256.ComputeHash(tokenBytes);
                return new Guid(hashBytes);
            }
        }

    }
}
