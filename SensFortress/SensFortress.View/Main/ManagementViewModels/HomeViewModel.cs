﻿using Prism.Commands;
using Prism.Mvvm;
using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Models.ViewModels;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using SensFortress.View.Main.ViewModel.HomeSubVms;
using SensFortress.View.Main.Views;
using SensFortress.View.Main.Views.HomeSubViews;
using SensFortress.View.TaskLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SensFortress.View.Main.ViewModel
{
    public class HomeViewModel : ViewModelManagementBase
    {
        private TreeItemViewModel _selectedTreeViewItem;
        private bool _isLoading;
        private int _changesTracker;
        private bool _scrollToBottom;
        private object _selectedContent;
        private bool _isLocked;
        private bool _showLockCard;

        /// <summary>
        /// Collection showing in the TreeView
        /// </summary>
        public ObservableCollection<TreeItemViewModel> RootNodes { get; set; } = new ObservableCollection<TreeItemViewModel>();
        /// <summary>
        /// List that tracks the actions don ein the TreeView
        /// </summary>
        public ObservableCollection<string> TaskLogs { get; set; } = new ObservableCollection<string>();
        public DelegateCommand<string> AddTreeItemCommand => new DelegateCommand<string>(AddTreeItem);
        public DelegateCommand EditTreeItemCommand => new DelegateCommand(EditTreeItem);
        public DelegateCommand DeleteTreeItemCommand => new DelegateCommand(DeleteTreeItem);
        public DelegateCommand SaveTreeChangesCommand => new DelegateCommand(SaveTreeChanges);
        public DelegateCommand LockUnlockFortressCommand => new DelegateCommand(LockUnlockFortress);

        /// <summary>
        /// Holds the currently selected item in the TreeView UI.
        /// </summary>
        public TreeItemViewModel SelectedTreeViewItem
        {
            get => _selectedTreeViewItem;
            set
            {
                SetProperty(ref _selectedTreeViewItem, value);
                UpdateRootNodes(true, false);
                UpdateContent();
            }
        }
        /// <summary>
        /// Is bound to the progressBar in UI.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
            }
        }
        /// <summary>
        /// Makes the scrollviewer of the TaskLog scroll to the bottom.
        /// </summary>
        public bool ScrollToBottom
        {
            get => _scrollToBottom;
            set
            {
                SetProperty(ref _scrollToBottom, value);
            }
        }
        /// <summary>
        /// Holds the view that is being shown in the middle menu.
        /// </summary>
        public object SelectedContent
        {
            get => _selectedContent;
            set
            {
                SetProperty(ref _selectedContent, value);
            }
        }
        /// <summary>
        /// Keeps track, of how many changes have been made, that are savable.
        /// </summary>
        public int ChangesTracker
        {
            get => _changesTracker;
            set
            {
                SetProperty(ref _changesTracker, value);
            }
        }
        /// <summary>
        /// Determines whether to show or not show the lock card.
        /// </summary>
        public bool ShowLockCard
        {
            get => _showLockCard;
            set
            {
                SetProperty(ref _showLockCard, value);
            }
        }

        public override bool IsLocked
        {
            get => _isLocked;
            set
            {
                SetProperty(ref _isLocked, value);
            }
        }

        public HomeViewModel()
        {
            try
            {
                TaskLogs.CollectionChanged += TaskLogs_CollectionChanges;
                TaskLogger.Instance.SetHomeView(this);
                LoadTreeView();
                TaskLogger.Instance.Track("Fortress has been built!");
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while loading HomeView: {ex}");
                ex.SetUserMessage("An error occured while trying to load data.");
                Communication.InformUserAboutError(ex);
            }
        }

        /// <summary>
        /// Shows the dialog card to lock the fortress
        /// </summary>
        private void LockUnlockFortress()
        {
            if (CurrentFortressData.IsLocked)
                ShowLockCard = true;
            else
                CurrentFortressData.IsLocked = true;
        }

        /// <summary>
        /// Handles switching of content assigned to the selected model.
        /// </summary>
        private void UpdateContent()
        {
            if (SelectedTreeViewItem.TreeType == TreeDepth.Root || SelectedTreeViewItem.TreeType == TreeDepth.Branch)
            {

            }
            else if (SelectedTreeViewItem.TreeType == TreeDepth.Leaf)
            {
                var leafView = new SelectedLeafView();
                leafView.DataContext = new SelectedLeafViewModel(SelectedTreeViewItem);
                SelectedContent = leafView;
            }
        }

        /// <summary>
        /// Event gets triggered when taskLog gets a new entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskLogs_CollectionChanges(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Go to the bottom once, then set the behaviour to be false again. Otherwise it's a bit buggy with the header...
            ScrollToBottom = true;
            ScrollToBottom = false;
        }

        /// <summary>
        /// Saves changes made in the TreeView.
        /// Mainly Models added, changed or deleted. We do it async to stay responsive.
        /// </summary>
        private async void SaveTreeChanges()
        {
            IsLoading = true;
            try
            {
                await Task.Run(() =>
                {
                    // Make sure all changes are being saved.
                    foreach (var node in RootNodes)
                    {
                        RecursivlySaveChanges(node);
                    }
                    // UI stuff has to be called from Dispatcher - WPF is a bit autistic here. 
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var saveView = new SaveFortressView();
                        saveView.ShowDialog();

                        // Saving was successfull
                        if (saveView.DialogResult == true)
                        {
                            ChangesTracker = 0;
                            UpdateRootNodes(false, false, true); // reset isDirty state
                            TaskLogger.Instance.Track($"Fortress has been saved.");
                        }
                        else // it was canceled.
                            return;
                    });
                });
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                ex.SetUserMessage("An error occured while trying to save the fortress.");
                Communication.InformUserAboutError(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Replaces the old models in the DC with the dirty models
        /// </summary>
        /// <param name="item"></param>
        private void RecursivlySaveChanges(TreeItemViewModel item)
        {
            if (item.IsDirty)
            {
                DataAccessService.Instance.DeleteOneFromMemoryDC(item.CurrentViewModel.Model);
                DataAccessService.Instance.AddOneToMemoryDC(item.CurrentViewModel.Model);
            }

            if (item.Children.Count > 0)
                foreach (var child in item.Children)
                    RecursivlySaveChanges(child);
        }

        /// <summary>
        /// Deletes the currently selected treeItem.
        /// Could take a while with great amount of data, thats why we do it async.
        /// </summary>
        private async void DeleteTreeItem()
        {
            IsLoading = true;
            try
            {
                await Task.Run(() =>
                {
                    if (SelectedTreeViewItem.Children.Count == 0)
                    {
                        DataAccessService.Instance.DeleteOneFromMemoryDC(SelectedTreeViewItem.CurrentViewModel.Model);
                        TaskLogger.Instance.Track($"{SelectedTreeViewItem.Name} has been deleted.");

                        foreach (var node in RootNodes)
                            DeleteItemFromParentChildren(SelectedTreeViewItem, node);

                        ChangesTracker++;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => ExpandAndHighlightAllChildren(SelectedTreeViewItem));
                        if (Application.Current.Dispatcher.Invoke(() => Communication.AskForAnswer("All highlighted items will be deleted.")))
                        {
                            TaskLogger.Instance.Track($"{SelectedTreeViewItem.Name} has been deleted.");

                            foreach (var child in SelectedTreeViewItem.Children)
                                DeleteAllChildren(child);

                            if (SelectedTreeViewItem.CurrentViewModel is BranchViewModel branchVm)
                            {
                                // If its a root, just delete it.
                                if (branchVm.ParentBranchId == Guid.Empty)
                                {
                                    Application.Current.Dispatcher.Invoke(() => RootNodes.Remove(SelectedTreeViewItem));
                                    return;
                                }

                                // if its not a root, then delete the item from its parent.
                                foreach (var node in RootNodes)
                                    DeleteItemFromParentChildren(SelectedTreeViewItem, node);
                            }
                        }
                        else
                        {
                            UpdateRootNodes(true, false);
                            IsLoading = false;
                            return;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ex.SetUserMessage("There was an error while trying to delete the item.");
                Logger.log.Error(ex);
                Communication.InformUserAboutError(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Recursivly deletes all children.
        /// </summary>
        /// <param name="currentItem"></param>
        private void DeleteAllChildren(TreeItemViewModel currentItem)
        {
            DataAccessService.Instance.DeleteOneFromMemoryDC(currentItem.CurrentViewModel.Model);
            TaskLogger.Instance.Track($"{currentItem.Name} has been deleted.");
            ChangesTracker++;

            if (currentItem.Children.Count > 0)
            {
                foreach (var child in currentItem.Children)
                {
                    DeleteAllChildren(child);
                }
            }
        }

        /// <summary>
        /// Expands an highlights all children and children children of the given TreeItem.
        /// </summary>
        /// <param name="currentItem"></param>
        private void ExpandAndHighlightAllChildren(TreeItemViewModel currentItem)
        {
            if (currentItem.Children.Count == 0)
                return;

            currentItem.IsHighlighted = true;
            currentItem.IsExpanded = true;

            foreach (var child in currentItem.Children)
            {
                child.IsExpanded = true;
                child.IsHighlighted = true;
                ExpandAndHighlightAllChildren(child);
            }
        }

        /// <summary>
        /// Deletes the given Item from its parent children
        /// </summary>
        /// <param name="deletableItem"></param>
        /// <param name="currentNode"></param>
        private void DeleteItemFromParentChildren(TreeItemViewModel deletableItem, TreeItemViewModel currentNode)
        {
            if (currentNode.Children.Count == 0)
                return;

            var foundItem = false;

            foreach (var child in currentNode.Children)
            {
                if (child == deletableItem)
                    foundItem = true;
                else
                    DeleteItemFromParentChildren(deletableItem, child);
            }

            if (foundItem)
                Application.Current.Dispatcher.Invoke(() => currentNode.Children.Remove(deletableItem));
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

            if (SelectedTreeViewItem.CurrentViewModel is BranchViewModel && SelectedTreeViewItem.MayHaveChildren)
            {
                if (buttonName == "AddBranchButton")
                {
                    var newBranch = new Branch { Name = "(new)", ParentBranchId = SelectedTreeViewItem.CurrentViewModel.Id };
                    var newBranchVm = new BranchViewModel(newBranch, this);
                    var newTreeViewItem = new TreeItemViewModel(newBranchVm, TreeDepth.Branch, true);
                    DataAccessService.Instance.AddOneToMemoryDC(newBranch); // Store the newly created model into the MemoryDc.
                    SelectedTreeViewItem.Children.Add(newTreeViewItem);
                    SelectedTreeViewItem.IsExpanded = true;
                }
                else if (buttonName == "AddLeafButton")
                {
                    var newLeaf = new Leaf { Name = "(new)", BranchId = SelectedTreeViewItem.CurrentViewModel.Id, Description = "This is a new password!" };
                    var exPw = ByteHelper.StringToByteArray("(new password)");
                    var newLeafPw = new LeafPassword{ForeignId=newLeaf.Id, Value = exPw};
                    var newLeafVm = new LeafViewModel(newLeaf, this);
                    var newTreeItem = new TreeItemViewModel(newLeafVm, TreeDepth.Leaf, true);
                    DataAccessService.Instance.AddOneToMemoryDC(newLeaf);
                    DataAccessService.Instance.AddOneToMemoryDC(null, true, newLeafPw);
                    SelectedTreeViewItem.Children.Add(newTreeItem);
                    SelectedTreeViewItem.IsExpanded = true;
                }
            }

            ChangesTracker++;
        }

        /// <summary>
        /// Updates Properties of all items in the TreeView
        /// </summary>
        private void UpdateRootNodes(bool isSelected = false, bool isEditable = false, bool resetIsDirty = false)
        {
            foreach (var item in RootNodes)
            {
                UpdateRootNodes(item, resetIsDirty);
            }
            if (isSelected)
                SelectedTreeViewItem.IsSelected = true;
            if (isEditable)
                SelectedTreeViewItem.IsEditable = true;
            if (SelectedTreeViewItem.CurrentViewModel is BranchViewModel)
                SelectedTreeViewItem.MayHaveChildren = true;
            if (resetIsDirty)
                SelectedTreeViewItem.IsDirty = false;
        }

        private void UpdateRootNodes(TreeItemViewModel currentItem, bool resetIsDirty)
        {
            currentItem.IsSelected = false;
            currentItem.IsEditable = false;
            currentItem.MayHaveChildren = false;
            currentItem.IsHighlighted = false;
            if (resetIsDirty)
                currentItem.IsDirty = false;
            if (currentItem.Children.Count > 0)
                foreach (var child in currentItem.Children)
                {
                    child.IsSelected = false;
                    child.IsEditable = false;
                    child.MayHaveChildren = false;
                    child.IsHighlighted = false;
                    if (resetIsDirty)
                        child.IsDirty = false;

                    UpdateRootNodes(child, resetIsDirty);
                }
        }

        /// <summary>
        /// Loads the treeview with all Branches and leafes.
        /// </summary>
        private void LoadTreeView()
        {
            Logger.log.Info("Loading Home TreeView...");
            var allBranchesVm = DataAccessService.Instance
                .GetAll<Branch>()
                .Select(b => new BranchViewModel(b, this));

            var allLeafesVmLookup = DataAccessService.Instance
                .GetAll<Leaf>()
                .Select(l => new LeafViewModel(l, this))
                .ToLookup(l => l.BranchId, l => l);

            var branchParentTreeItemLookup = allBranchesVm.ToLookup(b => b.ParentBranchId, b => new TreeItemViewModel(b, TreeDepth.Branch));

            var rootsOnly = allBranchesVm.Where(b => b.ParentBranchId == Guid.Empty).ToList();

            RootNodes.Clear();

            foreach (var root in rootsOnly)
            {
                var currentTreeItem = new TreeItemViewModel(root, TreeDepth.Root);
                // Add leafs
                foreach (var leaf in GetLeafes(allLeafesVmLookup, currentTreeItem.CurrentViewModel.Id))
                    currentTreeItem.Children.Add(leaf);
                // Add first children of root
                if (branchParentTreeItemLookup.Contains(root.Id))
                {
                    var children = branchParentTreeItemLookup.FirstOrDefault(b => b.Key == root.Id);
                    //Recusivly add children of children plus leafes.
                    foreach (var child in children)
                    {
                        RecusivlyAddTreeItems(child, branchParentTreeItemLookup, allLeafesVmLookup);
                        currentTreeItem.Children.Add(child);
                    }
                }
                RootNodes.Add(currentTreeItem);
            }
            Logger.log.Info("TreeView loaded!");
        }

        /// <summary>
        /// Recusivly add children of children and leafes.
        /// </summary>
        /// <param name="currentItem"></param>
        /// <param name="branchParentTreeItemLookup"></param>
        /// <param name="allLeafesVmLookup"></param>
        private void RecusivlyAddTreeItems(TreeItemViewModel currentItem, ILookup<Guid, TreeItemViewModel> branchParentTreeItemLookup, ILookup<Guid, LeafViewModel> allLeafesVmLookup)
        {
            // Add the leafs
            foreach (var leaf in GetLeafes(allLeafesVmLookup, currentItem.CurrentViewModel.Id))
            {
                // check if leafes have already been added. If true, don't add them again.
                if (!(currentItem.Children.Any(l => l.CurrentViewModel.Id == leaf.CurrentViewModel.Id)))
                    currentItem.Children.Add(leaf);
            }
            // Add branch children.
            if (branchParentTreeItemLookup.Contains(currentItem.CurrentViewModel.Id))
            {
                foreach (var item in branchParentTreeItemLookup.FirstOrDefault(i => i.Key == currentItem.CurrentViewModel.Id).ToList())
                {
                    if (!currentItem.Children.Contains(item))
                        currentItem.Children.Add(item);

                    foreach (var childItem in currentItem.Children)
                        RecusivlyAddTreeItems(childItem, branchParentTreeItemLookup, allLeafesVmLookup);
                }
            }
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

    }
}

