using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Main.ViewModel;
using SensFortress.View.Main.ViewModel.HomeSubVms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SensFortress.View.Bases
{
    public interface IPasswordAnalysable
    {
        public ObservableCollection<AnalysedEntryViewModel> AllAnalyseResults { get; set; } 

        public int TotalPWAnalysisScore { get; set; }
        public int StrongPasswords { get; set; }
        public int MediumPasswords { get; set; }
        public int WeakPasswords { get; set; }
        public int BlacklistedPasswords { get; set; }
        public string StartPasswordAnalysis()
        {
            // Get all LeafVm's
            var analyseableLeafs = DataAccessService.Instance
                .GetAll<Leaf>();
            AllAnalyseResults = new ObservableCollection<AnalysedEntryViewModel>();

            try
            {
                Task.Run(() =>
                {
                    int totalPasswordStrength = 0;
                    foreach (var leafVm in analyseableLeafs)
                    {
                        // Get the parent model
                        var tuples = new Tuple<string, object>[] { Tuple.Create("Id", (object)leafVm.BranchId) };
                        var parent = DataAccessService.Instance.GetExplicit<Branch>(tuples).FirstOrDefault();

                        if (parent != null)
                        {
                            byte[] encryptedPassword = null;
                            // Get the password
                            if (DataAccessService.Instance.TryGetSensible<LeafPassword>(leafVm.Id, out var leafPw))
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
                            Application.Current.Dispatcher.Invoke(() => AllAnalyseResults.Add(analysisVm));
                            // calculate the total score
                            TotalPWAnalysisScore = (totalPasswordStrength / AllAnalyseResults.Count);
                        }
                    }
                    //PWAnalysisIsLoading = false;
                    return null;
                });
                return null;
            }
            catch (Exception ex)
            {
                Logger.log.Error($"An error occured while trying to do a password-analysis: {ex}");
                ex.SetUserMessage("A problem occured while trying to analyse all passwords => information may be incomplete.");
                Communication.InformUserAboutError(ex);
                return null;
            }
        }


    }
}
