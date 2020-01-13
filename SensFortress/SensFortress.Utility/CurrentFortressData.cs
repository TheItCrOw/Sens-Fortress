using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SensFortress.Utility
{
    /// <summary>
    /// Holds information about the currently used fortress
    /// </summary>
    public static class CurrentFortressData
    {
        private static bool _isLocked;
        public delegate void FortressLockedStatusChangedEvent();
        /// <summary>
        /// Subscribe to get informed whenever the IsLocked status changes.
        /// </summary>
        public static event FortressLockedStatusChangedEvent FortressLockedStatusChanged;

        public static string FullPath { get; set; }

        public static string FortressName { get; set; }

        public static byte[] Salt { get; set; }

        public static bool IsLocked
        {
            get
            {
                return _isLocked;
            }
            set
            {
                _isLocked = value;
                FortressLockedStatusChanged?.Invoke();
            }
        }



    }
}
