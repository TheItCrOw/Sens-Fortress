using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SensFortress.Security.AES
{
    /// <summary>
    /// Wrapper for handling SecureStrings
    /// also see here: https://codereview.stackexchange.com/questions/107860/converting-a-securestring-to-a-byte-array 
    /// </summary>
    public sealed class SecureStringWrapper : IDisposable
    {
        private readonly Encoding encoding;
        private readonly SecureString secureString;
        private byte[] _bytes = null;

        public SecureStringWrapper(SecureString secureString)
              : this(secureString, Encoding.UTF8)
        { }

        public SecureStringWrapper(SecureString secureString, Encoding encoding)
        {
            if (secureString == null)
            {
                throw new ArgumentNullException(nameof(secureString));
            }

            this.encoding = encoding ?? Encoding.UTF8;
            this.secureString = secureString;
        }

        /// <summary>
        /// Returns the secure string as a byte array.
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] ToByteArray()
        {

            int maxLength = encoding.GetMaxByteCount(secureString.Length);

            IntPtr bytes = IntPtr.Zero;
            IntPtr str = IntPtr.Zero;

            try
            {
                bytes = Marshal.AllocHGlobal(maxLength);
                str = Marshal.SecureStringToBSTR(secureString);

                char* chars = (char*)str.ToPointer();
                byte* bptr = (byte*)bytes.ToPointer();
                int len = encoding.GetBytes(chars, secureString.Length, bptr, maxLength);

                _bytes = new byte[len];
                for (int i = 0; i < len; ++i)
                {
                    _bytes[i] = *bptr;
                    bptr++;
                }

                return _bytes;
            }
            finally
            {
                if (bytes != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(bytes);
                }
                if (str != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(str);
                }
            }
        }

        private bool _disposed = false;

        public void Dispose()
        {
            if (!_disposed)
            {
                Destroy();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        private void Destroy()
        {
            if (_bytes == null) { return; }

            for (int i = 0; i < _bytes.Length; i++)
            {
                _bytes[i] = 0;
            }
            _bytes = null;
        }

        ~SecureStringWrapper()
        {
            Dispose();
        }
    }
}
