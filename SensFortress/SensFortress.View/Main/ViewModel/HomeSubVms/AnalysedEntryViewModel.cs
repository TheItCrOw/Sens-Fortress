using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    /// <summary>
    /// Viewmodel for presenting the analysed passwords in UI
    /// </summary>
    public class AnalysedEntryViewModel
    {
        public DelegateCommand InspectResultCommand => new DelegateCommand(InspectResult);

        public string Category { get; set; }
        public string Name { get; set; }
        public double PasswordStrength { get; set; }

        /// <summary>
        /// Color mapped to PasswordStrength level
        /// </summary>
        public SolidColorBrush Color
        {
            get => PasswordStrength switch
                {
                    var x when x >= 70 && x <= 100 => Brushes.Lime,
                    var x when x < 70 && x >= 50 => Brushes.Orange,
                    var x when x < 50 => Brushes.Red,
                    _ => Brushes.White
                };
        }

        /// <summary>
        /// Inspects the analysis results in depth
        /// </summary>
        private void InspectResult()
        {

        }

    }
}
