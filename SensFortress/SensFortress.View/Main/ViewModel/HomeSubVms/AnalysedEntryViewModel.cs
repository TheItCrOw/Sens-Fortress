using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    /// <summary>
    /// Viewmodel for presenting the analysed passwords in UI
    /// </summary>
    public class AnalysedEntryViewModel
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string PasswordStrength { get; set; }
    }
}
