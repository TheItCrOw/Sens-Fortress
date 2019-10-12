using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a fortress operation failed.
    /// </summary>
    [Serializable]
    public class FortressException : Exception
    {
        public FortressException()
        { }

        public FortressException(string message)
            : base(message)
        { }

        public FortressException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
