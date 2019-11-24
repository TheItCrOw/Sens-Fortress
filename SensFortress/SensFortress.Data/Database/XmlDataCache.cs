using SensFortress.Data.Exceptions;
using SensFortress.Models.BaseClasses;
using SensFortress.Models.Fortress;
using SensFortress.Models.Interfaces;
using SensFortress.Security;
using SensFortress.Security.AES;
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
        /// Set the path of the current <see cref="TermHelper.GetDatabaseTerm()"/>.
        /// </summary>
        /// <param name="path"></param>
        internal XmlDataCache(string path)
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
        internal void BuildModelsOutOfBytes(byte[] arr)
        {
            if (arr == null)
                throw new XmlDataCacheException("Byte array was null.");

            XmlDocument doc = new XmlDocument();
            using (var ms = new MemoryStream(arr))
            {
                // doc holds the current xml File
                doc.Load(ms);
            }
        }

        /// <summary>
        /// Builds models out of a list of byte arrays.
        /// </summary>
        /// <param name="arrList"></param>
        internal void BuildModelsOutOfBytes(List<byte[]> arrList)
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

        /// <summary>
        /// Creates a new <see cref="Fortress"/> with a <see cref="MasterKey"/> and saves it encrypted.
        /// </summary>
        internal void CreateNewFortress(Fortress fortress)
        {
            try
            {
                Logger.log.Info("Starting to build a new fortress...");
                var databasePath = Path.Combine(fortress.FullPath, TermHelper.GetDatabaseTerm());

                // =========================================================== Create the root directory

                DirectoryHelper.CreateDirectory(fortress.FullPath);
                Logger.log.Debug($"Created outer walls {fortress.FullPath}.");

                // =========================================================== Create the sub directory for the database

                DirectoryHelper.CreateDirectory(databasePath);
                Logger.log.Debug($"Created the {TermHelper.GetDatabaseTerm()}");

                // =========================================================== Create the file which holds the salt to unlock the database

                StoreSalt(fortress.FullPath, fortress.Salt);
                Logger.log.Debug("Stored salt");

                // =========================================================== Store the user Input in the database

                StoreOne<Fortress>(fortress);
                Logger.log.Debug("Stored fortress information.");

                // =========================================================== Zip only the database 

                ZipHelper.ZipSavedArchives(databasePath, $"{databasePath}{TermHelper.GetZippedFileEnding()}");
                Directory.Delete(databasePath, true);
                Logger.log.Debug($"{TermHelper.GetDatabaseTerm()} has been zipped.");

                // =========================================================== Encrypt the database

                var aesAlg = new AesAlgorithm();
                // Read all bytes from the database directory
                var data = File.ReadAllBytes($"{databasePath}{TermHelper.GetZippedFileEnding()}");
                // Encrypt it
                var encryptedData = aesAlg.Encrypt(data, fortress.MasterKey.Value, fortress.Salt);
                // Write the encrypted file
                File.WriteAllBytes($"{databasePath}{TermHelper.GetDatabaseEnding()}", encryptedData);
                // Delete the zip
                File.Delete($"{databasePath}{TermHelper.GetZippedFileEnding()}");
                Logger.log.Debug($"Encrypted {TermHelper.GetDatabaseTerm()}");

                // =========================================================== Zip the whole fortress

                ZipHelper.ZipSavedArchives(fortress.FullPath, $"{fortress.FullPath}{TermHelper.GetZippedFileEnding()}");
                Directory.Delete(fortress.FullPath, true);
                Logger.log.Debug("Fortress has been zipped.");

                Logger.log.Info("Fortress has been sucessfully built!");
            }
            catch (Exception ex)
            {
                Logger.log.Error($"During building fortress: {ex}");

                // Delete all changes that have been made to this point. We do not want half-built fortresses.
                if (Directory.Exists(fortress.FullPath))
                {
                    Directory.Delete(fortress.FullPath, true);
                    Logger.log.Debug($"Fortress has been reversed due to an error: {fortress.FullPath}");
                }
                if (File.Exists(Path.Combine(fortress.FullPath, TermHelper.GetZippedFileEnding())))
                {
                    File.Delete(fortress.FullPath + TermHelper.GetZippedFileEnding());
                    Logger.log.Debug($"Fortress has been reversed due to an error: {fortress.FullPath}");
                }

                throw new FortressException($"Couldn't build fortress. All changes made to this point have been reversed. {Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Opens a <see cref="Fortress"/> and loads the database.
        /// </summary>
        public void BuildFortress(string fortressFullPath, string fortressName, string password)
        {
            try
            {
                Logger.log.Info($"Start opening the fortress {fortressFullPath}...");
                var aesHelper = new AesHelper();

                // =========================================================== Unzip the fortress - Read salt

                var unzippedFortress = ZipHelper.UnzipSavedZip(fortressFullPath);
                var entryOfSalt = fortressName + "/salt" + TermHelper.GetTextFileEnding();
                var saltEntry = unzippedFortress.GetEntry(entryOfSalt);

                var saltBytes = new byte[32];
                using (var stream = saltEntry.Open())
                {
                    saltBytes = ByteHelper.ReadBytesOfStream(stream);
                    stream.Close();
                }
                Logger.log.Debug("Unzipped fortress - Salt bytes read.");

                // =========================================================== Create masterkey

                var hashedKey = aesHelper.CreateKey(password, 512, saltBytes);
                var masterKey = new Masterkey(hashedKey);
                Logger.log.Debug("Masterkey created.");

                // =========================================================== Decrypt database

                var entryOfDatabase = fortressName + "/" + TermHelper.GetDatabaseTerm() + TermHelper.GetDatabaseEnding();
                var databaseEntry = unzippedFortress.GetEntry(entryOfDatabase);
                var aesAlg = new AesAlgorithm();

                using (var stream = databaseEntry.Open())
                {
                    var dbBytes = ByteHelper.ReadBytesOfStream(stream);
                    var decryptedDb = aesAlg.Decrypt(dbBytes, masterKey.Value, saltBytes);
                    Logger.log.Info($"Decrypted {TermHelper.GetDatabaseTerm()}");
                    //File.WriteAllBytes("C:\\Users\\Nutzer\\Desktop\\decryptedTestFile", decryptedDb);

                    // =========================================================== Unzip database

                    var unzippedByteEntriesOfDb = ZipHelper.GetEntriesFromZipArchive(decryptedDb); // These are the entries in byte arrays

                    foreach (var byteArr in unzippedByteEntriesOfDb)
                    {
                        BuildModelsOutOfBytes(byteArr);
                    }

                    stream.Close();
                }

            }
            catch (Exception ex)
            {
                Logger.log.Error($"During loading a fortress: {ex}");
                throw new FortressException($"Couldn't rebuild fortress. All data has been flushed out of the memory. {Environment.NewLine}{ex}");
            }
        }

    }
}
