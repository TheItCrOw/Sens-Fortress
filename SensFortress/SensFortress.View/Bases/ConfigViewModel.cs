using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Bases
{
    public class ConfigViewModel : ViewModelBase
    {
        public string Name { get; set; }

        public ConfigViewModel()
        {
            Id = new Guid();
        }
    }
}
