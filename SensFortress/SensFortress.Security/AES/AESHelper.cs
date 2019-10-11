using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SensFortress.Security.AES
{
    /// <summary>
    /// A helper class for an AES encryption
    /// </summary>
    public static class AESHelper
    {
        private static readonly int SaltByteLength = 32;
        private const ushort ITERATIONS = 5000;

        /// <summary>
        /// Creates a hashed key out of the user input
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keySize"> Typically it's 512 long. </param>
        /// <returns></returns>
        internal static byte[] CreateKey(string password, int keySize)
        {
            Logger.log.Info("Trying to create a key out of the user input...");
            DeriveBytes derivedKey = new Rfc2898DeriveBytes(password, GenerateSalt(), ITERATIONS);
            Logger.log.Info("Derived Key has been created!");
            return derivedKey.GetBytes(keySize >> 3);
        }

        private static byte[] GenerateSalt()
        {            
            RNGCryptoServiceProvider rncCsp = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SaltByteLength];
            rncCsp.GetBytes(salt);
            Logger.log.Info("Generated random salt.");

            return salt;
        }


    }
}
