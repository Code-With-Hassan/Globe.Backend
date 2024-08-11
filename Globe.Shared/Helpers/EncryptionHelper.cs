using System.Security.Cryptography;

namespace Globe.Shared.Helpers
{
    /// <summary>
    /// The encryption helper.
    /// </summary>
    public static class EncryptionHelper
    {
        private static readonly string _key = "D/0g+LeqgBuPUAzb/Y0L+A4mmFblmX6ZyjTKL7ER0nk=";
        private static readonly string _iV = "gLXoqXGmtY3glY+XMhuwJg==";

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <returns>A string.</returns>
        public static string GetKey() => _key;

        /// <summary>
        /// Encrypt the PlainText
        /// </summary>
        /// <param name="plainText">The PlainText.</param>
        /// <returns>Encrypted String.</returns>
        public static string EncryptString(string plainText)
        {
            if (plainText.Length == 0) throw new Exception("Plain text can't be Empty");

            using Aes aes = Aes.Create();

            ICryptoTransform encryptor = aes.CreateEncryptor(Convert.FromBase64String(_key), Convert.FromBase64String(_iV));

            string encryptedString;
            using (MemoryStream memoryStream = new())
            {
                using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }
                }
                encryptedString = Convert.ToBase64String(memoryStream.ToArray());
            }
            return encryptedString;
        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <returns>A string.</returns>
        public static string DecryptString(string cipherText)
        {
            if (cipherText.Length == 0) throw new Exception("Cipher text can't be Empty");

            byte[] buffer = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();

            ICryptoTransform decryptor = aes.CreateDecryptor(Convert.FromBase64String(_key), Convert.FromBase64String(_iV));

            string decryptedString;
            using (MemoryStream memoryStream = new(buffer))
            {
                using (CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new(cryptoStream))
                    {
                        decryptedString = streamReader.ReadToEnd();
                    }
                }
            }
            return decryptedString;
        }
    }
}
