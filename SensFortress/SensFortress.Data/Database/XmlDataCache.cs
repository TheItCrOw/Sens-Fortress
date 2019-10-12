using SensFortress.Models.BaseClasses;
using SensFortress.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SensFortress.Data.Database
{
    /// <summary>
    /// Handles writing and modifying operations in the datacache.
    /// </summary>
    internal class XmlDataCache
    {

        /// <summary>
        /// Do NOT use this unless a salt is stored in a single file.
        /// </summary>
        internal void StoreSalt(string path, byte[] salt)
        {
            File.WriteAllBytes(path + "\\salt" + TermHelper.GetTextFileEnding(), salt);
        }

        internal void StoreOne<T>() where T : ModelBase
        {

        }

    }
}
