using SensFortress.Models.BaseClasses;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SensFortress.Models.Fortress
{
    /// <summary>
    /// Branch model for visualizing data in the TreeView
    /// </summary>
    [DataContract]
    public class Branch : ModelBase
    {
        public Branch()
        {
            Id = Guid.NewGuid();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid ParentBranchId { get; set; }
    }
}
