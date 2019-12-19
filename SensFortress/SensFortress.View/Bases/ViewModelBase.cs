using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SensFortress.View.Bases
{
    /// <summary>
    /// Base class for all ViewModels
    /// </summary>
    public class ViewModelBase : BindableBase
    {
        /// <summary>
        /// For communication with the current ManagementBase
        /// </summary>
        public ViewModelManagementBase CurrentBase { get; protected set; }

        public Guid Id { get; set; }

    }
}
