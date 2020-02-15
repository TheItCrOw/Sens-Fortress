using SensFortress.Guardian.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Guardian.Models
{
    public class ScheduledConfig : GuardianTask
    {
        public string Name { get; set; }

        /// <summary>
        /// 0 = DateTime, 1 = interval, 2 = path (maybe)
        /// </summary>
        public object[] Parameters { get; set; }

        public ScheduledConfig(DateTime executionDate)
        {
            Gtid = new GTIdentifier(executionDate);
            Type = typeof(GuardianTask);
        }

    }
}
