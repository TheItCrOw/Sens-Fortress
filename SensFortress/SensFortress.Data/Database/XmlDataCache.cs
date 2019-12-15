using SensFortress.Data.Exceptions;
using SensFortress.Models.BaseClasses;
using SensFortress.Models.Fortress;
using SensFortress.Models.Interfaces;
using SensFortress.Security;
using SensFortress.Security.AES;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private IEnumerable<Type> _modelTypes;

        /// <summary>
        /// Represents the save Datacache stored in the RAM encryptedly.
        /// </summary>
        private List<byte[]> _secureDatacache;

        /// <summary>
        /// Represents the unencrypted Datacache in the RAM
        /// </summary>
        private Dictionary<Type, byte[]> _unsecureDatacache;

        /// <summary>
        /// Set the path of the current <see cref="TermHelper.GetDatabaseTerm()"/>.
        /// </summary>
        /// <param name="path"></param>
        internal XmlDataCache(string path)
        {
            _databasePath = path;
            _isInitialized = true;
            // Get a list of all modeltypes in the assembly for creating them again
            _modelTypes = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
            _secureDatacache = new List<byte[]>();
        }
        /// <summary>
        /// Do NOT use this unless a salt is stored in a single file.
        /// </summary>
        internal void StoreSalt(string path, byte[] salt)
        {
            if (!_isInitialized)
            {
                var ex = new XmlDataCacheException("XmlDataCache has not been initialized.");
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }

            try
            {
                File.WriteAllBytes(Path.Combine(path, $"salt{TermHelper.GetTextFileEnding()}"), salt);
            }
            catch (Exception ex)
            {
                ex.Source = $"{ex.Source} called by {MethodBase.GetCurrentMethod()}";
                ex.SetUserMessage("An error occured while trying to store data safely. Please wait as the memory is being flushed to prevent any leaks.");
                throw ex;
            }
        }

        /// <summary>
        /// Builds models out of one byte array.
        /// </summary>
        /// <param name="arr"></param>
        internal void BuildModelsOutOfBytes(byte[] arr)
        {
            if (arr == null)
            {
                var ex = new XmlDataCacheException("Tried to build a model, but the byteArray was null.");
                ex.Source = $"{ex.Source} called by {MethodBase.GetCurrentMethod()}";
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                using (var ms = new MemoryStream(arr))
                {
                    // doc holds the current xml File
                    doc.Load(ms);
                    var type = _modelTypes.First(t => t.Name == doc.DocumentElement.Name);
                    // Deserialize model
                    var reader = XmlDictionaryReader.CreateTextReader(arr, new XmlDictionaryReaderQuotas());
                    var dcs = new DataContractSerializer(type);
                    var model = dcs.ReadObject(reader);
                }
            }
            catch (Exception ex)
            {
                ex.Source = $"{ex.Source} called by {MethodBase.GetCurrentMethod()}";
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }
        }

        /// <summary>
        /// Takes a byte array and stores it savely in the <see cref="_secureDatacache"/>
        /// </summary>
        internal void AddToSecureMemoryDC(byte[] modelBytes)
        {
            var encrpytedBytes = CryptMemoryProtection.EncryptInMemoryData(modelBytes);
            modelBytes = null;
            _secureDatacache.Add(encrpytedBytes);
        }

        /// <summary>
        /// Stores a serializible model into the datacache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        internal void StoreOne<T>(ModelBase model) where T : Models.Interfaces.ISerializable
        {
            if (!_isInitialized)
            {
                var ex = new XmlDataCacheException("XmlDataCache has not been initialized.");
                ex.Source = $"{ex.Source} called by {MethodBase.GetCurrentMethod()}";
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }

            try
            {
                var ds = new DataContractSerializer(typeof(T));
                var obj = CastModelBase<T>(model);
                var settings = new XmlWriterSettings { Indent = true };
                var currentSaveLocation = Path.Combine(_databasePath, TermHelper.GetDatabaseTerm(), typeof(T).Name);

                // Always check if a directory exists. If not, create it.
                IOPathHelper.CreateDirectory(currentSaveLocation);

                using (var sww = new StringWriter())
                {
                    using (var w = XmlWriter.Create(Path.Combine(currentSaveLocation, $"{model.Id}.xml"), settings))
                    {
                        ds.WriteObject(w, obj);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Source = $"{ex.Source} called by {MethodBase.GetCurrentMethod()}";
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
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

                IOPathHelper.CreateDirectory(fortress.FullPath);
                Logger.log.Debug($"Created outer walls {fortress.FullPath}.");

                // =========================================================== Create the sub directory for the database

                IOPathHelper.CreateDirectory(databasePath);
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
                // Delete all changes that have been made to this point. We do not want half-built fortresses.
                if (Directory.Exists(fortress.FullPath))
                {
                    Directory.Delete(fortress.FullPath, true);
                }
                if (File.Exists(Path.Combine(fortress.FullPath, TermHelper.GetZippedFileEnding())))
                {
                    File.Delete(fortress.FullPath + TermHelper.GetZippedFileEnding());
                }

                ex.Source = $"{ex.Source} called by {MethodBase.GetCurrentMethod()}";
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
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
                password = string.Empty; // Delete the password in plaintext from RAM
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

                    // =========================================================== Unzip database

                    var unzippedByteEntriesOfDb = new Queue<byte[]>(ZipHelper.GetEntriesFromZipArchive(decryptedDb)); // These are the entries in byte arrays
                    decryptedDb = null;
                    foreach (var byteArr in unzippedByteEntriesOfDb.ToList()) // ToList() otherwise the iterations throws exception
                    {
                        AddToSecureMemoryDC(unzippedByteEntriesOfDb.Dequeue()); // Store the data into the secureMemoryDatacache
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Source = $"{ex.Source} called by {MethodBase.GetCurrentMethod()}";
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }
        }

    }
}
