using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    public class SelectedLeafViewModel : ViewModelManagementBase
    {
        private TreeItemViewModel _currentIten;
        public TreeItemViewModel CurrentItem
        {
            get
            {
                return _currentIten;
            }
            set
            {
                SetProperty(ref _currentIten, value);
            }
        }
        public SelectedLeafViewModel(TreeItemViewModel selectedLeaf)
        {
            CurrentItem = selectedLeaf;
        }
    }
}
