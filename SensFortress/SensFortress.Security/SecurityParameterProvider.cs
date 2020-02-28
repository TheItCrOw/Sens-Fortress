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
        #region lazy pattern
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
        #endregion
        /// <summary>
        /// In order to see, if any changes were made to the fortress outside of the applciation - we hash entities and compare the hash afterwards.
        /// </summary>
        private Dictionary<string, string> _compareHashes = new Dictionary<string, string>();

        /// <summary>
        /// Updates the given name in the hashes dictionary. If name doesn't exist yet, it is added into the dict.
        /// </summary>
        /// <param name="nOfupdateable"></param>
        /// <param name="nonDefaultPath"></param>
        public void UpdateHash(string nOfupdateable, string path)
        {
            // Check if dict has an entry for the given name
            if (_compareHashes.ContainsKey(nOfupdateable))
            {
                _compareHashes[nOfupdateable] = ByteHelper.ReadHash(path);
            }
            else // If it doesnt exist => add it to the dict
                _compareHashes.Add(nOfupdateable, ByteHelper.ReadHash(path));
        }

        public void UpdateHash(string nOfupdateable, object hashableO)
        {
            // Check if dict has an entry for the given name
            if (_compareHashes.ContainsKey(nOfupdateable))
            {
                _compareHashes[nOfupdateable] = hashableO.GetHashCode().ToString();
            }
            else // If it doesnt exist => add it to the dict
                _compareHashes.Add(nOfupdateable, hashableO.GetHashCode().ToString());
        }

        /// <summary>
        /// Returns the hash of the given name. Returns null if not found.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentHashOf(string name) => _compareHashes.ContainsKey(name) == true ? _compareHashes[name] : null;
    }
}
