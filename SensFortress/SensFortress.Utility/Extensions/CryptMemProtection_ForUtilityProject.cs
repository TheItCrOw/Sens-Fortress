using SensFortress.Utility.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SensFortress.Utility.Extensions
{
    /// <summary>
    /// This is only for the utility project. The real class is in the SensFortress.Security Project.
    /// 
    /// This is not well done - copying a class to another project for it's usability. Don't do this at home.
    /// 
    /// 
    /// </summary>
    internal class CryptMemProtection_ForUtilityProject
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
            catch (Exception ex)
            {
                ex.SetUserMessage("A security error occured while trying to encrypt sensible data in memory. Please wait while the memory is being flushed to prevent any leaks.");
                throw ex;
            }
            finally
            {
                buffer = null;
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
            finally
            {
                data = null;
            }
        }
    }
}
