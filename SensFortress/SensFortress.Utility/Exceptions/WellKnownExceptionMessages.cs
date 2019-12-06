using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Utility.Exceptions
{
    /// <summary>
    /// Class that returns suitable messages for exceptions shown in the UI 
    /// </summary>
    public static class WellKnownExceptionMessages
    {

        /// <summary>
        /// Message when a data operation fails
        /// </summary>
        /// <returns></returns>
        public static string DataExceptionMessage() => "An error occured while trying to handle data safely. Please wait as the memory is being flushed to prevent any leaks.";

    }
}
