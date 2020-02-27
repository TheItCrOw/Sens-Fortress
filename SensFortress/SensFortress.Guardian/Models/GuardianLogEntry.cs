using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Guardian.Models
{
    public class GuardianLogEntry
    {
        public GuardianLogEntry()
        {
            Id = new Guid();
        }
        public Guid Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EntryType LogType { get; set; }
        public DateTime Date { get; set; }
    }

    public enum EntryType
    {
        Success,
        Error,
        Danger
    }
}
