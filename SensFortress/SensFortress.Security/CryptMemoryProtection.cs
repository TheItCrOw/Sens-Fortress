using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using SensFortress.Utility.Log;
using System.Reflection;
using SensFortress.Utility.Exceptions;

namespace SensFortress.Security
{
    /// <summary>
    /// Class that encrypts/decrypts bytes in the memory for safety of sensible data.
    /// </summary>
    public static class CryptMemoryProtection
    {

        private static byte[] _aditionalEntropy = { 9, 8, 7, 6, 5 };

        public static byte[] DecryptInMemoryData(object password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encrypt a byte array in memory.
        /// </summary>
        /// <param name="buffer"></param>
        public static byte[] EncryptInMemoryData(byte[] buffer)
        {
            try
            {
                // Encrypt the data using the DataProtectionScope.CurrentUser. 
                // Only this user can decrypt it.
                return ProtectedData.Protect(buffer, _aditionalEntropy, DataProtectionScope.CurrentUser);
            }
            catch(Exception ex)
            {
                ex.SetUserMessage("A security error occured while trying to encrypt sensible data in memory. Please wait while the memory is being flushed to prevent any leaks.");
                throw ex;
            }
        }

        /// <summary>
        /// Decrypt a byte array in memory.
        /// </summary>
        /// <param name="data"></param>
        public static byte[] DecryptInMemoryData(byte[] data)
        {
            try
            {
                //Decrypt the data using DataProtectionScope.CurrentUser.
                return ProtectedData.Unprotect(data, _aditionalEntropy, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException ex)
            {
                ex.SetUserMessage("A security error occured while trying to decrypt sensible data in memory. Please wait while the memory is being flushed to prevent any leaks.");
                throw ex;
            }
        }
    }
}
