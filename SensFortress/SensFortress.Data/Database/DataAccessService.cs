using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
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

                // Create the root directory
                DirectoryHelper.CreateDirecotry(fortress.FullPath);
                Logger.log.Info($"Created Direcotry at {fortress.FullPath}.");

                // Create the sub directory for the database
                DirectoryHelper.CreateDirecotry(fortress.FullPath + "\\" + TermHelper.GetDatabaseTerm());
                Logger.log.Info($"Created sub directory for the {TermHelper.GetDatabaseTerm()}");

                // Create the file which holds the salt to unlock the database
                _xmlDataCache.StoreSalt(fortress.FullPath, fortress.Salt);
                Logger.log.Info("Stored salt");

                // Store the user Input in the database
                var allProperties = fortress.GetType().GetProperties();



                //// test
                //var data = File.ReadAllBytes("C:\\Users\\Nutzer\\Desktop\\encryptedTestFile.sfdb");
                //var aesAlg = new AesAlgorithm();
                //aesAlg.Encrypt(data, fortress.MasterKey.Value, fortress.Salt);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
