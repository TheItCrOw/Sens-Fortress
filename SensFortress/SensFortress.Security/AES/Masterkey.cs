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
            var encryptedKey = Encoding.UTF8.GetString(keyBytes);
            Value = GetSecureString(encryptedKey);
            encryptedKey = null;
        }

        public SecureString Value { get; }

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
