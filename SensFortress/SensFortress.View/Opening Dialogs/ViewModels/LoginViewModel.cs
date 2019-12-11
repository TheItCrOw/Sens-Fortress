using SensFortress.Models.ViewModels;
using SensFortress.Utility;
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
            var fortresses = Directory.GetFiles(DirectoryHelper.GetDefaultFortressDirectory());

            foreach(var fortress in fortresses)
            {
                var fortressVm = new FortressViewModel(fortress);
                Fortresses.Add(fortressVm);
            }
        }


    }
}
