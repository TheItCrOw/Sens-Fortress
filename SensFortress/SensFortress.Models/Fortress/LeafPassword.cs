using SensFortress.Models.BaseClasses;
using SensFortress.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Models.Fortress
{
    public class LeafPassword : ModelBase
    {
        public LeafPassword()
        {
            Id = Guid.NewGuid();
        }

        public Guid LeafId { get; set; }

        public byte[] Value { get; set; }
    }
}
