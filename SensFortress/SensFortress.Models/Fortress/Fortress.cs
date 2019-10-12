using SensFortress.Security.AES;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace SensFortress.Models.Fortress
{
    /// <summary>
    /// Model that holds information of a fortress.
    /// </summary>
    public class Fortress
    {
        public Fortress(byte[] salt, byte[] key, string fullPath, string name, string lastName, string userName, string eMail)
        {
            Salt = salt;
            MasterKey = new Masterkey(key);
            FullPath = fullPath;
            Name = name;
            LastName = lastName;
            EMail = eMail;
        }
        /// <summary>
        /// Random salt that is needed to decrypt the database.
        /// </summary>
        public byte[] Salt { get; }

        /// <summary>
        /// SecureString of the masterkey for passing in memory.
        /// </summary>
        public Masterkey MasterKey { get; } 

        /// <summary>
        /// Path in which the new fortress will be build.
        /// </summary>
        public string FullPath { get; }

        public string Name { get; }
        public string LastName { get; }
        public string UserName { get; }
        public string EMail { get; }

    }
}
