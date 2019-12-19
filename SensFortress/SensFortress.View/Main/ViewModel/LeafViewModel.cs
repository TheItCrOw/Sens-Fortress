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
        public LeafViewModel(Leaf model)
        {
            Id = model.Id;
            BranchId = model.BranchId;
            Name = model.Name;
            Description = model.Description;
        }
        public Guid BranchId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
    }
}
