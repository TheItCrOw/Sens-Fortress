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
        private string _name;

        public BranchViewModel(Branch model, ViewModelManagementBase currentBase)
        {
            Model = model;
            Id = model.Id;
            ParentBranchId = model.ParentBranchId;
            Name = model.Name;
            CurrentBase = currentBase;
        }
        public Guid ParentBranchId { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

    }
}
