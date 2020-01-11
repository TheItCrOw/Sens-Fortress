using SensFortress.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SensFortress.Models.BaseClasses
{
    /// <summary>
    /// Class that wraps sensibleData such as <see cref="LeafPassword"/> to store them on disk and perform serializble operations.
    /// </summary>
    public class ByteModel
    {
        public ByteModel(Guid foreignKey, byte[] encryptedModel)
        {
            ForeignKey = foreignKey;
            EncryptedValue = encryptedModel;
        }
        public Guid ForeignKey { get; set; }

        /// <summary>        
        /// Contains the encrypted actual model or value that is being wrapped.
        /// </summary>
        public byte[] EncryptedValue { get; set; }
    }
}
