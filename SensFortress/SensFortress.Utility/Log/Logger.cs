using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SensFortress.Utility.Log
{
    /// <summary>
    /// Wrapper for the log4net log. 
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Initializes the Log.
        /// </summary>
        internal static void EnsureLogger()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("E://WPF Projects//Sens-Fortress//SensFortress//SensFortress.Utility//Log//log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
            log.Info("        ==========================  Started Logging  ==========================        ");

        }

        /// <summary>
        /// Writes a statement into the log file
        /// </summary>
        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    }

    /// <summary>
    /// Helper class that initializes the Logger internally.
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// Call internal method from the static Logger class.
        /// </summary>
        public void InitializeLogger()
        {
            Logger.EnsureLogger();
        }

    }
}
