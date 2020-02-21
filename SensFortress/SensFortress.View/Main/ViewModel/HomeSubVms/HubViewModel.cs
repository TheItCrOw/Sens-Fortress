using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prism.Commands;
using SensFortress.Data.Database;
using SensFortress.Guardian.Models;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using SensFortress.View.Helper;
using SensFortress.View.Main.Views.HomeSubViews;
using SensFortress.View.TaskLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    public class HubViewModel : ViewModelManagementBase
    {
        private bool _isLocked;
        private HashSet<LeafViewModel> _allLeafsVmSnapshot;
        private int _chartMinValue;
        private int _chartMaxValue;
        private bool _chartIsLoading;
        private bool _pWAnalysisIsLoading;
        private int _totalPWAnalysisScore;
        private int _strongPasswords;
        private int _mediumPasswords;
        private int _weakPasswords;
        private int _blacklistedPasswords;

        public ObservableCollection<LeafViewModel> QuickBar { get; set; } = new ObservableCollection<LeafViewModel>();
        public ObservableCollection<AnalysedEntryViewModel> AllAnalyseResults { get; set; } = new ObservableCollection<AnalysedEntryViewModel>();
        public ObservableCollection<ConfigViewModel> Configurations { get; set; } = new ObservableCollection<ConfigViewModel>();
        public DelegateCommand<TreeItemViewModel> AddQuickBarItemCommand => new DelegateCommand<TreeItemViewModel>(AddQuickBarItem);
        public DelegateCommand<LeafViewModel> RemoveQuickBarItemCommand => new DelegateCommand<LeafViewModel>(RemoveQuickBarItem);
        public DelegateCommand StartPasswordAnalysisCommand => new DelegateCommand(StartPasswordAnalysis);
        public DelegateCommand OpenSettingsCommand => new DelegateCommand(() => Navigation.NavigateTo(NavigationViews.Settings));
        public DelegateCommand PrintEmergencySheetCommand => new DelegateCommand(PrintEmergencySheet);

        [Obsolete]
        #region Chart Properties
        public SeriesCollection ChartSeries { get; set; } = new SeriesCollection();
        public List<string> ChartLabels { get; set; } = new List<string>();
        public Func<int, string> ChartFormatter { get; set; }
        public int ChartMinValue
        {
            get => _chartMinValue;
            set
            {
                SetProperty(ref _chartMinValue, value);
            }
        }
        public int ChartMaxValue
        {
            get => _chartMaxValue;
            set
            {
                SetProperty(ref _chartMaxValue, value);
            }
        }
        public bool ChartIsLoading
        {
            get => _chartIsLoading;
            set
            {
                SetProperty(ref _chartIsLoading, value);
            }
        }
        #endregion

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
        public bool PWAnalysisIsLoading
        {
            get => _pWAnalysisIsLoading;
            set
            {
                SetProperty(ref _pWAnalysisIsLoading, value);
            }
        }
        #region PW analysis
        public int TotalPWAnalysisScore
        {
            get => _totalPWAnalysisScore;
            set
            {
                SetProperty(ref _totalPWAnalysisScore, value);
            }
        }
        public int StrongPasswords
        {
            get => _strongPasswords;
            set
            {
                SetProperty(ref _strongPasswords, value);
            }
        }
        public int MediumPasswords
        {
            get => _mediumPasswords;
            set
            {
                SetProperty(ref _mediumPasswords, value);
            }
        }
        public int WeakPasswords
        {
            get => _weakPasswords;
            set
            {
                SetProperty(ref _weakPasswords, value);
            }
        }
        public int BlacklistedPasswords
        {
            get => _blacklistedPasswords;
            set
            {
                SetProperty(ref _blacklistedPasswords, value);
            }
        }
        #endregion

        public void Initialize()
        {
            try
            {
                Settings.HandledSettingHasBeenRaised += Settings_ConfigHasBeenRaised;

                var currentNodes = Navigation.HomeManagementInstance.GetRootNodesSnapshot();
                _allLeafsVmSnapshot = new HashSet<LeafViewModel>();
                LoadConfigurations();
                LoadQuickbar(currentNodes);
                //LoadChart();
            }
            catch (Exception ex)
            {
                ex.SetUserMessage("A problem occured while trying to load the Home-Hub. Some functions may not work proberly.");
                Logger.log.Error($"Error while trying to initialize HubView: {ex}");
                Communication.InformUserAboutError(ex);
            }
        }

        /// <summary>
        /// Loads all scheduled configs of the user
        /// </summary>
        public void LoadConfigurations()
        {
            Configurations.Clear();

            // Get all ScheudledConfigs and make ViewModels
            var configs = Settings
                .GetSettingsForGuardian()
                .Where(gT => gT is ScheduledConfig config)
                .Select(c => new ScheduledConfigViewModel((ScheduledConfig)c, this));

            foreach (var config in configs)
                Configurations.Add(config);
        }

        /// <summary>
        /// Triggers, when a scheduledConfig has been raised by <see cref="Settings"/>.
        /// </summary>
        /// <param name="config"></param>
        private void Settings_ConfigHasBeenRaised(ScheduledConfig config)
        {
            // Name must always be unique with settings => Id's may change
            foreach (var c in Configurations)
                if (c.Name == config.Name && c is ScheduledConfigViewModel scheduledC)
                    scheduledC.UpdateNextExecution(config.Gtid.ExecutionDate);
        }

        /// <summary>
        /// Opens a new print view for printing a password emergency sheet
        /// </summary>
        private void PrintEmergencySheet()
        {
            var printVm = new PrintViewModel();
            printVm.StartPasswordEntriesPrintProcess(_allLeafsVmSnapshot);
        }

        /// <summary>
        /// Analysies all passwords and calculates it's strength
        /// </summary>
        private void StartPasswordAnalysis()
        {
            if (_allLeafsVmSnapshot.Count == 0 || _allLeafsVmSnapshot == null)
                return;

            PWAnalysisIsLoading = true;
            AllAnalyseResults.Clear();
            StrongPasswords = 0;
            MediumPasswords = 0;
            WeakPasswords = 0;
            BlacklistedPasswords = 0;

            try
            {
                Task.Run(() =>
                {
                    int totalPasswordStrength = 0;
                    foreach (var leafVm in _allLeafsVmSnapshot)
                    {
                        // Get the parent model
                        var tuples = new Tuple<string, object>[] { Tuple.Create("Id", (object)leafVm.BranchId) };
                        var parent = DataAccessService.Instance.GetExplicit<Branch>(tuples).FirstOrDefault();

                        if (parent != null)
                        {
                            byte[] encryptedPassword = null;
                            if (leafVm.LeafPasswordCopy != null)
                                encryptedPassword = leafVm.LeafPasswordCopy.EncryptedValue;
                            else if (DataAccessService.Instance.TryGetSensible<LeafPassword>(leafVm.Id, out var leafPw))
                            {
                                encryptedPassword = CryptMemoryProtection.EncryptInMemoryData(leafPw.Value);
                            }
                            var passwordStrength = PasswordHelper.CalculatePasswordStrength(encryptedPassword, out var resultTips, out var isBlackListed);
                            totalPasswordStrength += (int)(passwordStrength * 100);
                            encryptedPassword = null;

                            var analysisVm = new AnalysedEntryViewModel(passwordStrength, resultTips)
                            {
                                Category = parent.Name,
                                Name = leafVm.Name,
                            };

                            if (analysisVm.PasswordStrength >= 70)
                                Application.Current.Dispatcher.Invoke(() => StrongPasswords++);
                            else if (analysisVm.PasswordStrength >= 50)
                                Application.Current.Dispatcher.Invoke(() => MediumPasswords++);
                            else if (analysisVm.PasswordStrength > 0)
                                Application.Current.Dispatcher.Invoke(() => WeakPasswords++);
                            else if (analysisVm.PasswordStrength <= 0)
                                Application.Current.Dispatcher.Invoke(() => BlacklistedPasswords++);

                            // Can be deleted later
                            Thread.Sleep(50);
                            // add the result to the list
                            Application.Current.Dispatcher.Invoke(() => AllAnalyseResults.Add(analysisVm));
                            // calculate the total score
                            TotalPWAnalysisScore = (totalPasswordStrength / AllAnalyseResults.Count);
                        }
                    }

                    PWAnalysisIsLoading = false;
                });
            }
            catch (Exception ex)
            {
                Logger.log.Error($"An error occured while trying to do a password-analysis: {ex}");
                ex.SetUserMessage("A problem occured while trying to analyse all passwords => information may be incomplete.");
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

        /// <summary>
        /// Currently not supported!
        /// </summary>
        [Obsolete]
        private void LoadChart()
        {
            ChartIsLoading = true;
            ChartSeries.Clear();

            Task.Run(() =>
            {
                // Load the chart values
                var topTenInteracted = new List<Tuple<int, string>>();

                foreach (var leafVm in _allLeafsVmSnapshot)
                {
                    // We cant plot value 0
                    if (leafVm.InteractedCounter == 0)
                        leafVm.InteractedCounter = 1;
                    // If the list isnt at 10, just add it
                    if (topTenInteracted.Count < 10)
                    {
                        topTenInteracted.Add(Tuple.Create(leafVm.InteractedCounter, leafVm.Name));
                    }
                    // Else check for higher values
                    else
                    {
                        var possibleLowestValue = topTenInteracted.FirstOrDefault(t => t.Item1 < leafVm.InteractedCounter);
                        if (possibleLowestValue != null)
                        {
                            var index = topTenInteracted.IndexOf(possibleLowestValue);

                            topTenInteracted.RemoveAt(index);
                            topTenInteracted.Add(Tuple.Create(leafVm.InteractedCounter, leafVm.Name));
                        }
                    }
                }
                // Order from 10 to 1 e.g.
                topTenInteracted = topTenInteracted.OrderByDescending(i => i.Item1).ToList();
                var chartValues = new ChartValues<int>();
                ChartLabels.Clear();
                foreach (var tuple in topTenInteracted)
                {
                    chartValues.Add(tuple.Item1);
                    ChartLabels.Add(tuple.Item2);
                }
                // Cant change UI stuff in diff task...
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var columnSeries = new ColumnSeries
                    {
                        Title = "Usage amount: ",
                        Values = chartValues,
                        Fill = Brushes.Black,
                        StrokeThickness = 1,
                        Stroke = Brushes.Black
                    };

                    ChartMinValue = 0;
                    if (chartValues.Max() < 10)
                        ChartMaxValue = 10;
                    else
                        ChartMaxValue = chartValues.Max();

                    ChartSeries.Add(columnSeries);
                    // Obsolete, but maybe need later
                    ChartFormatter = value => value.ToString("N");
                }); // dispatcher end
            }); // task end

            ChartIsLoading = false;
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
