using SensFortress.Security.AES;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace SensFortress.Security
{
    /*=========================== Functionality of the encryption ===========================
     * 
     * -> User inputs a masterkey
     * -> The masterkey is being hashed and also made random with a salt variable
     * -> The hash is never saved anywhere
     * -> The salt is sotred along the cipher text. It doesn't matter if the attacker knows this variable.
     * -> The hashed key is the key for the AES-Encryption
     * -> A random IV Vector is generated that the AES also takes in.
     * -> The IV Vector is saved with the cipher text. It doesn't matter if the attacker knows this variable.
     * -> A minimum of 1000 iterations is at least requiered.
     * -> The data is then encrypted and stored as a *.sfdb (Sen's Fortress data base) file
     * -> To decrypt it, the IV Vector, salt and masterkey (hashed user input) is needed.
     * 
     * Reference: https://stackoverflow.com/questions/13901529/symmetric-encryption-aes-is-saving-the-iv-and-salt-alongside-the-encrypted-da
     * */

    /// <summary>
    /// Encrypt data using the AES-Algorithm
    /// </summary>
    public class AesAlgorithm
    {
        public byte[] onlyForTestingVariable;

        /// <summary>
        /// Encrypts the data with the given password and salt with the AES algorithm.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data, SecureString password, byte[] salt)
        {
            byte[] encryptedData = null;
            var _aesHelper = new AesHelper();

            using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider())
            {
                provider.GenerateIV();
                // Create a byte array out of the secure string
                using (SecureStringWrapper wrapper = new SecureStringWrapper(password))
                {
                    byte[] passwordBytes = wrapper.ToByteArray();
                    provider.Key = passwordBytes;
                    provider.Mode = CipherMode.CBC;
                    provider.Padding = PaddingMode.PKCS7;

                    using (MemoryStream memStream = new MemoryStream())
                    {
                        memStream.Write(provider.IV, 0, 16);
                        using (ICryptoTransform encryptor = provider.CreateEncryptor(provider.Key, provider.IV))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                        }
                        encryptedData = memStream.ToArray();
                    }
                }

            }
            return encryptedData;

        }

        /// <summary>
        /// Decrypts data with the password and salt that its been encrypted with.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] data, SecureString password, byte[] salt)
        {
            byte[] decryptedData = new byte[data.Length];
            var aesHelper = new AesHelper();

            using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider())
            {
                // Create a byte array out of the secure string
                using (SecureStringWrapper wrapper = new SecureStringWrapper(password))
                {
                    byte[] passwordBytes = wrapper.ToByteArray();
                    provider.Key = passwordBytes;
                    provider.Mode = CipherMode.CBC;
                    provider.Padding = PaddingMode.PKCS7;
                    using (MemoryStream memStream = new MemoryStream(data))
                    {
                        byte[] iv = new byte[16];
                        memStream.Read(iv, 0, 16);
                        using (ICryptoTransform decryptor = provider.CreateDecryptor(provider.Key, iv))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                            {
                                cryptoStream.Read(decryptedData, 0, decryptedData.Length);
                            }
                        }
                    }
                }

            }
            return decryptedData;
        }

    }
}
