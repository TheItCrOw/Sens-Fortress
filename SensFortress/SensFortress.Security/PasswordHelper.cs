using SensFortress.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensFortress.Security
{
    /// <summary>
    /// Helper for handling passwords.
    /// </summary>
    public static class PasswordHelper
    {
        private static string[] _chars = 
        {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", 
            "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", 
            "u", "v", "w", "x", "y", "z", "A", "B", "C", "D",
            "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",
            "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", 
            "Y", "Z",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
        };

        private static string[] _specialChars =
        {
            "!", "#", "$", "%", "&",
            "(", ")", "*", "+", ",", "-", ".",
            "/", ":", ";", "<", "=", ">", "?",
            "@", "[","]", "^", "_", "{", "}"
        };

        /// <summary>
        /// Generates a secure password out of chars, numbers and specials.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateSecurePassword(int length = 32)
        {
            var result = string.Empty;

            for(int i = 0; i < length; i++)
            {
                var random = new Random();
                if(random.Next(1,4) == 1)
                {
                    var num = random.Next(0, _specialChars.Length);
                    result += _specialChars[num];
                }
                else
                {
                    var num = random.Next(0, _chars.Length);
                    result += _chars[num];
                }
            }
            return result;
        }

        /// <summary>
        /// Takes in an encrypted pw and calculates its strength
        /// </summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        public static double CalculatePasswordStrength(byte[] encryptedPw)
        {
            var pw = ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(encryptedPw));
            double passwordStrengthValue = 1;

            if (!pw.Any(c => char.IsUpper(c)))
                passwordStrengthValue -= 0.25;
            if (!pw.Any(c => char.IsDigit(c)))
                passwordStrengthValue -= 0.25;
            if (pw.Length < 12)
                passwordStrengthValue -= 0.50;
            if (!WellKnownSpecialCharacters.ContainsSpecialCharacters(pw))
                passwordStrengthValue -= 0.25;

            // We do not want a "negative" value password strength
            if (passwordStrengthValue <= 0)
                passwordStrengthValue = 0.1;

            pw = string.Empty;
            return passwordStrengthValue;
        }

    }
}
