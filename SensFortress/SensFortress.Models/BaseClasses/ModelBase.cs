using SensFortress.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SensFortress.Models.BaseClasses
{
    [DataContract]
    public class ModelBase : Interfaces.ISerializable, IDeletable
    {
        [DataMember]
        public Guid Id { get; set; }
    }
}
