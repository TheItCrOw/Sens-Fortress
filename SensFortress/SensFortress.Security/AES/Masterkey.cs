using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace SensFortress.Security.AES
{
    /// <summary>
    /// Class that holds the master key.
    /// </summary>
    public class Masterkey
    {
        public Masterkey(byte[] keyBytes)
        {
            var encryptedKey = CryptMemoryProtection.EncryptInMemoryData(keyBytes);
            Value = encryptedKey;
            encryptedKey = null;
        }

        /// <summary>
        /// Gets the encrypted byte value of the masterkey
        /// </summary>
        public byte[] Value { get; }

        /// <summary>
        /// Creates a SecureString out of a string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private protected SecureString GetSecureString(string text)
        {
            SecureString strSecure = new SecureString();
            foreach (char c in text)
            {
                strSecure.AppendChar(c);
            }
            return strSecure;
        }

    }
}
