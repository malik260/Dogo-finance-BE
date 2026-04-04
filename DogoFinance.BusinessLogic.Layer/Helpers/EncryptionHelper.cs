using System.Security.Cryptography;
using System.Text;

namespace DogoFinance.BusinessLogic.Layer.Helpers
{
    /// <summary>
    /// Ported from Fintrak. Provides AES encryption/decryption.
    /// Used for sensitive fields or basic obsfucation.
    /// </summary>
    public class EncryptionHelper
    {
        private const int Keysize = 128;
        private const int DerivationIterations = 10000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(plainText), passPhrase));
        }

        public static string Decrypt(string encryptedText, string passPhrase)
        {
            try
            {
                if (!IsBase64String(encryptedText)) return encryptedText;
                byte[] encryptedData = Convert.FromBase64String(encryptedText);
                int length = Keysize / 8;
                if (encryptedData.Length < length * 2) return encryptedText;

                byte[] decryptedBytes = Decrypt(encryptedData, passPhrase);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch { return encryptedText; }
        }

        public static byte[] Encrypt(byte[] plainData, string passPhrase)
        {
            var salt = Generate128BitsOfRandomEntropy();
            var iv = Generate128BitsOfRandomEntropy();
            using var password = new Rfc2898DeriveBytes(passPhrase, salt, DerivationIterations);
            var key = password.GetBytes(Keysize / 8);
            using var symmetricKey = new AesManaged(); // AesManaged is more modern than RijndaelManaged in .NET 6+
            symmetricKey.BlockSize = 128;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;
            using var encryptor = symmetricKey.CreateEncryptor(key, iv);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(plainData, 0, plainData.Length);
                cs.FlushFinalBlock();
            }
            return salt.Concat(iv).Concat(ms.ToArray()).ToArray();
        }

        public static byte[] Decrypt(byte[] encryptedData, string passPhrase)
        {
            var salt = encryptedData.Take(Keysize / 8).ToArray();
            var iv = encryptedData.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            var cipher = encryptedData.Skip((Keysize / 8) * 2).ToArray();

            using var password = new Rfc2898DeriveBytes(passPhrase, salt, DerivationIterations);
            var key = password.GetBytes(Keysize / 8);
            using var symmetricKey = new AesManaged();
            symmetricKey.BlockSize = 128;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;
            using var decryptor = symmetricKey.CreateDecryptor(key, iv);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            var plain = new byte[cipher.Length];
            var count = cs.Read(plain, 0, plain.Length);
            return plain.Take(count).ToArray();
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var bytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return bytes;
        }

        private static bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return false;
            Span<byte> buffer = new byte[base64.Length];
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}
