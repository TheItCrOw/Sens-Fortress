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

        private XmlDataCache _xmlDatacache;

        /// <summary>
        /// Builds a fortress.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="fortressName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool BuildFortress(string fullPath, string fortressName, string password)
        {
            try
            {
                _xmlDatacache = new XmlDataCache(fullPath);
                _xmlDatacache.BuildFortress(fullPath, fortressName, password);
                password = string.Empty;

                return true;
            }
            catch (Exception ex)
            {
                _xmlDatacache = null;
                password = string.Empty;
                Logger.log.Error($"Couldn't build fortress: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Builds a new fortress.
        /// </summary>
        /// <param name="fortess"></param>
        public void CreateNewFortress(Fortress fortess)
        {
            try
            {
                _xmlDatacache = new XmlDataCache(fortess.FullPath);
                _xmlDatacache.CreateNewFortress(fortess);
            }
            catch (Exception ex)
            {
                _xmlDatacache = null;
                Logger.log.Error($"Couldn't build fortress: {ex}");
            }
        }

        /// <summary>
        /// Adds a ModelBase to the MemoryDC
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSensibleData"></param>
        public void AddOneToMemoryDC(ModelBase model, bool isSensibleData = false)
        {
            if (!isSensibleData)
                _xmlDatacache.AddToUnsecureMemoryDC(model);
            else { }
            //Implement later
        }

        /// <summary>
        /// Deletes the given Modelbase from the MemoryDC
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSensibleData"></param>
        public void DeleteOneFromMemoryDC(ModelBase model, bool isSensibleData = false)
        {
            try
            {
                if (!isSensibleData)
                    _xmlDatacache.DeleteOneFromUnsecureMemoryDC(model);
                else { } // Implement sensible part later
            }
            catch (Exception ex)
            {
                Communication.InformUserAboutError(ex);
            }
        }

        /// <summary>
        /// Get all models of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAll<T>(bool isSensibleData = false)
        {
            if (!isSensibleData)
                return _xmlDatacache.GetAllFromUnsecure<T>();
            else // Implement later
                return _xmlDatacache.GetAllFromUnsecure<T>();
        }


    }
}
