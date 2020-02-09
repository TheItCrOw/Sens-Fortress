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
            var log4netconfig = GetLogConfigAndSetCustomSettings();
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netconfig["log4net"]);
            log.Info("        ==========================  Started Logging  ==========================        ");
        }

        /// <summary>
        /// Gets the log Instance
        /// </summary>
        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get the logConfig and sets the custom settings
        /// </summary>
        /// <param name="logConfig"></param>
        private static XmlDocument GetLogConfigAndSetCustomSettings()
        {
            // Open the logConfig
            using (FileStream fs = new FileStream(IOPathHelper.Getlog4netConfigFile(), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                XmlDocument log4netconfig = new XmlDocument();
                log4netconfig.Load(fs);
                var logPathNode = log4netconfig.SelectSingleNode("log4net/appender/file");
                logPathNode.InnerXml = IOPathHelper.GetLogFileDirectory();
                fs.SetLength(0);
                log4netconfig.Save(fs);
                return log4netconfig;
            }
        }

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
