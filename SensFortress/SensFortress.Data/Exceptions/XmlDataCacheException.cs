using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an operation in the Xml Datacache failed.
    /// </summary>
    [Serializable]
    public class XmlDataCacheException : Exception
    {
        public XmlDataCacheException()
        { }

        public XmlDataCacheException(string message)
            : base(message)
        { }

        public XmlDataCacheException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
