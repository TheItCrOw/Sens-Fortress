using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Utility
{
    /// <summary>
    /// Helper class for working with DateTimes
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Creates a new datetime out of a date and timeOfDay.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeOfDay"></param>
        /// <returns></returns>
        public static DateTime CreateNew(DateTime date, DateTime timeOfDay) => new DateTime(date.Year, date.Month, date.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second); 

    }
}
