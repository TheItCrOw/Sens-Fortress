using Ionic.Zlib;
using SensFortress.Utility.Exceptions;
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
            try
            {
                Logger.log.Info($"Trying to zip from {fullFileName} to {fullZipName}...");
                ZipFile.CreateFromDirectory(fullFileName, fullZipName, System.IO.Compression.CompressionLevel.Fastest, true);
                Logger.log.Info("Zip successfull!");
            }
            catch (ZipArchiveException ex)
            {
                Logger.log.Error($"Error during zipping: {ex}");
                throw new ZipArchiveException("Something wrent wrong while trying to zip an archive. ", ex);
            }
        }

        /// <summary>
        /// Unzips a saved file and returns the ZipArchive
        /// </summary>
        /// <param name="fullZipName"></param>
        /// <returns></returns>
        public static ZipArchive UnzipSavedZip(string fullZipName)
        {
            try
            {
                Logger.log.Info($"Unzipping file from {fullZipName}.");
                return ZipFile.OpenRead(fullZipName);
            }
            catch (ZipArchiveException ex)
            {
                Logger.log.Error($"Error during unzipping: {ex}");
                throw new ZipArchiveException("Something wrent wrong while trying to unzip an archive. ", ex);
            }
        }

    }
}
