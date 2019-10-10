using System;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Reflection;

namespace SensFortress.Utility.Testing
{
    public static class ZipTests
    {

        public static void CustomZipHelper(string startPath, string zipPath, string extractPath)
        {
            startPath = "C:\\Users\\Nutzer\\Desktop\\DateTickets";
            zipPath = "C:\\Users\\Nutzer\\Desktop\\TestZip.zip";
            extractPath = "C:\\Users\\Nutzer\\Desktop\\extract";

            ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);

            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }

    }
}
