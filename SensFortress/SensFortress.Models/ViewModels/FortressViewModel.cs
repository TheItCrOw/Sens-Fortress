using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Models.ViewModels
{
    public class FortressViewModel
    {

        public FortressViewModel(string fullPath)
        {
            FullName = fullPath;
            var splitedString = fullPath.Split("\\");
            Name = splitedString[splitedString.Length - 1];
        }

        public string FullName { get; }

        public string Name { get; }

    }
}
