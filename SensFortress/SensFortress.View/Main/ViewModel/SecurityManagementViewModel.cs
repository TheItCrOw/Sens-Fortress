using Prism.Commands;
using SensFortress.Data.Database;
using SensFortress.Guardian;
using SensFortress.Guardian.Models;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using SensFortress.View.Extensions;
using SensFortress.View.Main.ViewModel.HomeSubVms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SensFortress.View.Main.ViewModel
{
    public class SecurityManagementViewModel : ViewModelManagementBase
    {
        private bool _isLocked;
        private bool _isScanning;
        private HashSet<AnalysedEntryViewModel> _allAnalysedPasswords;
        private bool _pWAnalysisIsLoading;
        private int _mediumPasswords;
        private int _strongPasswords;
        private int _weakPasswords;
        private int _blacklistedPasswords;
        private int _totalPWAnalysisScore;
        private Point _shieldEndPoint;

        public ObservableCollection<GuardianLogEntry> GuardianLogs { get; set; } = new ObservableCollection<GuardianLogEntry>();
        public ObservableCollection<AnalysedEntryViewModel> AnalysedPasswordsList { get; set; } = new ObservableCollection<AnalysedEntryViewModel>();
        public DelegateCommand ScanFortressCommand => new DelegateCommand(ScanFortress);
        public DelegateCommand AnalysePasswordsCommand => new DelegateCommand(AnalysePasswords);
        public DelegateCommand ClearGuardianLog => new DelegateCommand(() => GuardianLogs.Clear());
        public DelegateCommand<string> FilterAnalysedPasswordListCommand => new DelegateCommand<string>(FilterAnalysedPasswordList);

        public override bool IsLocked
        {
            get
            {
                return _isLocked;
            }
            set
            {
                bool locked = false;
                if (value == true)
                    locked = Settings.GetSettingValue<bool>("B_LockingIncludeSecurityManagement") == true ? true : false;

                SetProperty(ref _isLocked, locked);
            }
        }
        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }
        #region PW analysis properties
        public bool PWAnalysisIsLoading
        {
            get => _pWAnalysisIsLoading;
            set => SetProperty(ref _pWAnalysisIsLoading, value);
        }
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
            set => SetProperty(ref _mediumPasswords, value);
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
        public Point ShieldEndPoint
        {
            get => _shieldEndPoint;
            set
            {
                SetProperty(ref _shieldEndPoint, value);
            }
        }
        #endregion

        /// <summary>
        /// Use this instead of ctor for more control over when it's called.
        /// </summary>
        public void Initialize()
        {
        }

        public SecurityManagementViewModel()
        {
            GuardianLogger.Instance.GuardianLogEntryAdded += GuardianLogger_EntryAdded;
        }

        /// <summary>
        /// Filters the anaylsed passwords for strength
        /// </summary>
        private void FilterAnalysedPasswordList(string filter)
        {
            if (_allAnalysedPasswords == null || _allAnalysedPasswords.Count == 0)
                return;

            AnalysedPasswordsList.Clear();

            switch (filter)
            {
                case "All":
                    AnalysedPasswordsList.AddRange(_allAnalysedPasswords);
                    break;
                case "Strong":
                    AnalysedPasswordsList.AddRange(_allAnalysedPasswords.Where(l => l.PasswordStrength >= 70));
                    break;
                case "Medium":
                    AnalysedPasswordsList.AddRange(_allAnalysedPasswords.Where(l => l.PasswordStrength < 70 && l.PasswordStrength >= 50));
                    break;
                case "Weak":
                    AnalysedPasswordsList.AddRange(_allAnalysedPasswords.Where(l => l.PasswordStrength < 50 && l.PasswordStrength > 0));
                    break;
                case "Blacklisted":
                    AnalysedPasswordsList.AddRange(_allAnalysedPasswords.Where(l => l.PasswordStrength <= 0));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Analysis passwords strength
        /// </summary>
        private void AnalysePasswords()
        {
            try
            {
                _allAnalysedPasswords = new HashSet<AnalysedEntryViewModel>();

                StrongPasswords = 0;
                MediumPasswords = 0;
                WeakPasswords = 0;
                BlacklistedPasswords = 0;
                TotalPWAnalysisScore = 0;
                AnalysedPasswordsList.Clear();

                // Get all LeafVm's
                var analyseableLeafsVm = DataAccessService.Instance
                    .GetAll<Leaf>()
                    .Select(l => new LeafViewModel(l, this));

                PWAnalysisIsLoading = true;

                Task.Run(() =>
                {
                    int totalPasswordStrength = 0;
                    foreach (var leafVm in analyseableLeafsVm)
                    {
                        // Get the parent model
                        var tuples = new Tuple<string, object>[] { Tuple.Create("Id", (object)leafVm.BranchId) };
                        var parent = DataAccessService.Instance.GetExplicit<Branch>(tuples).FirstOrDefault();

                        if (parent != null)
                        {
                            byte[] encryptedPassword = null;
                            // Get the password
                            if (leafVm.LeafPasswordCopy != null)
                                encryptedPassword = leafVm.LeafPasswordCopy.EncryptedValue;
                            else if (DataAccessService.Instance.TryGetSensible<LeafPassword>(leafVm.Id, out var leafPw))
                            {
                                encryptedPassword = CryptMemoryProtection.EncryptInMemoryData(leafPw.Value);
                            }
                            // Caclculate strength
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

                            // add the result to the list
                            _allAnalysedPasswords.Add(analysisVm);

                            Application.Current.Dispatcher.Invoke(() => AnalysedPasswordsList.Add(analysisVm));
                            // calculate the total score
                            TotalPWAnalysisScore = (totalPasswordStrength / AnalysedPasswordsList.Count);
                            ShieldEndPoint = new Point(0, (double)TotalPWAnalysisScore / 100);
                            Thread.Sleep(50);
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
        /// Scans the fortress and stores the results into the guardianlog
        /// </summary>
        private void ScanFortress()
        {
            try
            {
                IsScanning = true;

                Task.Run(() =>
                {
                    var scanner = new FortressScanner();
                    var logEntry = new GuardianLogEntry()
                    {
                        Name = "Fortress scan",
                        Date = DateTime.Now,
                    };

                    bool foundMal = false;
                    var sb = new StringBuilder();
                    sb.AppendLine("Scan results:");

                    Thread.Sleep(1000); // Delete later

                    if (scanner.ScanFortressFile())
                        sb.AppendLine($"Fortress and {TermHelper.GetDatabaseTerm()} have not been maliciously modified from the outside.");
                    else
                    {
                        foundMal = true;
                        sb.AppendLine("The guardian has spotted modifications in your fortress which were injected from outside of Sen's fortess walls. " +
                                $"If you do not recall having modified this file yourself on the harddrive, something else must've done it. " +
                                $"I suggest starting a scan with your antivirus system.");
                    }

                    Thread.Sleep(1000); // Delete later

                    if (scanner.ScanSettings())
                        sb.AppendLine("Your custom configurations have not been maliciously modified from the outside.");
                    else
                    {
                        foundMal = true;
                        sb.AppendLine("The guardian has spotted modifications in your configurations which were injected from outside of Sen's fortess walls. " +
                                $"If you do not recall having modified this file yourself on the harddrive, something else must've done it. " +
                                $"I suggest starting a scan with your antivirus system.");
                    }

                    logEntry.LogType = foundMal == true ? EntryType.Danger : EntryType.Success;
                    sb.AppendLine();
                    sb.AppendLine(foundMal == true ? "Fortress is not 100% save." : "No malicous actions were detected.");
                    logEntry.Description = sb.ToString();

                    Application.Current.Dispatcher?.Invoke(() => GuardianLogs.Add(logEntry));
                    IsScanning = false;
                });

            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while trying to scan fortress: {ex}");
                ex.SetUserMessage("An error occured while trying to scan the fortress - some functionalities may not fully work.");
                Communication.InformUserAboutError(ex);
            }
        }

        private void GuardianLogger_EntryAdded(GuardianLogEntry entry) => Application.Current.Dispatcher?.Invoke(() => GuardianLogs.Add(entry));
    }
}
