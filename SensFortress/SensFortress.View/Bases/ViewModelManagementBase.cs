using Prism.Mvvm;
using SensFortress.Utility;
using SensFortress.View.Main.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Bases
{
    public abstract class ViewModelManagementBase : BindableBase
    {
        /// <summary>
        /// Determines whether the fortress is currently locked or not.
        /// </summary>
        public abstract bool IsLocked { get; set; }

        public ViewModelManagementBase() => CurrentFortressData.FortressLockedStatusChanged += IsLocked_Changed;        

        private void IsLocked_Changed() => IsLocked = CurrentFortressData.IsLocked;        

    }
}
