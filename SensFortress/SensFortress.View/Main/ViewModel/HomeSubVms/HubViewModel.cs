using Prism.Commands;
using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using SensFortress.View.Helper;
using SensFortress.View.TaskLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    public class HubViewModel : ViewModelManagementBase
    {
        private bool _isLocked;
        public ObservableCollection<LeafViewModel> QuickBar { get; set; } = new ObservableCollection<LeafViewModel>();
        public DelegateCommand<TreeItemViewModel> AddQuickBarItemCommand => new DelegateCommand<TreeItemViewModel>(AddQuickBarItem);
        public DelegateCommand<LeafViewModel> RemoveQuickBarItemCommand => new DelegateCommand<LeafViewModel>(RemoveQuickBarItem);

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
            LoadQuickbar();
        }

        /// <summary>
        /// Returns a snapshot of the current quickbar.
        /// </summary>
        /// <returns></returns>
        public List<LeafViewModel> GetQuickbarSnapshot() => QuickBar.ToList();

        /// <summary>
        /// Recursivly loads the quickbar from the RootNodes in the TreeView.
        /// </summary>
        private void LoadQuickbar()
        {
            var currentNodes = Navigation.HomeManagementInstance.GetRootNodesSnapshot();
            QuickBar.Clear();
            foreach (var node in currentNodes)
                LoadQuickbar(node);
        }

        private void LoadQuickbar(TreeItemViewModel currentItem)
        {
            if(currentItem.CurrentViewModel is LeafViewModel leafVm)
            {
                if (leafVm.QuickbarOrder > 0)
                    QuickBar.Add(leafVm);
            }
            else
            {
                foreach (var child in currentItem.Children)
                    LoadQuickbar(child);
            }
        }

        /// <summary>
        /// Removes a leafVm from the quickbar. Gets called by a command of <see cref="LeafViewModel"/>
        /// </summary>
        /// <param name="leafVm"></param>
        private void RemoveQuickBarItem(LeafViewModel leafVm)
        {
            if (QuickBar.Contains(leafVm))
            {
                // QuickbarOrder = 0  equals not being in the quickbar
                leafVm.QuickbarOrder = 0;
                QuickBar.Remove(leafVm);
                TaskLogger.Instance.Track($"Removed {leafVm.Name} from Quickbar.");
                Navigation.HomeManagementInstance.ChangesTracker++;
            }
            else
                Logger.log.Info("Tried to remove item from quickbar that didnt exist.");
        }

        /// <summary>
        /// Adds a dragged item to the Quickbar
        /// </summary>
        /// <param name="draggedItem"></param>
        private void AddQuickBarItem(TreeItemViewModel draggedItem)
        {
            if (draggedItem.CurrentViewModel is LeafViewModel leafVm)
            {
                if (!QuickBar.Contains(leafVm))
                {
                    QuickBar.Add(leafVm);
                    leafVm.QuickbarOrder = QuickBar.IndexOf(leafVm) + 1;
                    TaskLogger.Instance.Track($"{leafVm.Name} added to Quickbar.");
                    Navigation.HomeManagementInstance.ChangesTracker++;
                }
                else
                    Communication.InformUser("Item has already been added to the Quickbar.");
            }
            else
                Communication.InformUser("You can currently only add Password-Entries to the Quickbar.");
        }

    }
}
