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
        private string _userName;

        public LeafViewModel(Leaf model, ViewModelManagementBase currentBase)
        {
            Model = model;
            Id = model.Id;
            BranchId = model.BranchId;
            Name = model.Name;
            Description = model.Description;
            CurrentBase = currentBase;
            Username = model.Username;
        }
        public Guid BranchId { get; set; }
        public LeafPassword LeafPasswordCopy { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
            }
        }
        public string Username
        {
            get => _userName;            
            set
            {
                SetProperty(ref _userName, value);
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
            }
        }
    }
}
