using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace SensFortress.Utility
{
    /// <summary>
    /// Determines the type of the logged message.
    /// </summary>
    public enum LogType
    {
        Info,
        Error,
        CriticalError
    }

    /// <summary>
    /// Base of every Logger
    /// </summary>
    public abstract class LogBase
    {
        protected readonly object lockObj = new Object();
        public abstract void Log(LogType logType, string message);
    }

    /// <summary>
    /// Writes message into the log file
    /// </summary>
    public class FileLogger : LogBase
    {
        public string filePath = DirectoryHelper.GetLogDirectory();
        public override void Log(LogType logType, string message)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine($"[{logType.ToString()}]:  {message}");
                streamWriter.Close();
            }
        }
    }

    /// <summary>
    /// Logs a message under the given <see cref="LogType"/>
    /// </summary>
    public static class LogHelper
    {
        private static LogBase logger = null;
        public static void Log(LogType logType, string message)
        {
            logger = new FileLogger();
            logger.Log(logType, message);
        }
    }
}
