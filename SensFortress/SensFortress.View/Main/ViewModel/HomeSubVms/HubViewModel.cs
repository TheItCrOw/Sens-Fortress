using SensFortress.Models.Fortress;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    public class HubViewModel : ViewModelManagementBase
    {
        private bool _isLocked;
        public ObservableCollection<LeafViewModel> QuickBar { get; set; } = new ObservableCollection<LeafViewModel>();

        /// <summary>
        /// Determines whether the fortress is currently locked.
        /// </summary>
        public override bool IsLocked
        {
            get => _isLocked;
            set
            {
                SetProperty(ref _isLocked, value);
            }
        }

        public void Initialize()
        {
            Testing();
        }

        private void Testing()
        {
            var leaf = new Leaf{ Name="Password1", Description="This is a description", Username="Usernamexy"};
            var leafVm = new LeafViewModel(leaf, this);
            QuickBar.Add(leafVm);
            QuickBar.Add(leafVm);
            QuickBar.Add(leafVm);
            QuickBar.Add(leafVm);
            QuickBar.Add(leafVm);
            QuickBar.Add(leafVm);
            QuickBar.Add(leafVm);
            QuickBar.Add(leafVm);
        }

    }
}
