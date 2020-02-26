using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    public class SecurityManagementViewModel : ViewModelManagementBase
    {
        private bool _isLocked;

        public override bool IsLocked 
        { 
            get => _isLocked; 
            set => SetProperty(ref _isLocked, value);
        }

        /// <summary>
        /// Use this instead of ctor for more control over when it's called.
        /// </summary>
        public void Initialize()
        {

        }
    }
}
