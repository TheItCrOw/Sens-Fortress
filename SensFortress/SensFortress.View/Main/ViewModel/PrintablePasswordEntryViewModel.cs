using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    /// <summary>
    /// ViewModel that wraps category, name and acutal password
    /// </summary>
    public class PrintablePasswordEntryViewModel
    {
        public PrintablePasswordEntryViewModel(string name, string category, string username, string password)
        {
            Name = name;
            Category = category;
            Username = username;
            Password = password;
        }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
