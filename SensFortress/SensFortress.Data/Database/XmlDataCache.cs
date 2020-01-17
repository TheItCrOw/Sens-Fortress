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
        private Dictionary<Guid, byte[]> _secureDatacache;

        /// <summary>
        /// Represents the unencrypted Datacache in the RAM
        /// </summary>
        private Dictionary<Type, List<ModelBase>> _unsecureDatacache;

        /// <summary>
        /// Set the path of the current <see cref="TermHelper.GetDatabaseTerm()"/>.
        /// </summary>
        /// <param name="path"></param>
        internal XmlDataCache(string path)
        {
            var splited = path.Split('.');
            _databasePath = splited[0];
            _isInitialized = true;
            // Get a list of all modeltypes in the assembly for creating them again
            _modelTypes = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
            _secureDatacache = new Dictionary<Guid, byte[]>();
            _unsecureDatacache = new Dictionary<Type, List<ModelBase>>();
        }
        /// <summary>
        /// Do NOT use this unless a salt is stored in a single file.
        /// </summary>
        internal void StoreSalt(string path, byte[] salt)
        {
            CheckDatacache();

            File.WriteAllBytes(Path.Combine(path, $"salt{TermHelper.GetTextFileEnding()}"), salt);
        }

        /// <summary>
        /// Builds models out of one byte array.
        /// </summary>
        /// <param name="arr"></param>
        internal T BuildModelsOutOfBytes<T>(byte[] arr, bool isByteModel = false) where T : Models.Interfaces.ISerializable, IDeletable
        {
            if(!isByteModel)
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
                    return (T)model;
                }
            }
            else
            {
                var model = ByteHelper.ByteArrayToObject(arr);
                return (T)model;
            }
        }

        /// <summary>
        /// Stores a serializible model into the datacache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        internal void StoreOne(ModelBase model, bool isByteModel = false, ByteModel byteModel = null)
        {
            CheckDatacache();
            // If its a normal ModelBase storing.
            if (!isByteModel)
            {
                var ds = new DataContractSerializer(model.GetType());
                var settings = new XmlWriterSettings { Indent = true };
                var currentSaveLocation = Path.Combine(_databasePath, TermHelper.GetDatabaseTerm(), model.GetType().Name);

                // Always check if a directory exists. If not, create it.
                IOPathHelper.CreateDirectory(currentSaveLocation);

                using (var sww = new StringWriter())
                {
                    using (var w = XmlWriter.Create(Path.Combine(currentSaveLocation, $"{model.Id}.xml"), settings))
                    {
                        ds.WriteObject(w, model);
                    }
                }
            }
            else if(isByteModel) // If it's a byteModel
            {
                var currentSaveLocation = Path.Combine(_databasePath, TermHelper.GetDatabaseTerm(), byteModel.GetType().Name);

                // It's important to write the bytes decrypted since MemProtection works with the localUser. So the data would be bound to this pc and user.
                var decryptedValue = CryptMemoryProtection.DecryptInMemoryData(byteModel.EncryptedValue);

                // Always check if a directory exists. If not, create it.
                IOPathHelper.CreateDirectory(currentSaveLocation);

                // Write the Value of byteModels into a file with the foreignKey as the name.
                File.WriteAllBytes($"{currentSaveLocation}\\{byteModel.ForeignKey}", decryptedValue);
                decryptedValue = null;
            }
        }

        /// <summary>
        /// Takes a byte array and stores it savely in the <see cref="_secureDatacache"/>
        /// </summary>
        internal void AddToSecureMemoryDC(Guid foreignKey, byte[] modelBytes)
        {            
            CheckDatacache();
            var encrpytedBytes = CryptMemoryProtection.EncryptInMemoryData(modelBytes);
            modelBytes = null;
            _secureDatacache.Add(foreignKey, encrpytedBytes);
        }

        /// <summary>
        /// Delets a sensible Model from the secureDC
        /// </summary>
        /// <param name="foreignKey"></param>
        /// <param name="modelBytes"></param>
        internal void DeleteOneFromSecureMemoryDC(Guid foreignKey)
        {
            CheckDatacache();
            _secureDatacache.Remove(foreignKey);
        }

        /// <summary>
        /// Add a <see cref="ModelBase"/> to the unsecureMemoryDc
        /// </summary>
        /// <param name="modelBytes"></param>
        internal void AddToUnsecureMemoryDC(ModelBase model)
        {
            CheckDatacache();

            if (_unsecureDatacache.TryGetValue(model.GetType(), out var listOfModels))
            {
                listOfModels.Add(model);
            }
            else
            {
                if (model is ModelBase)
                {
                    var newList = new List<ModelBase>() { model };
                    _unsecureDatacache.Add(model.GetType(), newList);
                }
            }
        }

        /// <summary>
        /// Deletes the given ModelBase from the unsecure MemoryDC
        /// </summary>
        /// <param name="model"></param>
        internal void DeleteOneFromUnsecureMemoryDC(ModelBase model)
        {
            if (_unsecureDatacache.TryGetValue(model.GetType(), out var listOfModels))
            {
                if (listOfModels.Contains(model))
                {
                    listOfModels.Remove(model);
                }
                else
                {
                    var ex = new ArgumentNullException($"{model.Id} could not be found in the MemoryDC for deletion.");
                    ex.SetUserMessage("Item has already been deleted.");
                    throw ex;
                }
            }
            else
            {
                var ex = new ArgumentNullException($"{model.Id} could not be found in the MemoryDC for deletion.");
                ex.SetUserMessage("Item has already been deleted.");
                throw ex;
            }
        }

        /// <summary>
        /// Saves all changes made in memory to disk
        /// </summary>
        internal void SaveFortress(Masterkey masterkey)
        {
            var currentFortress = GetAllFromUnsecure<Fortress>().FirstOrDefault();
            currentFortress.FullPath = _databasePath;
            currentFortress.MasterKey = masterkey;
            currentFortress.Salt = CurrentFortressData.Salt;
            WriteFortress(currentFortress, true);
            masterkey = null;
        }

        /// <summary>
        /// Get all models of type T from the unsecureDatacache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal IEnumerable<T> GetAllFromUnsecure<T>()
        {
            CheckDatacache();
            if (_unsecureDatacache.TryGetValue(typeof(T), out var allModels))
            {
                return allModels.Cast<T>();
            }
            else
            {
                var ex = new XmlDataCacheException($"Could not get the model {typeof(T).Name} from unsecureDC");
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }
        }

        /// <summary>
        /// Returns the model with the given foreignKey from the secureDatacache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="foreignKey"></param>
        /// <returns></returns>
        internal bool TryGetSensible<T>(Guid foreignKey, out T model)
        {
            CheckDatacache();
            if(_secureDatacache.TryGetValue(foreignKey, out var encryptedBytes))
            {
                var decryptedBytes = CryptMemoryProtection.DecryptInMemoryData(encryptedBytes);
                model = (T)ByteHelper.ByteArrayToObject(decryptedBytes);
                return true;
            }

            model = (T)new object();
            return false;
        }

        /// <summary>
        /// Throws an exception if the DC has not bene initialzed.
        /// </summary>
        private void CheckDatacache()
        {
            if (!_isInitialized)
            {
                var ex = new XmlDataCacheException("XmlDataCache has not been initialized.");
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }
        }


        /// <summary>
        /// Creates a new <see cref="Fortress"/> with a <see cref="MasterKey"/> and saves it encrypted.
        /// </summary>
        internal void WriteFortress(Fortress fortress, bool overwrite = false)
        {
            try
            {
                Logger.log.Info("Starting to write a fortress...");
                var databasePath = Path.Combine(fortress.FullPath, TermHelper.GetDatabaseTerm());

                // =========================================================== Create the root directory

                IOPathHelper.CreateDirectory(fortress.FullPath);
                Logger.log.Debug($"Created outer walls {fortress.FullPath}.");

                // =========================================================== Create the sub directories for the database

                IOPathHelper.CreateDirectory(databasePath);
                Logger.log.Debug($"Created the {TermHelper.GetDatabaseTerm()}");

                // =========================================================== Create the file which holds the salt to unlock the database

                StoreSalt(fortress.FullPath, fortress.Salt);
                Logger.log.Debug("Stored salt");

                // =========================================================== Store the user Input and initial data in the database

                foreach (var modelList in _unsecureDatacache.Values) // UnsecureDatacache
                    foreach (var model in modelList)
                    {
                        StoreOne(model);
                    }

                foreach(var pair in _secureDatacache)
                {
                    var byteModel = new ByteModel(pair.Key, pair.Value);
                    StoreOne(null, true, byteModel);
                }

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

                if (overwrite)
                    File.Delete($"{fortress.FullPath}{TermHelper.GetZippedFileEnding()}");

                ZipHelper.ZipSavedArchives(fortress.FullPath, $"{fortress.FullPath}{TermHelper.GetZippedFileEnding()}");
                Directory.Delete(fortress.FullPath, true);
                Logger.log.Debug("Fortress has been zipped.");

                Logger.log.Info("Fortress has been sucessfully written!");
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
                using (unzippedFortress)
                {
                    var entryOfSalt = fortressName + "/salt" + TermHelper.GetTextFileEnding();
                    var saltEntry = unzippedFortress.GetEntry(entryOfSalt);

                    var saltBytes = new byte[32];
                    using (var stream = saltEntry.Open())
                    {
                        saltBytes = ByteHelper.ReadBytesOfStream(stream);
                    }
                    CurrentFortressData.Salt = saltBytes;
                    Logger.log.Debug("Unzipped fortress - Salt bytes read.");

                    // =========================================================== Create masterkey

                    var hashedKey = aesHelper.CreateKey(password, 512, saltBytes);
                    password = string.Empty; // Delete the password in plaintext from RAM
                    var masterKey = new Masterkey(hashedKey);
                    hashedKey = null; // Hash also
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
                        // We distinguish between sensible data and normal data. We put the sensible data into the secureDatacache.
                        var unzippedByteEntriesOfDb = ZipHelper.GetEntriesFromZipArchive(decryptedDb); // These are the entries in byte arrays
                        decryptedDb = null;
                        // Add to secureDC.
                        foreach (var sensibleBytes in unzippedByteEntriesOfDb.Item2.Item2.ToList()) // ToList() otherwise the iterations throws exception
                        {
                            AddToSecureMemoryDC(unzippedByteEntriesOfDb.Item2.Item1.Pop(), unzippedByteEntriesOfDb.Item2.Item2.Pop());
                        }
                        foreach (var bytes in unzippedByteEntriesOfDb.Item1.ToList()) // Add not sensible data to the "unsecure" DC.
                        {
                            AddToUnsecureMemoryDC(BuildModelsOutOfBytes<ModelBase>(unzippedByteEntriesOfDb.Item1.Pop()));
                        }
                        unzippedByteEntriesOfDb = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }
        }

        /// <summary>
        /// Validates the masterkey by decrypting the given fortress and flushing the memory afterwards.
        /// </summary>
        /// <param name="fortressFullPath"></param>
        /// <param name="fortressName"></param>
        /// <param name="password"></param>
        internal void ValidateMasterKey(string fortressFullPath, string fortressName, string password)
        {
            try
            {
                Logger.log.Info($"Start validating the masterkey of fortress {fortressFullPath}...");
                var aesHelper = new AesHelper();

                // =========================================================== Unzip the fortress - Read salt

                var unzippedFortress = ZipHelper.UnzipSavedZip(fortressFullPath);
                using (unzippedFortress)
                {
                    var entryOfSalt = fortressName + "/salt" + TermHelper.GetTextFileEnding();
                    var saltEntry = unzippedFortress.GetEntry(entryOfSalt);

                    var saltBytes = new byte[32];
                    using (var stream = saltEntry.Open())
                    {
                        saltBytes = ByteHelper.ReadBytesOfStream(stream);
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
                        Logger.log.Info($"Validated {TermHelper.GetDatabaseTerm()}");
                        decryptedDb = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }
        }

    }
}
