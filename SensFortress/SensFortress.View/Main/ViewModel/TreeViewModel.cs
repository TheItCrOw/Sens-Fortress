using SensFortress.Models.BaseClasses;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    /// <summary>
    /// Represents a modelbase in the TreeView.
    /// </summary>
    public class TreeItemViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private bool _isSelected;

        public TreeItemViewModel(ViewModelBase model, TreeDepth type)
        {
            _currentViewModel = model;
            ChildrenType = type;
            if (model is BranchViewModel branch)
                Name = branch.Name;
            else if (model is LeafViewModel leaf)
                Name = leaf.Name;
        }
        public TreeDepth ChildrenType { get; set; }
        public string Name { get; set; }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }
        public ObservableCollection<TreeItemViewModel> Children { get; set; } = new ObservableCollection<TreeItemViewModel>();
    }

    public enum TreeDepth
    {
        Root,
        Branch,
        Leaf
    }
}
