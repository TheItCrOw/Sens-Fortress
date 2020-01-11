﻿using SensFortress.Utility.Exceptions;
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
        /// <returns>item1: List of unsecurebytes, item2: Tuple with Guids and bytes as Stacks</returns>
        public static Tuple<Stack<byte[]>, Tuple<Stack<Guid>, Stack<byte[]>>> GetEntriesFromZipArchive(byte[] data, bool protectedMemory = false)
        {
            using (var zippedStream = new MemoryStream(data))
            {
                using (var archive = new ZipArchive(zippedStream))
                {
                    // unsecure data
                    var openData = new Stack<byte[]>();
                    // sensible data
                    var guids = new Stack<Guid>();
                    var sensibleBytes = new Stack<byte[]>();
                    var sensibleData = Tuple.Create(guids, sensibleBytes);
                    var entries = Tuple.Create(openData, sensibleData);
                    foreach(var entry in archive.Entries)
                    {
                        bool sensible = false;

                        if(entry.FullName.StartsWith($"{TermHelper.GetDatabaseTerm()}/ByteModel"))
                            sensible = true;

                        if (entry != null)
                        {
                            using (var unzippedEntryStream = entry.Open())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    unzippedEntryStream.CopyTo(ms);
                                    var unzippedArray = ms.ToArray();
                                    if (sensible)
                                    {
                                        entries.Item2.Item1.Push(new Guid(entry.Name));
                                        entries.Item2.Item2.Push(unzippedArray);
                                    }
                                    else
                                        entries.Item1.Push(unzippedArray);
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
