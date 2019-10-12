using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Creates a new Database with a mastereky.
        /// </summary>
        public void CreateNewFortress(string fullPath)
        {
            try
            {
                Logger.log.Info("Starting to build a new fortress...");

                // Create the root directory
                DirectoryHelper.CreateDirecotry(fullPath);
                Logger.log.Info($"Created Direcotry at {fullPath}.");

                // Create the sub directory for the database
                DirectoryHelper.CreateDirecotry(fullPath + "\\" + TermHelper.GetDatabaseTerm());
                Logger.log.Info($"Created sub directory for the {TermHelper.GetDatabaseTerm()}");

                // Create the file which holds the salt to unlock the database
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
