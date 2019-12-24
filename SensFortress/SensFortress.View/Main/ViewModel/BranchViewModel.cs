using SensFortress.Models.Fortress;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    public class BranchViewModel : ViewModelBase
    {
        public BranchViewModel(Branch model, ViewModelManagementBase currentBase)
        {
            Id = model.Id;
            ParentBranchId = model.ParentBranchId;
            Name = model.Name;
            CurrentBase = currentBase;
        }
        public Guid ParentBranchId { get; set; }
        public string Name { get; set; }

    }
}
