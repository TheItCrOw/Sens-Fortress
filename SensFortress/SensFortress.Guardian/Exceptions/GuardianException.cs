using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Guardian.Exceptions
{
    [Serializable]
    public class GuardianException : Exception
    {
        public GuardianException()
        { }

        public GuardianException(string message)
            : base(message)
        { }

        public GuardianException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
