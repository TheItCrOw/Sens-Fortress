using SensFortress.Models.BaseClasses;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SensFortress.Models.Fortress
{
    /// <summary>
    /// Leaf model which stores one password.
    /// </summary>
    [DataContract]
    public class Leaf : ModelBase
    {
        public Leaf(string password)
        {
            Id = Guid.NewGuid();
            Password = password;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid BranchId { get; set; }

        [DataMember]
        public string Password { get; private set; }

        [DataMember]
        public string Description { get; set; }

    }
}
