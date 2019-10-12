using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Interfaces
{
    public class Fortress
    {
        /// <summary>
        /// Random salt that is needed to decrypt the database.
        /// </summary>
        public byte[] Salt { get; set; }

        /// <summary>
        /// Path in which the new fortress will be build.
        /// </summary>
        public string FullPath { get; set; }

        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }

    }
}
