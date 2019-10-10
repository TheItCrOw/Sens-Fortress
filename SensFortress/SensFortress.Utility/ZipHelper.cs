using Ionic.Zlib;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SensFortress.Utility
{
    public static class ZipHelper
    {

        /// <summary>
        /// Takes in a file and zip-name and creates a zip archive.
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="fullZipName"></param>
        /// <param name="extractPath"></param>
        public static void ZipSavedArchives(string fullFileName, string fullZipName)
        {
            Logger.log.Info($"Trying to zip from {fullFileName} to {fullZipName}...");
            ZipFile.CreateFromDirectory(fullFileName, fullZipName, System.IO.Compression.CompressionLevel.Fastest, true);
            Logger.log.Info("Zip successfull!");
        }

        public static void UnzipSavedZip(string fullZipName)
        {
            var test = ZipFile.OpenRead(fullZipName);
            var xd = File.ReadAllBytes(fullZipName);

        }

    }
}
