using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prism.Commands;
using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
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
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<int, string> Formatter { get; set; }

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
            try
            {
                LoadQuickbar();
                ChartTesting();
            }
            catch (Exception ex)
            {
                ex.SetUserMessage("A problem occured while trying to load the Home-Hub. Some functions may not work proberly.");
                Logger.log.Error($"Error while trying to initialize HubView: {ex}");
                Communication.InformUserAboutError(ex);
            }
        }

        private void ChartTesting()
        {
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2015",
                    Values = new ChartValues<int> { 10, 50, 39, 50 }
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "2016",
                Values = new ChartValues<int> { 11, 56, 42 }
            });

            //also adding values updates and animates the chart automatically
            SeriesCollection[1].Values.Add(48);

            Labels = new[] { "Maria", "Susan", "Charles", "Frida" };
            Formatter = value => value.ToString("N");

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
            if (currentItem.CurrentViewModel is LeafViewModel leafVm)
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
