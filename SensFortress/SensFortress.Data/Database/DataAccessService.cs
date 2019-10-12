using SensFortress.Data.Exceptions;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

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
        /// Creates a new Database with a mastereky.
        /// </summary>
        public void CreateNewFortress(Fortress fortress)
        {
            try
            {
                Logger.log.Info("Starting to build a new fortress...");
                var dataBasePath = fortress.FullPath + "\\" + TermHelper.GetDatabaseTerm();

                // =========================================================== Create the root directory

                DirectoryHelper.CreateDirecotry(fortress.FullPath);
                Logger.log.Debug($"Created outer walls {fortress.FullPath}.");

                // =========================================================== Create the sub directory for the database

                DirectoryHelper.CreateDirecotry(dataBasePath);
                Logger.log.Debug($"Created the {TermHelper.GetDatabaseTerm()}");

                // =========================================================== Create the file which holds the salt to unlock the database

                _xmlDataCache.StoreSalt(fortress.FullPath, fortress.Salt);
                Logger.log.Debug("Stored salt");

                // =========================================================== Store the user Input in the database

                _xmlDataCache.StoreOne<Fortress>(dataBasePath, fortress);
                Logger.log.Debug("Stored fortress information.");

                // =========================================================== Zip only the database 

                ZipHelper.ZipSavedArchives(dataBasePath, dataBasePath + TermHelper.GetZippedFileEnding());
                Directory.Delete(dataBasePath, true);
                Logger.log.Debug($"{TermHelper.GetDatabaseTerm()} has been zipped.");

                // =========================================================== Encrypt the database

                var aesAlg = new AesAlgorithm();
                // Read all bytes from the database directory
                var data = File.ReadAllBytes(dataBasePath + TermHelper.GetZippedFileEnding());
                // Encrypt it
                var encryptedData = aesAlg.Encrypt(data, fortress.MasterKey.Value, fortress.Salt);
                // Write the encrypted file
                File.WriteAllBytes(dataBasePath + TermHelper.GetDatabaseEnding(), encryptedData);
                // Delete the zip
                File.Delete(dataBasePath + TermHelper.GetZippedFileEnding());
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

                throw new FortressException($"An error ocurred while building the fortress. All operations to this point have been reversed.", ex);
            }
        }

        /// <summary>
        /// Open a fortress and load the database
        /// </summary>
        public ZipArchive GetFortress(string fortressPath)
        {
            try
            {
                Logger.log.Info($"Start opening the fortress {fortressPath}...");

                // =========================================================== Unzip the fortress
                var unzippedFortress = ZipHelper.UnzipSavedZip(fortressPath);
                Logger.log.Debug("Unzipped fortress.");

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
