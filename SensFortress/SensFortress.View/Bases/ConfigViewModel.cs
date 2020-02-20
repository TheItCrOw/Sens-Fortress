using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Bases
{
    public class ConfigViewModel : ViewModelBase
    {
        /// <summary>
        /// Settings have unique names => this is better than Id's, since Id's are not being saved.
        /// </summary>
        public string Name { get; set; }

        public ConfigViewModel()
        {
            Id = new Guid();
        }
    }
}
