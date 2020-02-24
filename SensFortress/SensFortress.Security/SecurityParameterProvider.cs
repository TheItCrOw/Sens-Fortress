using SensFortress.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SensFortress.Security
{
    /// <summary>
    /// Static class that holds important parameters for keeping track of malicious modifications
    /// </summary>
    public class SecurityParameterProvider
    {
        /// <summary>
        /// Implement the lazy pattern.
        /// </summary>
        private static readonly Lazy<SecurityParameterProvider>
            lazy =
            new Lazy<SecurityParameterProvider>
            (() => new SecurityParameterProvider());

        /// <summary>
        /// Instance of the <see cref="Factory"/> class.
        /// </summary>
        public static SecurityParameterProvider Instance { get { return lazy.Value; } }

        /// <summary>
        /// In order to see, if any changes were made to the fortress outside of the applciation - we hash it and compare the hash afterwards.
        /// </summary>
        private Dictionary<string, string> _fortressHashes = new Dictionary<string, string>();

        /// <summary>
        /// Holds the current hash of the fortress.
        /// </summary>
        private string _currentFortressHash;

        /// <summary>
        /// Updates the fortress hash for comparing it later.
        /// </summary>
        public void UpdateFortressHash(string nonDefaultPath = null)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var path = (nonDefaultPath == null) ? CurrentFortressData.FullPath : nonDefaultPath;
                var buffer = md5.ComputeHash(File.ReadAllBytes(path));
                var sb = new StringBuilder();
                for (var i = 0; i < buffer.Length; i++)
                {
                    sb.Append(buffer[i].ToString("x2"));
                }

                _currentFortressHash = sb.ToString();
            }
        }

        /// <summary>
        /// Returns the hash of the current fortress.
        /// </summary>
        /// <returns></returns>
        public string GetHashOfFortress() => _currentFortressHash;

    }
}
