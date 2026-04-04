using System.Security.Cryptography;
using System.Text;

namespace DogoFinance.BusinessLogic.Layer.Helpers
{
    public static class HashHelper
    {
        public static (string hash, string salt) CreateHash(string password)
        {
            var saltBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            using var sha256 = SHA256.Create();
            var combinedPassword = password + salt;
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
            var hash = Convert.ToBase64String(hashBytes);

            return (hash, salt);
        }

        public static bool VerifyHash(string password, string hash, string salt)
        {
            using var sha256 = SHA256.Create();
            var combinedPassword = password + salt;
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
            var computedHash = Convert.ToBase64String(hashBytes);

            return hash == computedHash;
        }
    }
}
