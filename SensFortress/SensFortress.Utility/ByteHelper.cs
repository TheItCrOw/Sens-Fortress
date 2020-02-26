using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace SensFortress.Utility
{
    public static class ByteHelper
    {
        /// <summary>
        /// Takes in two byte arrays and appends them.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static byte[] AppendTwoByteArrays(byte[] one, byte[] two)
        {
            byte[] combined = one.Concat(two).ToArray();
            return combined;
        }

        /// <summary>
        /// Takes a string and returns it as a byte array.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string str)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }

        /// <summary>
        /// Takes a byte array and returns a string.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] arr)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(arr);
        }

        /// <summary>
        /// Reads a stream and returns an array of bytes.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadBytesOfStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Returns the hash of the given path
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string ReadHash(string fullPath)
        {
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Couldn't hash the path: {fullPath}");

            // Write hash out of file bytes.
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var buffer = md5.ComputeHash(File.ReadAllBytes(fullPath));
                var sb = new StringBuilder();
                for (var i = 0; i < buffer.Length; i++)
                {
                    sb.Append(buffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        ///  Convert an object to a byte array
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                bytes = ms.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Convert a byte array to an Object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public static object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        /// <summary>
        /// Compares 2 byte arrays and returns false if they are different.
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

    }
}
