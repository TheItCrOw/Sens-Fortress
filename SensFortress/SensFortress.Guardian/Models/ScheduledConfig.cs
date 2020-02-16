using SensFortress.Guardian.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Guardian.Models
{
    public class ScheduledConfig : GuardianTask
    {
        /// <summary>
        /// 0 = DateTime, 1 = interval, 2 = path (maybe)
        /// </summary>
        public object[] Parameters { get; set; }

        public ScheduledConfig(DateTime executionDate, object[] param)
        {
            Gtid = new GTIdentifier(executionDate, param[1]?.ToString());
            Parameters = param;
            Type = typeof(GuardianTask);
        }

    }
}
