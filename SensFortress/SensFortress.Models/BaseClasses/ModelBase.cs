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
        public override string ToString()
        {
            var properties = this.GetType().GetProperties();
            var sb = new StringBuilder();
            foreach(var prop in properties)
            {
                sb.Append($"{prop.Name} | {prop.GetValue(prop)}");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
