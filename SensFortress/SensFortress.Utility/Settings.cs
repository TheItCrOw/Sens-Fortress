using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

/* It's crucial, that anything that is being read or loaded into the fortress from outside it's gate is valid data and not modified malicious code.
 * In order to save settings, we need to check the insides of each file with the WellKnownSettings Dict and the SettingType enumeration.
 * If anything is out of order => inform the user and handle the situation accordingly.
 * 
 * Convention for setting entry:                Name                           Value (must match SettingType)
 *                              [SettingType.ToString()]_[PascalCase] | part1, part2, part3
 * If the value of a setting is 'Void', it has not yet a value, which is fine.                              
 * */
namespace SensFortress.Utility
{
    /// <summary>
    /// Static class that keeps track of the users custom settings
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Contains all registered settings.
        /// </summary>
        private static Dictionary<string, SettingType> _wellKnownSettings = new Dictionary<string, SettingType>
        {
            { "B_LockingIncludeQuickBar", SettingType.B }, { "B_AutomaticBackup", SettingType.B }, { "DI_AutomaticBackupIntervall", SettingType.DI },
        };

        /// <summary>
        /// The default settings that are being written when the setting dont exist
        /// </summary>
        private static Dictionary<string, string> _defaultSettings = new Dictionary<string, string>
        {
            { "B_LockingIncludeQuickBar", "False"}, {"B_AutomaticBackup", "True"}, { "DI_AutomaticBackupIntervall", "Void"}
        };

        /// <summary>
        /// Call this method when a user has been logged in.
        /// </summary>
        public static void Initialize()
        {
            if (!File.Exists(IOPathHelper.GetSettingsFile()))
            {
                // Write the default settings file.
                IOPathHelper.CreateDirectory(IOPathHelper.GetConfigsDirectory());

                var xmlWriterSettings = new XmlWriterSettings { NewLineOnAttributes = true, Indent = true };
                using (XmlWriter writer = XmlWriter.Create(IOPathHelper.GetSettingsFile(), xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Settings");

                    foreach (var pair in _defaultSettings)
                    {
                        writer.WriteElementString(pair.Key, pair.Value);
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        /// <summary>
        /// Takes in a setting name plus value and either updates an existing one or creates it as a new setting.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SaveSetting(string name, string value)
        {
            // Do something later if the setting is not valid. An empty value is fine.
            if (value != "Void" && !ValidateSetting(name, value))
                return;

            var setDoc = XDocument.Load(IOPathHelper.GetSettingsFile());
            var setDocElements = setDoc.Root.Elements();

            foreach (var element in setDocElements)
            {
                if (element.Name == name)
                {
                    element.Value = value;
                }
            }

            setDoc.Save(IOPathHelper.GetSettingsFile());
        }

        /// <summary>
        /// Gets the value of the given setting name. Returns null if not found or invalid. Returns Void if setting has no value right now.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetSettingValue(string name)
        {
            if (!_wellKnownSettings.ContainsKey(name))
                return null;

            var setDoc = XDocument.Load(IOPathHelper.GetSettingsFile());
            var setDocElements = setDoc.Root.Elements();

            foreach (var element in setDocElements)
            {
                if (element.Name == name)
                {
                    if (element.Value == "Void")
                        return "Void";

                    if (ValidateSetting(name, element.Value))
                        return element.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Validates the setting with the <see cref="_wellKnownSettings"/> and <see cref="SettingType"/>
        /// </summary>
        /// <returns></returns>
        private static bool ValidateSetting(string name, string value)
        {
            // First check if the given setting exists in the Dictionary
            if (_wellKnownSettings.TryGetValue(name, out var settingType))
            {
                // splited[0] should contain type => if it equals with settingtype, move on
                var splited = name.Split('_');
                if (splited[0] == settingType.ToString())
                {
                    // Now check the value => if it fits the according settingType format, continue
                    if (SettingTypeMatchesValue(settingType, value))
                    {
                        // If we reach this, the setting is valid.
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the given value matches the settingType format.
        /// </summary>
        /// <param name="settingType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool SettingTypeMatchesValue(SettingType settingType, string value)
        {
            switch (settingType)
            {
                // SettingType.B can only be true or false.
                case SettingType.B:
                    if (value != "True" || value != "False")
                    {
                        return false;
                    }
                    return true; // If we reach here => setting is valid

                // SettingType.D can only be one exact date
                case SettingType.D:
                    if (!DateTime.TryParseExact(value, "g", new CultureInfo("en-US"), DateTimeStyles.None, out var date))
                    {
                        return false;
                    }
                    return true; // If we reach here => setting is valid

                // SettingType.DI can only be a date followed by a SettingInterval
                case SettingType.DI:
                    var splited = value.Split(',');
                    if (!DateTime.TryParseExact(splited[0], "g", new CultureInfo("en-US"), DateTimeStyles.None, out var date2))
                    {
                        return false;
                    }

                    if (splited[1] != SettingInterval.Hourly.ToString() ||
                        splited[1] != SettingInterval.Daily.ToString() ||
                        splited[1] != SettingInterval.Weekly.ToString() ||
                        splited[1] != SettingInterval.Monthly.ToString() ||
                        splited[1] != SettingInterval.Yearly.ToString())
                    {
                        return false;
                    }
                    return true; // If we reach here => setting is valid

                // SettingType.DIP can only be date followed by settingInterval finished by an absolute path.
                case SettingType.DIP:
                    var splited2 = value.Split(',');
                    if (!DateTime.TryParseExact(splited2[0], "g", new CultureInfo("en-US"), DateTimeStyles.None, out var date3))
                    {
                        return false;
                    }

                    if (splited2[1] != SettingInterval.Hourly.ToString() ||
                        splited2[1] != SettingInterval.Daily.ToString() ||
                        splited2[1] != SettingInterval.Weekly.ToString() ||
                        splited2[1] != SettingInterval.Monthly.ToString() ||
                        splited2[1] != SettingInterval.Yearly.ToString())
                    {
                        return false;
                    }

                    if (!Uri.IsWellFormedUriString(splited2[2], UriKind.Absolute))
                    {
                        return false;
                    }
                    return true; // If we reach here => setting is valid

                default:
                    return false;
            }
        }
    }

    public enum SettingInterval
    {
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    /// <summary>
    /// Enum that contains all types of settings
    /// </summary>
    internal enum SettingType
    {
        [Description("BOOLEAN value")]
        B,
        [Description("DATETIME value")]
        D,
        [Description("Perform an action at DATETIME in a specific INTERVAL")]
        DI,
        [Description("Perform an action at DATETIME in a specific INTERVAL with given PATH")]
        DIP
    }
}
