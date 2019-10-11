using System;
using System.Collections.Generic;
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

    }
}
