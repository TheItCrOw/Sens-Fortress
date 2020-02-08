﻿using SensFortress.Models.BaseClasses;
using SensFortress.Security;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SensFortress.Models.Fortress
{
    [Serializable]
    public class LeafPassword : SensibleModelBase
    {
        public LeafPassword()
        {
            Id = Guid.NewGuid();
        }
        public byte[] Value { get; set; }

        /// <summary>
        /// This only has value if the password has been edited!
        /// </summary>
        public byte[] EncryptedValue { get; set; }

    }
}
