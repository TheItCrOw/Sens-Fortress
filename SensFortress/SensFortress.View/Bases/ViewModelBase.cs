using Prism.Mvvm;
using SensFortress.Models.BaseClasses;
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

        /// <summary>
        /// Returns the Id.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Id}";
        }

    }
}
