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
        public BranchViewModel(Branch model)
        {
            Id = model.Id;
            ParentBranchId = model.ParentBranchId;
            Name = model.Name;
        }
        public Guid ParentBranchId { get; set; }
        public string Name { get; set; }

    }
}
