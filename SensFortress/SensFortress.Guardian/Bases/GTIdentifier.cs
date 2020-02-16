using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Guardian.Bases
{
    /// <summary>
    /// Guardian Task Identifier insteda of a simple Id.
    /// </summary>
    public class GTIdentifier
    {
        public GTIdentifier(DateTime exceutionDate, string interval)
        {
            Id = Guid.NewGuid();
            ExecutionDate = exceutionDate;
            Interval = interval;
        }
        public string Value { get => $"{Id}-{ExecutionDate}"; }
        public Guid Id { get; }
        public DateTime ExecutionDate { get; set; }
        public string Interval { get; }
    }
}
