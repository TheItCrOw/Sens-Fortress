using Prism.Commands;
using SensFortress.Utility;
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
        private HashSet<string> _resultTips;
        public AnalysedEntryViewModel(double passwordStrength, HashSet<string> resultTips)
        {
            PasswordStrength = passwordStrength * 100;
            _resultTips = resultTips;
        }
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
                    var x when x >= 70 && x <= 100 => Brushes.Lime, // green when 70-100
                    var x when x < 70 && x >= 50 => Brushes.Orange, //orange when 50 - 70
                    var x when x < 50 => Brushes.Red, // Red when 0-50
                    _ => Brushes.White // default
                };
        }

        /// <summary>
        /// Inspects the analysis results in depth
        /// </summary>
        private void InspectResult()
        {
            // If the list is empty - it's a good pw
            if (_resultTips.Count == 0)
            {
                _resultTips.Add("A good password.");
            }

            var fullString = string.Empty;
            foreach(var tip in _resultTips)
            {
                fullString += tip + Environment.NewLine + Environment.NewLine;
            }
            Communication.InformUser(fullString);
        }

    }
}
