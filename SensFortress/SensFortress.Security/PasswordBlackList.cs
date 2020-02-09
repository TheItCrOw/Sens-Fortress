using SensFortress.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SensFortress.Security
{
    /// <summary>
    /// Wrapper for the pw blacklist.
    /// </summary>
    public static class PasswordBlackList
    {
        private static string[] _blacklist;

        public static string[] GetBlackList()
        {
            if (_blacklist == null)
                _blacklist = File.ReadAllLines(IOPathHelper.GetPasswordBlackListFile());

            return _blacklist;
        }
    }
}
