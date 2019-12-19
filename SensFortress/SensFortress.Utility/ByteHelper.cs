﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    }
}
