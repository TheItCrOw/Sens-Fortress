using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Models.ViewModels
{
    public class FortressViewModel
    {
        public FortressViewModel(string fullPath, DateTime created, DateTime modified)
        {
            FullName = fullPath;
            var splitedString = fullPath.Split("\\");
            Name = splitedString[splitedString.Length - 1];
            Created = created;
            Modified = modified;
            Information = $"{Modified.ToShortDateString()}: {Name}";
        }
        public string FullName { get; }
        public string Name { get; }
        public DateTime Created { get; }
        public DateTime Modified { get; }
        public string Information { get; set; }
    }
}
