using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SensFortress.Utility
{
    /// <summary>
    /// Class that creates and holds every necessesary directory.
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// Creates a new directory in the given path.
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirecotry(string fullName)
        {
            Directory.CreateDirectory(fullName);
        }

        /// <summary>
        /// Returns the path of the log4net.config of the logger.
        /// </summary>
        /// <returns></returns>
        public static string Getlog4netConfigDirectory()
        {
            // The user will adjust this in the UI later
            return "E:\\WPF Projects\\Sens-Fortress\\SensFortress\\SensFortress.Utility\\Log\\log4net.config"; 
        }

        /// <summary>
        /// Returns the path where the log file should be saved.
        /// </summary>
        /// <returns></returns>
        public static string GetLogFileDirectory()
        {
            // The user will adjust this in the UI later
            return "C:\\Users\\Nutzer\\AppData\\Roaming\\Sen's Fortress\\Sen's Fortress.log"; 
        }

    }
}
