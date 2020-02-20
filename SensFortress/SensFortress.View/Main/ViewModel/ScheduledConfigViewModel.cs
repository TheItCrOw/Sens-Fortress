using SensFortress.Guardian.Models;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    /// <summary>
    /// ViewModel for scheduledConfigs executed by the guardian
    /// </summary>
    public class ScheduledConfigViewModel : ConfigViewModel
    {
        private string _nextExecution;
        private string _description;

        public ScheduledConfigViewModel(ScheduledConfig model, ViewModelManagementBase cBase)
        {
            CurrentBase = cBase;
            Name = model.Name;
            Description = model.Description;

            UpdateNextExecution(model.Gtid.ExecutionDate);
        }

        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
            }
        }

        public string NextExecution
        {
            get => _nextExecution;
            set
            {
                SetProperty(ref _nextExecution, value);
            }
        }

        public void UpdateNextExecution(DateTime date)
        {
            if (date.Date <= DateTime.Now.Date)
                NextExecution = $"Scheduled today at {date.TimeOfDay}";
            else
                NextExecution = $"Next execution: {date}";
        }
    }
}
