using SensFortress.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Security
{
    /// <summary>
    /// Scanner class that checks different parts of the application for malicious actions.
    /// </summary>
    public class FortressScanner
    {
        /// <summary>
        /// Returns false if any changes were made to the fortress from outside.
        /// </summary>
        /// <returns></returns>
        public bool ScanFortressFile() =>
            SecurityParameterProvider.Instance.GetCurrentHashOf("Fortress") == ByteHelper.ReadHash(CurrentFortressData.FullPath) ? true : false;

        /// <summary>
        /// Returns false if any changes were made to the settings from outside.
        /// </summary>
        /// <returns></returns>
        public bool ScanSettings() =>
            SecurityParameterProvider.Instance.GetCurrentHashOf(nameof(Settings)) == ByteHelper.ReadHash(IOPathHelper.GetSettingsFile()) ? true : false;

    }
}
