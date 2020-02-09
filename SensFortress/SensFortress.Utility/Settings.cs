using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SensFortress.Utility
{
    /// <summary>
    /// Static class that keeps track of the users custom settings
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Call this method when a user has been logged in.
        /// </summary>
        public static void Initialize()
        {
            if(!File.Exists(IOPathHelper.GetSettingsFile()))
            {
                // Write the default settings file.
                IOPathHelper.CreateDirectory(IOPathHelper.GetConfigsDirectory());

                var xmlWriterSettings = new XmlWriterSettings { NewLineOnAttributes = true, Indent = true };
                using (XmlWriter writer = XmlWriter.Create(IOPathHelper.GetSettingsFile(), xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Settings");

                    // Create test settings
                    var settingsDict = new Dictionary<string, string>();
                    settingsDict.Add("BOOL_LockingIncludeQuickBar", "False");
                    settingsDict.Add("BOOL_AutomaticBackup", "True");
                    settingsDict.Add("STRING_AutomaticBackupIntervall", $"{DateTime.Now.Day}, daily");

                    foreach(var pair in settingsDict)
                    {
                        writer.WriteElementString(pair.Key, pair.Value);
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }

            //Testing
            SaveSetting("STRING_AuomaticScanIntervall", $"{DateTime.Now.Day}, weekly");
        }

        /// <summary>
        /// Takes in a setting name plus value and either updates an existing one or creates it as a new setting.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SaveSetting(string name, string value)
        {
            var setDoc = XDocument.Load(IOPathHelper.GetSettingsFile());
            var setDocElements = setDoc.Root.Elements();
            var foundEl = false;

            foreach(var element in setDocElements)
            {
                if (element.Name == name)
                {
                    element.Value = value;
                    foundEl = true;
                }
            }

            // If the setting doesnt exist yet in the file => add it
            if(!foundEl)
            {
                var root = setDoc.Root;
                root.Add(new XElement(name, value));
            }
            setDoc.Save(IOPathHelper.GetSettingsFile());
        }

    }
}
