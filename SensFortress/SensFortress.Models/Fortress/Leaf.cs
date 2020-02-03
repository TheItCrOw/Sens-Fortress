using SensFortress.Models.BaseClasses;
using SensFortress.Security;
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
        public Leaf()
        {
            Id = Guid.NewGuid();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid BranchId { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// When 0, the item is not in the quickbar.
        /// </summary>
        [DataMember]
        public int QuickbarOrder { get; set; }

        /// <summary>
        /// Tracks how much this leaf is being used => copied e.g.
        /// </summary>
        [DataMember]
        public int InteractedCounter { get; set; }
    }
}
