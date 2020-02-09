using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SensFortress.Utility
{
    /// <summary>
    /// Class that creates and holds every necessesary directory.
    /// </summary>
    public static class IOPathHelper
    {
        private static string _appdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        /// <summary>
        /// Creates a new directory in the given path.
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string fullName)
        {
            if (!Directory.Exists(fullName))
                Directory.CreateDirectory(fullName);
        }

        /// <summary>
        /// Gets the path where the current exe lies.
        /// </summary>
        /// <returns></returns>
        public static string GetAppLocation() => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Returns the path of the log4net.config of the logger.
        /// </summary>
        /// <returns></returns>
        public static string Getlog4netConfigFile() => AppDomain.CurrentDomain.BaseDirectory + "\\log4net.config";

        /// <summary>
        /// Gets the desktopPath of the current user.
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopPath() => (Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

        /// <summary>
        /// Returns the path where the log file should be saved.
        /// </summary>
        /// <returns></returns>
        public static string GetLogFileDirectory() => $"{_appdataPath}\\Sen's Fortress\\log\\Sen's Fortress.log";

        /// <summary>
        /// Gets the default location of all fortresses saved.
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultFortressDirectory() => $"{_appdataPath}\\Sen's Fortress\\Fortresses";

        /// <summary>
        /// Gets the location of the file that holds newly selected fortresses.
        /// </summary>
        /// <returns></returns>
        public static string GetLinkedFortressListFile() => $"{_appdataPath}\\Sen's Fortress\\Configs\\Linked Fortresses.config";

        /// <summary>
        /// Gets the location of the directory where the linkedFortresses are being written to.
        /// </summary>
        /// <returns></returns>
        public static string GetConfigsDirectory() => $"{_appdataPath}\\Sen's Fortress\\Configs";

        /// <summary>
        /// Gets the location of the backed up fortresses.
        /// </summary>
        /// <returns></returns>
        public static string GetBackedUpFortressDirectory() => $"{_appdataPath}\\Sen's Fortress\\Backups";

        /// <summary>
        /// Returns the full path of the password_black list file.
        /// </summary>
        /// <returns></returns>
        public static string GetPasswordBlackListFile() => $"{_appdataPath}\\Sen's Fortress\\Lists\\{TermHelper.GetPasswordBlackListName()}";

        /// <summary>
        /// Returns the path of the "lists" direcoty
        /// </summary>
        /// <returns></returns>
        public static string GetListsDirectory() => $"{_appdataPath}\\Sen's Fortress\\Lists";

        /// <summary>
        /// Returns the path of the settings config file of the current fortress. Only use this after someone  has logged in!
        /// </summary>
        /// <returns></returns>
        public static string GetSettingsFile() => GetConfigsDirectory() + $"\\{TermHelper.GetSettingsConfigName()}";
    }
}
