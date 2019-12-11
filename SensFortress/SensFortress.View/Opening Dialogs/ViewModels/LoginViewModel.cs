using SensFortress.Models.ViewModels;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace SensFortress.View.Opening_Dialogs.ViewModels
{
    public class LoginViewModel : ViewModelManagementBase
    {
        public ObservableCollection<FortressViewModel> Fortresses { get; set; } = new ObservableCollection<FortressViewModel>();

        public LoginViewModel()
        {
            LoadFortresses();
        }

        private void LoadFortresses()
        {
            try
            {
                var fortresses = Directory.GetFiles(DirectoryHelper.GetDefaultFortressDirectory());

                foreach (var fortress in fortresses)
                {
                    var created = File.GetCreationTime(fortress);
                    var modified = File.GetLastWriteTime(fortress);
                    var fortressVm = new FortressViewModel(fortress, created, modified);
                    Fortresses.Add(fortressVm);
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while loading the fortress list: {ex}");
                ex.SetUserMessage("An error occured while trying to load all known fortresses. If you moved the fortress, try to select it again.");
                InformUserAboutError(ex);
            }
        }


    }
}
