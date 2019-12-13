using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Utility
{
    /// <summary>
    /// Sen's Fortress uses abstract terms which are stored in here.
    /// </summary>
    public static class TermHelper
    {
        /// <summary>
        /// Gets the term for the database.
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseTerm() => "Storage Chamber";

        /// <summary>
        /// Encrypted database file ending.
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseEnding() => ".sfdb";

        /// <summary>
        /// Zipped file ending.
        /// </summary>
        /// <returns></returns>
        public static string GetZippedFileEnding() => ".sfzf";

        /// <summary>
        /// Normal textfile ending.
        /// </summary>
        /// <returns></returns>
        public static string GetTextFileEnding() => ".sftf";

    }
}
