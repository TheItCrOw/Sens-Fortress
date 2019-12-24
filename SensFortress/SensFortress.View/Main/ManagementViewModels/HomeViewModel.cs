using Prism.Commands;
using Prism.Mvvm;
using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Models.ViewModels;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    public class HomeViewModel : ViewModelManagementBase
    {
        private TreeItemViewModel _selectedTreeViewItem;
        /// <summary>
        /// Collection showing in the TreeView
        /// </summary>
        public ObservableCollection<TreeItemViewModel> RootNodes { get; set; } = new ObservableCollection<TreeItemViewModel>();
        public DelegateCommand<string> AddTreeItemCommand => new DelegateCommand<string>(AddTreeItem);
        public DelegateCommand EditTreeItemCommand => new DelegateCommand(EditTreeItem);

        /// <summary>
        /// Holds the currently selected item in the TreeView UI.
        /// </summary>
        public TreeItemViewModel SelectedTreeViewItem
        {
            get
            {
                return _selectedTreeViewItem;
            }
            set
            {
                SetProperty(ref _selectedTreeViewItem, value);
                UpdateRootNodes(true, false);
            }
        }

        public HomeViewModel()
        {
            try
            {
                LoadTreeView();
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while loading HomeView: {ex}");
                ex.SetUserMessage("An error occured while trying to load data.");
                Communication.InformUserAboutError(ex);
            }
        }

        /// <summary>
        /// Marks the TreeItem as editable
        /// </summary>
        private void EditTreeItem()
        {
            UpdateRootNodes(true, true);
        }

        /// <summary>
        /// Adds a new object to the currently selected treeItem
        /// </summary>
        private void AddTreeItem(string buttonName)
        {
            if (SelectedTreeViewItem == null)
                return;

            if(SelectedTreeViewItem.CurrentViewModel is BranchViewModel && SelectedTreeViewItem.MayHaveChildren)
            {
                if(buttonName == "AddBranchButton")
                {
                    var newBranch = new Branch{ Name="(new)", ParentBranchId = SelectedTreeViewItem.CurrentViewModel.Id};
                    var newBranchVm = new BranchViewModel(newBranch, this);
                    var newTreeViewItem = new TreeItemViewModel(newBranchVm, TreeDepth.Branch);
                    DataAccessService.Instance.AddOneToMemoryDC(newBranch); // Store the newly created model into the MemoryDc.
                    SelectedTreeViewItem.Children.Add(newTreeViewItem);
                    SelectedTreeViewItem = newTreeViewItem;
                }
                else if(buttonName == "AddLeafButton")
                {

                }
            }
        }

        /// <summary>
        /// Updates Properties of all items in the TreeView
        /// </summary>
        private void UpdateRootNodes(bool isSelected = false, bool isEditable = false)
        {
            foreach (var item in RootNodes)
            {
                UpdateRootNodes(item);
            }
            if (isSelected)
                SelectedTreeViewItem.IsSelected = true;
            if (isEditable)
                SelectedTreeViewItem.IsEditable = true;
            if (SelectedTreeViewItem.CurrentViewModel is BranchViewModel)
                SelectedTreeViewItem.MayHaveChildren = true;
        }

        private void UpdateRootNodes(TreeItemViewModel currentItem)
        {
            currentItem.IsSelected = false;
            currentItem.IsEditable = false;
            currentItem.MayHaveChildren = false;
            if (currentItem.Children.Count > 0)
                foreach (var child in currentItem.Children)
                {
                    child.IsSelected = false;
                    child.IsEditable = false;
                    child.MayHaveChildren = false;

                    UpdateRootNodes(child);
                }
        }

        /// <summary>
        /// Loads the treeview with all Branches and leafes.
        /// </summary>
        private void LoadTreeView()
        {
            Logger.log.Info("Loading Home TreeView...");
            var rootNodes = new List<BranchViewModel>();
            var allBranchesVm = DataAccessService.Instance
                .GetAll<Branch>()
                .Select(b => new BranchViewModel(b, this));

            var allLeafesVmLookup = DataAccessService.Instance
                .GetAll<Leaf>()
                .Select(l => new LeafViewModel(l, this))
                .ToLookup(l => l.BranchId, l => l);

            var allBranchesVmLookup = allBranchesVm.ToLookup(b => b.ParentBranchId, b => b);

            RootNodes.Clear();

            foreach (var branch in allBranchesVm)
            {
                var currentItem = new TreeItemViewModel(branch, TreeDepth.Branch);

                // If it's a root branch, just add it
                if (branch.ParentBranchId == Guid.Empty)
                {
                    currentItem.TreeType = TreeDepth.Root;
                    foreach (var leaf in GetLeafes(allLeafesVmLookup, branch.Id))
                        currentItem.Children.Add(leaf);

                    RootNodes.Add(currentItem);
                }
                // Find the sub branches and add it as a children
                if (allBranchesVmLookup.Contains(branch.Id))
                {
                    var subbranches = allBranchesVmLookup.FirstOrDefault(b => b.Key == branch.Id);
                    foreach (var subbranch in subbranches)
                    {
                        var currentSubItem = new TreeItemViewModel(subbranch, TreeDepth.Branch);
                        // Add leafes to the subBranch
                        foreach (var leaf in GetLeafes(allLeafesVmLookup, subbranch.Id))
                            currentSubItem.Children.Add(leaf);
                        // Add the subBranch to the curretn branch.
                        currentItem.Children.Add(currentSubItem);
                    }
                }
            }
            Logger.log.Info("TreeView loaded!");
        }

        /// <summary>
        /// Returns a list of possible leafes of given branchId.
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        private List<TreeItemViewModel> GetLeafes(ILookup<Guid, LeafViewModel> allLeafesVmLookup, Guid branchId)
        {
            var returnList = new List<TreeItemViewModel>();

            // Add maybe leafes first.
            if (allLeafesVmLookup.Contains(branchId))
            {
                var leafes = allLeafesVmLookup.FirstOrDefault(b => b.Key == branchId);
                returnList.AddRange(leafes.Select(l => new TreeItemViewModel(l, TreeDepth.Leaf)));
            }
            return returnList;
        }

        #region Testing
        //=> testing
        //private void Testing()
        //{
        //    var testVM = new TestViewModel { Name = "Projects", TreeType = TreeDepth.Root };
        //    testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
        //    testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
        //    testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
        //    testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
        //    testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
        //    testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });

        //    var testVM2 = new TestViewModel { Name = "Projects", TreeType = TreeDepth.Root };
        //    var child = new TestViewModel { Name = "SubSubProjects", TreeType = TreeDepth.Leaf };
        //    testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch, Children = new ObservableCollection<TestViewModel> { child } });
        //    testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch, Children = new ObservableCollection<TestViewModel> { child } });
        //    testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch, Children = new ObservableCollection<TestViewModel> { child } });
        //    testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });


        //    TestCollection.Add(testVM);
        //    TestCollection.Add(testVM2);
        //}

        //public ObservableCollection<TestViewModel> TestCollection { get; set; } = new ObservableCollection<TestViewModel>();

        //<= testing
        #endregion
    }
}

