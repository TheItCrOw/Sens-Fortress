using SensFortress.Data.Exceptions;
using SensFortress.Models.BaseClasses;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Security.AES;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Text;
using System.Windows;

namespace SensFortress.Data.Database
{
    /// <summary>
    /// Interface for communicating with the datacache.
    /// </summary>
    public sealed class DataAccessService
    {
        private static readonly Lazy<DataAccessService>
            lazy =
            new Lazy<DataAccessService>
            (() => new DataAccessService());

        /// <summary>
        /// Instance of the <see cref="DataAccessService"/> class.
        /// </summary>
        public static DataAccessService Instance { get { return lazy.Value; } }

        //private XmlDataCache _xmlDataCache = new XmlDataCache();

        /// <summary>
        /// Sets the path for the current database that is being used.
        /// </summary>
        /// <param name="path"></param>
        //public void InitializeCurrentDatacache(string path) => _xmlDataCache.InitializeXmlDatacache(path);

        //public void Save(ModelBase model) => _xmlDataCache.StoreOne<Fortress>(model);

       

       

    }
}
