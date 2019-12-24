using Prism.Commands;
using Prism.Mvvm;
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
    public class TreeItemViewModel : BindableBase
    {
        private bool _isSelected;
        private bool _isEditable;
        public ObservableCollection<TreeItemViewModel> Children { get; set; } = new ObservableCollection<TreeItemViewModel>();
        public DelegateCommand EditTreeItemCommand => new DelegateCommand(((HomeViewModel)CurrentViewModel.CurrentBase).EditTreeItemCommand.Execute);
        public DelegateCommand AddTreeItemCommand => new DelegateCommand(((HomeViewModel)CurrentViewModel.CurrentBase).AddTreeItemCommand.Execute);

        public TreeItemViewModel(ViewModelBase model, TreeDepth type)
        {
            CurrentViewModel = model;            
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
        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                SetProperty(ref _isEditable, value);
            }
        }
        public ViewModelBase CurrentViewModel { get; }

    }

    public enum TreeDepth
    {
        Root,
        Branch,
        Leaf
    }
}
