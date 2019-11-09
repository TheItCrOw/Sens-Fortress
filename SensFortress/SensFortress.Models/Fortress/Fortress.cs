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
        public Fortress(byte[] salt, byte[] key, string fullPath, string name, string lastName, string userName, string eMail, Guid id)
        {
            Salt = salt;
            MasterKey = new Masterkey(key);
            FullPath = fullPath;
            Name = name;
            LastName = lastName;
            UserName = userName;
            EMail = eMail;
            Id = id;
        }

        public override void Create()
        {

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
