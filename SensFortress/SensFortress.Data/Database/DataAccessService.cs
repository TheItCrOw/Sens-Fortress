﻿using SensFortress.Data.Exceptions;
using SensFortress.Models.BaseClasses;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Security.AES;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
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
        #region Implement lazy
        private static readonly Lazy<DataAccessService>
            lazy =
            new Lazy<DataAccessService>
            (() => new DataAccessService());

        /// <summary>
        /// Instance of the <see cref="DataAccessService"/> class.
        /// </summary>
        public static DataAccessService Instance { get { return lazy.Value; } }
        #endregion

        /// <summary>
        /// Gets the params of the current fortress: Fullpath, Fortressname.
        /// </summary>
        public string[] CurrentFortressGeneralData { get; set; }

        /// <summary>
        /// Gets or sets the salt of the current fortress.
        /// </summary>
        public byte[] CurrentFortressSalt { get; set; }

        /// <summary>
        /// Current xmlDataCache Instance.
        /// </summary>
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
                CurrentFortressGeneralData = new string[2];
                CurrentFortressGeneralData[0] = fullPath;
                CurrentFortressGeneralData[1] = fortressName;

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
        /// Only validate a masterkey with the given fortress.
        /// </summary>
        /// <param name="fortressFullPath"></param>
        /// <param name="fortressName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateMasterkey(string password, bool useDefaultValues = true, string fortressFullPath = "", string fortressName = "")
        {
            try
            {
                if (useDefaultValues)
                {
                    fortressFullPath = CurrentFortressGeneralData[0];
                    fortressName = CurrentFortressGeneralData[1];
                }
                _xmlDatacache.ValidateMasterKey(fortressFullPath, fortressName, password);
                password = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                password = string.Empty;
                Logger.log.Error($"Couldn't validate key: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Builds a new fortress.
        /// </summary>
        /// <param name="fortess"></param>
        public void CreateNewFortress(Fortress fortess)
        {
            _xmlDatacache = new XmlDataCache(fortess.FullPath);

            // Store the example data:
            var rootBranch = new Branch { Name = "Example: Projects", ParentBranchId = Guid.Empty };
            var rootBranch2 = new Branch { Name = "Example: Projects2", ParentBranchId = Guid.Empty };
            var subBranch = new Branch { Name = "Example: Passwords", ParentBranchId = rootBranch.Id };
            var examplePw = ByteHelper.StringToByteArray("thisIsAnExamplePassword");
            var leaf = new Leaf { Name = "Password1", Description = "Here you can describe this entry.", BranchId = subBranch.Id };
            var leafPw = new LeafPassword { ForeignId = leaf.Id, Value = examplePw };
            examplePw = null;
            _xmlDatacache.AddToUnsecureMemoryDC(fortess);
            _xmlDatacache.AddToUnsecureMemoryDC(rootBranch);
            _xmlDatacache.AddToUnsecureMemoryDC(rootBranch2);
            _xmlDatacache.AddToUnsecureMemoryDC(subBranch);
            _xmlDatacache.AddToUnsecureMemoryDC(leaf);
            var leafPwAsBytes = ByteHelper.ObjectToByteArray(leafPw);
            _xmlDatacache.AddToSecureMemoryDC(leafPw.ForeignId,leafPwAsBytes);
            leafPw = null;
            // end exampel data

            _xmlDatacache.WriteFortress(fortess);
        }

        /// <summary>
        /// Adds a Model to the MemoryDC
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSensibleData"></param>
        public void AddOneToMemoryDC(ModelBase model, bool isSensibleData = false, SensibleModelBase sensibleModel = null)
        {
            if (!isSensibleData)
                _xmlDatacache.AddToUnsecureMemoryDC(model);
            else
                _xmlDatacache.AddToSecureMemoryDC(sensibleModel.ForeignId, ByteHelper.ObjectToByteArray(sensibleModel));
        }

        /// <summary>
        /// Deletes the given Modelbase from the MemoryDC
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSensibleData"></param>
        public void DeleteOneFromMemoryDC(ModelBase model, bool isSensibleData = false)
        {
            if (!isSensibleData)
                _xmlDatacache.DeleteOneFromUnsecureMemoryDC(model);
            else { } // Implement sensible part later
        }

        /// <summary>
        /// Saves the fortress with all it's changes.
        /// </summary>
        /// <param name="masterKey"></param>
        /// <param name="alsoSaveSecureDC"></param>
        public void SaveFortress(Masterkey masterKey, bool alsoSaveSecureDC) => _xmlDatacache.SaveFortress(masterKey);

        /// <summary>
        /// Get all non sensible models of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAll<T>() => _xmlDatacache.GetAllFromUnsecure<T>();

        /// <summary>
        /// Returns the sensible model with the given foreignKey from the secureMemoryDC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        public T GetSensible<T>(Guid foreignKey) => _xmlDatacache.GetSensible<T>(foreignKey);


    }
}
