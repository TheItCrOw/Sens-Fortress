using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using SensFortress.Security.AES;
using System.Xml;
using SensFortress.Utility;

namespace SensFortress.Security.Testing
{
    public static class AesTests
    {

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var testKey = "testKey123";
                var testVI = "testVI123";

                var testByteKey = Encoding.ASCII.GetBytes(testKey);
                var testByteVI = Encoding.ASCII.GetBytes(testVI);

                var test = System.Text.Encoding.UTF8.GetString(testByteKey);
                var test2 = System.Text.Encoding.UTF8.GetString(testByteVI);

                var test3 = BitConverter.ToInt32(Key);
                var test4 = BitConverter.ToInt32(IV);

                var test5 = AESHelper.CreateKey("testKey", 512);

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        public static void TestFileEncryption()
        {
            byte[] encryptedFile;
            //using (FileStream fs = new FileStream("C:\\Users\\Nutzer\\Desktop\\4484d5bb-658f-47f5-a1f0-ab738c120252.xml", FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            //{
            //    XmlDocument testDoc = new XmlDocument();
            //    testDoc.Load(fs);
            //    byte[] bytes = Encoding.Default.GetBytes(testDoc.OuterXml);

            //    var test = CustomAES.Encrypt(bytes, "password");
            //    File.WriteAllBytes("C:\\Users\\Nutzer\\Desktop\\encryptedTestFile.sfdb", test);
            //}

            var file = File.ReadAllBytes("C:\\Users\\Nutzer\\Desktop\\TestZip.sfzf");
            var test = CustomAES.Encrypt(file, "password");
            var testSalt = "testSalt";
            var testSaltBytes = Encoding.ASCII.GetBytes(testSalt);
            var allBytes = ByteHelper.AppendTwoByteArrays(test,testSaltBytes);


            File.WriteAllBytes("C:\\Users\\Nutzer\\Desktop\\encryptedTestFile.sfdb", allBytes);
        }

        public static void TestFileDecryption()
        {

            var file = File.ReadAllBytes("C:\\Users\\Nutzer\\Desktop\\encryptedTestFile.sfdb");
            var decryptedBytes = CustomAES.Decrypt(file, "password");
            var decryptedString = System.Text.Encoding.UTF8.GetString(decryptedBytes);
            File.WriteAllBytes("C:\\Users\\Nutzer\\Desktop\\decryptedTestFile.zip", decryptedBytes);
        }
    }
}
