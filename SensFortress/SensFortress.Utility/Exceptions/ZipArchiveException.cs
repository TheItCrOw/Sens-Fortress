using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Utility.Exceptions
{
    /// <summary>
    /// The exception that is thrown when zipping of an archive fails.
    /// </summary>
    [Serializable]
    class ZipArchiveException : Exception
    {
        public ZipArchiveException()
        { }

        public ZipArchiveException(string message)
            : base(message)
        { }

        public ZipArchiveException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
