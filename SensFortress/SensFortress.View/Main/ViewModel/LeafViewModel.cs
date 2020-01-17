using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    public class LeafViewModel : ViewModelBase
    {
        private string _name;
        private string _description;

        public LeafViewModel(Leaf model, ViewModelManagementBase currentBase)
        {
            Model = model;
            Id = model.Id;
            BranchId = model.BranchId;
            Name = model.Name;
            Description = model.Description;
            CurrentBase = currentBase;
        }
        public Guid BranchId { get; set; }
        public LeafPassword LeafPasswordCopy { get; set; }
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
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetProperty(ref _description, value);
            }
        }
    }
}
