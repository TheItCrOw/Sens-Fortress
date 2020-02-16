using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Guardian.Bases
{
    /// <summary>
    /// Base for exectuable tasks the guardian may take care of.
    /// </summary>
    public class GuardianTask
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GTIdentifier Gtid { get; set; }
        public Type Type { get; set; }
    }
}
