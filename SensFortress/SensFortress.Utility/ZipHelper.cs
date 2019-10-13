using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
            ZipFile.CreateFromDirectory(fullFileName, fullZipName, System.IO.Compression.CompressionLevel.Optimal, true);
            Logger.log.Info("Zip successfull!");
        }

        /// <summary>
        /// Unzips a saved file and returns the ZipArchive
        /// </summary>
        /// <param name="fullZipName"></param>
        /// <returns></returns>
        public static ZipArchive UnzipSavedZip(string fullZipName)
        {
            Logger.log.Info($"Unzipping file from {fullZipName}.");
            return ZipFile.OpenRead(fullZipName);
        }

        /// <summary>
        /// Decompresses a zipped <see cref="byte"/> array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<byte[]> GetEntriesFromZipArchive(byte[] data)
        {
            using (var zippedStream = new MemoryStream(data))
            {
                using (var archive = new ZipArchive(zippedStream))
                {
                    var entries = new List<byte[]>();
                    foreach(var entry in archive.Entries)
                    {
                        if (entry != null)
                        {
                            using (var unzippedEntryStream = entry.Open())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    unzippedEntryStream.CopyTo(ms);
                                    var unzippedArray = ms.ToArray();
                                    entries.Add(unzippedArray);
                                }
                            }
                        }
                    }
                    return entries;
                }
            }
        }
    }
}
