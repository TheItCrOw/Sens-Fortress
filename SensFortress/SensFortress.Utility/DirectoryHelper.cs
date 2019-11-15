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
        public static void CreateDirectory(string fullName) => Directory.CreateDirectory(fullName);

        /// <summary>
        /// Returns the path of the log4net.config of the logger.
        /// </summary>
        /// <returns></returns>
        public static string Getlog4netConfigDirectory() => "E:\\WPF Projects\\Sens-Fortress\\SensFortress\\SensFortress.Utility\\Log\\log4net.config";
        // The user will adjust this in the UI later
        // When using PC
        //return "E:\\WPF Projects\\Sens-Fortress\\SensFortress\\SensFortress.Utility\\Log\\log4net.config";
        // When using Surface
        // "C:\\WPF Projects\\Sen's Fortress\\Sens-Fortress\\SensFortress\\SensFortress.Utility\\Log\\log4net.config"

        /// <summary>
        /// Gets the desktopPath of the current user.
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopPath() => (Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

        /// <summary>
        /// Returns the path where the log file should be saved.
        /// </summary>
        /// <returns></returns>
        public static string GetLogFileDirectory() => "C:\\Users\\Nutzer\\AppData\\Roaming\\Sen's Fortress\\Sen's Fortress.log";

    }
}
