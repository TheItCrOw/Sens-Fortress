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
    /// Class that encrypts/decrypts byte in the memory for safety of sensible data.
    /// </summary>
    public static class CryptMemoryProtection
    {

        private static byte[] _aditionalEntropy = { 9, 8, 7, 6, 5 };

        /// <summary>
        /// Encrypt a byte array in memory.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
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
                ex.Source = $"{ex.Source} called by {MethodBase.GetCurrentMethod()}";
                ex.SetUserMessage("A security error occured while trying to encrypt sensible data in memory. Please wait while the memory is being flushed to prevent any leaks.");
                throw ex;
            }
        }

        /// <summary>
        /// Decrypt a byte array in memory.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] DecryptInMemoryData(byte[] data)
        {
            try
            {
                //Decrypt the data using DataProtectionScope.CurrentUser.
                return ProtectedData.Unprotect(data, _aditionalEntropy, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException ex)
            {
                ex.Source = $"{ex.Source} called by {MethodBase.GetCurrentMethod()}";
                ex.SetUserMessage("A security error occured while trying to decrypt sensible data in memory. Please wait while the memory is being flushed to prevent any leaks.");
                throw ex;
            }
        }
    }
}
