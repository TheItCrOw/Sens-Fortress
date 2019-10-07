using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SensFortress.Utility
{
    /// <summary>
    /// Class that creates and holds every necessesary directory.
    /// </summary>
    static class DirectoryHelper
    {
        /// <summary>
        /// Creates and returns Directory for the <see cref="LogHelper"/>.
        /// </summary>
        /// <returns></returns>
        public static string GetLogDirectory()
        {
            var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sen's Fortress\\");
            var createdDirectory = System.IO.Directory.CreateDirectory(fullPath);            
            return fullPath + "Log.txt";
        }

    }
}
