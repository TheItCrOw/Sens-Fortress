using SensFortress.Models.Fortress;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    public class WebsiteViewModel : ViewModelBase
    {
        public WebsiteViewModel(Website website, ViewModelManagementBase currentBase)
        {
            Id = new Guid();
            CurrentBase = currentBase;
            Model = website;
            Name = website.Name;
            Adress = website.Adress;
            LogoSource = website.LogoSource;
        }

        public string Name { get; }

        public Uri Adress { get; }

        public string LogoSource { get; }

        public bool IsSelected { get; set; }

    }
}
