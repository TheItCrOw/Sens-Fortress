using SensFortress.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Models.BaseClasses
{
    /// <summary>
    /// ModelBase for sensible data that has to be serialized as bytes.
    /// Main difference: DataContract and Seriazible
    /// </summary>
    [Serializable]
    public class SensibleModelBase : Interfaces.ISerializable, IDeletable
    {
        public Guid Id { get; set; }

        public Guid ForeignId { get; set; }
    }
}
