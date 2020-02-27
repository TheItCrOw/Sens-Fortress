using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Utility.Exceptions
{
    public static class ExceptionExtension
    {
        private static string _userMessage;
        public static string SetUserMessage(this Exception ex, string message) => _userMessage = message;
        public static string GetUserMessage(this Exception ex) => _userMessage;
    }
}
