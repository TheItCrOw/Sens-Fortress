using SensFortress.Models.BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace SensFortress.Models.Fortress
{
    /// <summary>
    /// Class representing all data needed to log into a webpage in Sen's Fortress Webbrowser
    /// </summary>
    public class Website : ModelBase
    {
        public Website(string adress)
        {
            Id = new Guid();
            Adress = new Uri(adress);
        }

        public Uri Adress { get; }

        /// <summary>
        /// Later I think I wanna save the whole image as bytes
        /// </summary>
        public string LogoSource { get; set; }

        public string Name { get; set; }

    }
}
