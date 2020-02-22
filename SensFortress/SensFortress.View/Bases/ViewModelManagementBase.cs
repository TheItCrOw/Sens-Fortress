using Prism.Mvvm;
using SensFortress.Guardian;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using SensFortress.View.Main.Views;
using SensFortress.View.TaskLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SensFortress.View.Bases
{
    public abstract class ViewModelManagementBase : BindableBase
    {
        /// <summary>
        /// Determines whether the fortress is currently locked or not.
        /// </summary>
        public abstract bool IsLocked { get; set; }

        public ViewModelManagementBase()
        {
            CurrentFortressData.FortressLockedStatusChanged += IsLocked_Changed;

            IsLocked = CurrentFortressData.IsLocked;
        }

        public void IsLocked_Changed() => IsLocked = CurrentFortressData.IsLocked;

    }
}
