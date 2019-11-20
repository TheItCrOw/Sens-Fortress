using SensFortress.Data.Exceptions;
using SensFortress.Models.BaseClasses;
using SensFortress.Models.Interfaces;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SensFortress.Data.Database
{
    /// <summary>
    /// Handles writing and modifying operations in the datacache.
    /// </summary>
    internal class XmlDataCache
    {
        private string _databasePath;
        private bool _isInitialized;

        /// <summary>
        /// Set the location of the current datacache. If it's not initialzed, the datacache will break.
        /// </summary>
        /// <param name="path"></param>
        public void InitializeXmlDatacache(string path)
        {
            _databasePath = path;
            _isInitialized = true;
        }
        /// <summary>
        /// Do NOT use this unless a salt is stored in a single file.
        /// </summary>
        internal void StoreSalt(string path, byte[] salt)
        {
            if (!_isInitialized)
                throw new XmlDataCacheException("The Datacache has not been initialized.");

            try
            {
                File.WriteAllBytes(Path.Combine(path, $"salt{TermHelper.GetTextFileEnding()}"), salt);
            }
            catch (Exception ex)
            {
                Logger.log.Error($"During storing salt: {ex}");
                throw new XmlDataCacheException($"Error while trying to store salt in the {TermHelper.GetDatabaseTerm()} ", ex);
            }
        }

        /// <summary>
        /// Builds models out of one byte array.
        /// </summary>
        /// <param name="arr"></param>
        public void BuildModelsOutOfBytes(byte[] arr)
        {
            if (arr == null)
                throw new XmlDataCacheException("Byte array was null.");

            XmlDocument doc = new XmlDocument();
            using (var ms = new MemoryStream(arr))
            {
                // doc holds the current xml File
                doc.Load(ms);
            }

            //ModelFactory.Instance.AddToFactoryQueue<XmlDocument>(doc);
        }

        /// <summary>
        /// Builds models out of a list of byte arrays.
        /// </summary>
        /// <param name="arrList"></param>
        public void BuildModelsOutOfBytes(List<byte[]> arrList)
        {
            var docs = new List<XmlDocument>();
            foreach(var byteArray in arrList)
            {
                using (var ms = new MemoryStream(byteArray))
                {
                    var doc = new XmlDocument();
                    doc.Load(ms);
                    docs.Add(doc);
                }
            }
        }

        internal void StoreOne<T>(ModelBase model) where T : Models.Interfaces.ISerializable
        {
            if (!_isInitialized)
                throw new XmlDataCacheException("The Datacache has not been initialized.");

            var ds = new DataContractSerializer(typeof(T));
            var obj = CastModelBase<T>(model);
            var settings = new XmlWriterSettings { Indent = true };
            var currentSaveLocation = Path.Combine(_databasePath, TermHelper.GetDatabaseTerm(), typeof(T).Name);

            // Always check if a directory exists. If not, create it.
            if (!Directory.Exists(currentSaveLocation))
                DirectoryHelper.CreateDirectory(currentSaveLocation);

            using (var sww = new StringWriter())
            {
                using (var w = XmlWriter.Create(Path.Combine(currentSaveLocation, $"{model.Id}.xml"), settings))
                {
                    ds.WriteObject(w, obj);
                }
            }
        }

        /// <summary>
        /// Casts the ModelBase into T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        private T CastModelBase<T>(ModelBase model)
        {
            if (model is T)
                return (T)Convert.ChangeType(model, typeof(T));
            else
                throw new InvalidCastException($"Cannot cast type {model.GetType()} to {typeof(T)}");
        }

    }
}
