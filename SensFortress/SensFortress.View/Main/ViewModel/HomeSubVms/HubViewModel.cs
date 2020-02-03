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
using System.Windows.Media;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    public class HubViewModel : ViewModelManagementBase
    {
        private bool _isLocked;
        private List<LeafViewModel> _allLeafsVmSnapshot;
        public ObservableCollection<LeafViewModel> QuickBar { get; set; } = new ObservableCollection<LeafViewModel>();
        public DelegateCommand<TreeItemViewModel> AddQuickBarItemCommand => new DelegateCommand<TreeItemViewModel>(AddQuickBarItem);
        public DelegateCommand<LeafViewModel> RemoveQuickBarItemCommand => new DelegateCommand<LeafViewModel>(RemoveQuickBarItem);
        public SeriesCollection ChartSeries { get; set; }
        public List<string> ChartLabels { get; set; }
        public Func<int, string> ChartFormatter { get; set; }

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
                var currentNodes = Navigation.HomeManagementInstance.GetRootNodesSnapshot();
                _allLeafsVmSnapshot = new List<LeafViewModel>();
                LoadQuickbar(currentNodes);
                LoadChart(currentNodes);
            }
            catch (Exception ex)
            {
                ex.SetUserMessage("A problem occured while trying to load the Home-Hub. Some functions may not work proberly.");
                Logger.log.Error($"Error while trying to initialize HubView: {ex}");
                Communication.InformUserAboutError(ex);
            }
        }

        /// <summary>
        /// Returns a snapshot of the current quickbar.
        /// </summary>
        /// <returns></returns>
        public List<LeafViewModel> GetQuickbarSnapshot() => QuickBar.ToList();

        /// <summary>
        /// Recursivly loads the quickbar from the RootNodes in the TreeView.
        /// </summary>
        private void LoadQuickbar(List<TreeItemViewModel> currentNodes)
        {
            QuickBar.Clear();
            foreach (var node in currentNodes)
                LoadQuickbar(node);
        }

        private void LoadChart(List<TreeItemViewModel> currentNodes)
        {
            ChartSeries = new SeriesCollection();

            // Load the chart values
            var topTenInteracted = new ChartValues<int>();
            var topTenLabels = new List<string>();

            foreach(var leafVm in _allLeafsVmSnapshot)
            {
                if (topTenInteracted.Count < 10)
                {
                    topTenInteracted.Add(leafVm.InteractedCounter);
                    topTenLabels.Add(leafVm.Name);
                }
                else
                {
                    var possibleLessValue = topTenInteracted.FirstOrDefault(i => i < leafVm.InteractedCounter);
                    if(possibleLessValue != default)
                    {
                        topTenInteracted.Remove(possibleLessValue);
                        topTenInteracted.Add(leafVm.InteractedCounter);

                        var index = topTenInteracted.IndexOf(possibleLessValue);
                        topTenLabels.RemoveAt(index);
                        topTenLabels.Add(leafVm.Name);
                    }
                }
            }

            var columnSeries = new ColumnSeries
            {
                Title = "Usage amount: ",
                Values = topTenInteracted,
                Fill = Brushes.Black,
                StrokeThickness = 1,
                Stroke = Brushes.Black,   
            };

            //also adding values updates and animates the chart automatically
            //ChartSeries[0].Values.Add(48);

            ChartSeries.Add(columnSeries);
            ChartLabels = topTenLabels;
            ChartFormatter = value => value.ToString();
        }

        private void LoadQuickbar(TreeItemViewModel currentItem)
        {
            if (currentItem.CurrentViewModel is LeafViewModel leafVm)
            {
                _allLeafsVmSnapshot.Add(leafVm);

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
