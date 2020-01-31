using SensFortress.Models.BaseClasses;
using SensFortress.Security.AES;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace SensFortress.Models.Fortress
{
    /// <summary>
    /// Model that holds information of a fortress.
    /// </summary>
    [DataContract]
    public class Fortress : ModelBase
    {
        public Fortress(byte[] salt, byte[] key, string fullPath, string name, string lastName, string userName, string eMail)
        {
            Salt = salt;
            MasterKey = new Masterkey(key);
            FullPath = fullPath;
            Name = name;
            LastName = lastName;
            UserName = userName;
            EMail = eMail;
            base.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Random salt that is needed to decrypt the database.
        /// </summary>
        [DataMember]
        public byte[] Salt { get; set; }

        /// <summary>
        /// Encrypted Masterkey for passing in memory.
        /// </summary>
        public Masterkey MasterKey { get; set; } 

        /// <summary>
        /// Path in which the new fortress will be build.
        /// </summary>
        public string FullPath { get; set; }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string EMail { get; set; }

    }
}
