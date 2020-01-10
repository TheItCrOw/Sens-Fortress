using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Utility
{
    public static class WellKnownSpecialCharacters
    {
        /// <summary>
        /// Contains a collection of special characters.
        /// </summary>
        public static readonly string[] SpecialCharacters =
        {
            "!", "\"", "#", "$", "%", "&", "'",
            "(", ")", "*", "+", ",", "-", ".",
            "/", ":", ";", "<", "=", ">", "?",
            "@", "[", "\\","]", "^", "_", "'",
            "{", "}", "|", "~"
        };

        /// <summary>
        /// Returns true when string contains special characters.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ContainsSpecialCharacters(string s)
        {
            var foundSpecial = false;
            foreach (var ch in SpecialCharacters)
            {
                if (s.Contains(ch))
                {
                    foundSpecial = true;
                }
            }
            return foundSpecial;
        }

    }
}
