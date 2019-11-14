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

        private XmlDataCache _xmlDataCache = new XmlDataCache();

        /// <summary>
        /// Sets the path for the current database that is being used.
        /// </summary>
        /// <param name="path"></param>
        public void InitializeCurrentDatacache(string path) => _xmlDataCache.InitializeXmlDatacache(path);

        public void TestStoreOne(ModelBase model)
        {
            _xmlDataCache.StoreOne<Fortress>(model);
        }

        /// <summary>
        /// Creates a new <see cref="Fortress"/> with a <see cref="MasterKey"/> and saves it encrypted.
        /// </summary>
        public void CreateNewFortress(Fortress fortress)
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

                _xmlDataCache.StoreSalt(fortress.FullPath, fortress.Salt);
                Logger.log.Debug("Stored salt");

                // =========================================================== Store the user Input in the database

                _xmlDataCache.StoreOne<Fortress>(fortress);
                Logger.log.Debug("Stored fortress information.");

                // =========================================================== Zip only the database 

                ZipHelper.ZipSavedArchives(databasePath, databasePath + TermHelper.GetZippedFileEnding());
                Directory.Delete(databasePath, true);
                Logger.log.Debug($"{TermHelper.GetDatabaseTerm()} has been zipped.");

                // =========================================================== Encrypt the database

                var aesAlg = new AesAlgorithm();
                // Read all bytes from the database directory
                var data = File.ReadAllBytes(databasePath + TermHelper.GetZippedFileEnding());
                // Encrypt it
                var encryptedData = aesAlg.Encrypt(data, fortress.MasterKey.Value, fortress.Salt);
                // Write the encrypted file
                File.WriteAllBytes(databasePath + TermHelper.GetDatabaseEnding(), encryptedData);
                // Delete the zip
                File.Delete(databasePath + TermHelper.GetZippedFileEnding());
                Logger.log.Debug($"Encrypted {TermHelper.GetDatabaseTerm()}");

                // =========================================================== Zip the whole fortress

                ZipHelper.ZipSavedArchives(fortress.FullPath, fortress.FullPath + TermHelper.GetZippedFileEnding());
                Directory.Delete(fortress.FullPath, true);
                Logger.log.Debug("Fortress has been zipped.");

                Logger.log.Info("Fortress has been sucessfully built!");
            }
            catch (Exception ex)
            {
                Logger.log.Error($"During building fortress: {ex}");

                // Delete all changes that have been made to this point. We do not want half-built fortresses.
                if(Directory.Exists(fortress.FullPath))
                {
                    Directory.Delete(fortress.FullPath, true);
                    Logger.log.Debug($"Fortress has been reversed due to an error: {fortress.FullPath}");
                }
                if(File.Exists(fortress.FullPath + TermHelper.GetZippedFileEnding()))
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

                    foreach(var byteArr in unzippedByteEntriesOfDb)
                    {
                        _xmlDataCache.BuildModelsOutOfBytes(byteArr);
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
