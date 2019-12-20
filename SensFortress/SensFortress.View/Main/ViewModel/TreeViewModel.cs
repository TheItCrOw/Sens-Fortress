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
    public class TreeItemViewModel
    {
        private ViewModelBase _currentViewModel;
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

        public ObservableCollection<TreeItemViewModel> Children { get; set; } = new ObservableCollection<TreeItemViewModel>();
    }

    public enum TreeDepth
    {
        Root,
        Branch,
        Leaf
    }
}
