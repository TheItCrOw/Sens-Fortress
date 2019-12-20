﻿using SensFortress.Data.Exceptions;
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
        private Dictionary<Type, List<ModelBase>> _unsecureDatacache;

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
            _unsecureDatacache = new Dictionary<Type, List<ModelBase>>();
        }
        /// <summary>
        /// Do NOT use this unless a salt is stored in a single file.
        /// </summary>
        internal void StoreSalt(string path, byte[] salt)
        {
            CheckDatacache();

            try
            {
                File.WriteAllBytes(Path.Combine(path, $"salt{TermHelper.GetTextFileEnding()}"), salt);
            }
            catch (Exception ex)
            {
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }
        }

        /// <summary>
        /// Builds models out of one byte array.
        /// </summary>
        /// <param name="arr"></param>
        internal T BuildModelsOutOfBytes<T>(byte[] arr) where T : ModelBase
        {
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
                    return (T)model;
                }
            }
            catch (Exception ex)
            {
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }
        }

        /// <summary>
        /// Stores a serializible model into the datacache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        internal void StoreOne<T>(ModelBase model) where T : Models.Interfaces.ISerializable
        {
            CheckDatacache();

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
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                throw ex;
            }
        }

        /// <summary>
        /// Takes a byte array and stores it savely in the <see cref="_secureDatacache"/>
        /// </summary>
        internal void AddToSecureMemoryDC(byte[] modelBytes)
        {
            CheckDatacache();
            var encrpytedBytes = CryptMemoryProtection.EncryptInMemoryData(modelBytes);
            modelBytes = null;
            _secureDatacache.Add(encrpytedBytes);
        }

        /// <summary>
        /// Creates the unsecureDC from a List of models.
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
                if(model is ModelBase)
                {
                    var newList = new List<ModelBase>() { model };
                    _unsecureDatacache.Add(model.GetType(), newList);
                }
            }
        }

        /// <summary>
        /// Get all models of type T from the unsecureDatacache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal IEnumerable<T> GetAllFromUnsecure<T>()
        {
            CheckDatacache();
            if(_unsecureDatacache.TryGetValue(typeof(T), out var allModels))
            {
                return allModels.Cast<T>();
            }
            else
            {
                var emptyList = new List<T>();
                return emptyList;
            }
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

                // =========================================================== Create the sub directories for the database

                IOPathHelper.CreateDirectory(databasePath);
                Logger.log.Debug($"Created the {TermHelper.GetDatabaseTerm()}");

                // =========================================================== Create the file which holds the salt to unlock the database

                StoreSalt(fortress.FullPath, fortress.Salt);
                Logger.log.Debug("Stored salt");

                // =========================================================== Store the user Input and initial data in the database

                var rootBranch = new Branch { Name = "Example: Projects", ParentBranchId = Guid.Empty };
                var subBranch = new Branch { Name = "Example: Passwords", ParentBranchId = rootBranch.Id };
                var examplePw = ByteHelper.StringToByteArray("thisIsAnExamplePassword");
                var leaf = new Leaf { Name = "Password1", Description = "Here you can describe this entry.", BranchId = subBranch.Id };
                var leafPw = new LeafPassword { LeafId = leaf.Id, Value = examplePw };
                examplePw = null;
                StoreOne<Fortress>(fortress);
                StoreOne<Branch>(rootBranch);
                StoreOne<Branch>(subBranch);
                StoreOne<Leaf>(leaf);
                StoreOne<LeafPassword>(leafPw);
                leafPw = null;
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
                    // We distinguish between sensible data and normal data. We put the sensible data into the secureDatacache.
                    var unzippedByteEntriesOfDb = ZipHelper.GetEntriesFromZipArchive(decryptedDb); // These are the entries in byte arrays
                    decryptedDb = null;
                    foreach (var sensibleBytes in unzippedByteEntriesOfDb.Item2.ToList()) // ToList() otherwise the iterations throws exception
                    {
                        AddToSecureMemoryDC(unzippedByteEntriesOfDb.Item2.Pop()); // Add sensible data to secure DC
                    }
                    foreach (var bytes in unzippedByteEntriesOfDb.Item1.ToList()) // Add not sensible data to the "unsecure" DC.
                    {
                        AddToUnsecureMemoryDC(BuildModelsOutOfBytes<ModelBase>(unzippedByteEntriesOfDb.Item1.Pop()));
                    }
                    unzippedByteEntriesOfDb = null;
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
