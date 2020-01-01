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
        private bool _mayHaveChildren;
        private bool _isExpanded;
        private bool _isHighlighted;
        private string _name;
        private bool _isDirty;

        public ObservableCollection<TreeItemViewModel> Children { get; set; } = new ObservableCollection<TreeItemViewModel>();
        public DelegateCommand EditTreeItemCommand => new DelegateCommand(((HomeViewModel)CurrentViewModel.CurrentBase).EditTreeItemCommand.Execute);
        public DelegateCommand<string> AddTreeItemCommand => new DelegateCommand<string>(((HomeViewModel)CurrentViewModel.CurrentBase).AddTreeItemCommand.Execute);
        public DelegateCommand DeleteTreeItemCommand => new DelegateCommand(((HomeViewModel)CurrentViewModel.CurrentBase).DeleteTreeItemCommand.Execute);
        public TreeItemViewModel(ViewModelBase modelVm, TreeDepth type, bool isNew = false)
        {
            CurrentViewModel = modelVm;
            TreeType = type;
            if (modelVm is BranchViewModel branch)
                Name = branch.Name;
            else if (modelVm is LeafViewModel leaf)
                Name = leaf.Name;

            if (isNew) // If the item has been newly created - then it's saveable.
                IsDirty = true;
            else
                IsDirty = false;
        }
        public TreeDepth TreeType { get; set; }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
                IsDirty = true;
            }
        }
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
        public bool MayHaveChildren
        {
            get
            {
                return _mayHaveChildren;
            }
            set
            {
                SetProperty(ref _mayHaveChildren, value);
            }
        }
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (Children.Count > 0)
                    SetProperty(ref _isExpanded, value);
            }
        }
        public bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                SetProperty(ref _isHighlighted, value);
            }
        }
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                SetProperty(ref _isDirty, value);
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
