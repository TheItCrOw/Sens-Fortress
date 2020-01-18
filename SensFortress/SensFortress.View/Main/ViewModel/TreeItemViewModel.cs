using Prism.Commands;
using Prism.Mvvm;
using SensFortress.Models.BaseClasses;
using SensFortress.View.Bases;
using SensFortress.View.TaskLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
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
        private bool _setInitialProperties = false;

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
            {
                IsDirty = true;
                TaskLogger.Instance.Track($"{Name} has been created!");
            }
            else
            {
                IsDirty = false;
            }
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
                HandleChangeableProperties(nameof(Name), Name);
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

        /// <summary>
        /// This informs the model that is being saved in the end about changes made in the UI.
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="change"></param>
        private void UpdateChangesToModel(string propName, object change)
        {
            var type = CurrentViewModel.Model.GetType();
            var props = new List<PropertyInfo>(type.GetProperties());

            foreach (var prop in props)
            {
                if (prop.Name == propName)
                {
                    var from = prop.GetValue(CurrentViewModel.Model);
                    // When no change happened return.
                    if (from == change)
                        return;

                    IsDirty = true;
                    ((HomeViewModel)CurrentViewModel.CurrentBase).ChangesTracker++;
                    TaskLogger.Instance.Track($"{Name}: Changed the {prop.Name} from {from} to {change}.");
                    prop.SetValue(CurrentViewModel.Model, change);
                }
            }
        }

        /// <summary>
        /// Handles flags and updating of the properties to model.
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="change"></param>
        public void HandleChangeableProperties(string propName, object change)
        {
            if (!_setInitialProperties)
            {
                _setInitialProperties = true;
            }
            else
            {
                UpdateChangesToModel(propName, change);
            }
        }

    }

    public enum TreeDepth
    {
        Root,
        Branch,
        Leaf
    }
}
