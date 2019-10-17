using SensFortress.Data.Exceptions;
using SensFortress.Models.BaseClasses;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

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
            try
            {
                File.WriteAllBytes(path + "\\salt" + TermHelper.GetTextFileEnding(), salt);
            }
            catch (Exception ex)
            {
                Logger.log.Error($"During storing salt: {ex}");
                throw new XmlDataCacheException($"Error while trying to store salt in the {TermHelper.GetDatabaseTerm()} ", ex);
            }
        }

        /// <summary>
        /// Builds models out of byte array.
        /// </summary>
        /// <param name="arr"></param>
        public void BuildModelsOutOfBytes (byte[] arr)
        {
            XmlDocument doc = new XmlDocument();
            MemoryStream ms = new MemoryStream(arr);

            // doc holds the current xml File
            doc.Load(ms);

            ms.Close();
        }

        internal void StoreOne<T>(string datacacheRootPath, ModelBase model) where T : ModelBase
        {
            try
            {
                Logger.log.Info($"Starting to write instance of {typeof(T).Name}: {model.Id} to {datacacheRootPath}...");
                var path = datacacheRootPath + "\\" + typeof(T).Name;
                DirectoryHelper.CreateDirecotry(path);
                var fullName = path + "\\" + model.Id + ".xml";

                //So that the xmlWriter makes breaks between lines
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.NewLineOnAttributes = true;
                xmlWriterSettings.Indent = true;

                using (XmlWriter writer = XmlWriter.Create(fullName, xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(typeof(T).Name);

                    foreach(var property in typeof(T).GetProperties())
                    {
                        writer.WriteElementString(property.Name, property.GetValue(model, null).ToString());
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                Logger.log.Info($"Writing successfull!");
            }
            catch (Exception ex)
            {
                Logger.log.Error($"During storing one: {ex}");
                throw new XmlDataCacheException($"Error while trying to store {typeof(T).Name} in the {TermHelper.GetDatabaseTerm()} ", ex);
            }
        }

    }
}
