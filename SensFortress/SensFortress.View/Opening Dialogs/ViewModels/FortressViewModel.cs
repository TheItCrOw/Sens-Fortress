using Prism.Commands;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Opening_Dialogs.ViewModels
{
    public class FortressViewModel : ViewModelBase
    {
        #region Properties
        public FortressViewModel(string fullPath, DateTime created, DateTime modified, ViewModelManagementBase currentBase, bool isDefaultLocated = true)
        {
            FullName = fullPath;
            var splitedString = fullPath.Split("\\");
            Name = splitedString[splitedString.Length - 1];
            Created = created;
            Modified = modified;
            Information = $"{Modified.ToShortDateString()}: {Name}";
            IsDefaultLocated = isDefaultLocated;
            CurrentBase = currentBase;
        }
        public string FullName { get; }
        public string Name { get; }
        public DateTime Created { get; }
        public DateTime Modified { get; }
        public string Information { get; set; }
        public bool IsDefaultLocated { get; set; }
        #endregion

        public DelegateCommand DelinkCommand => new DelegateCommand(Delink);

        /// <summary>
        /// Delinks an externally added fortress.
        /// </summary>
        private void Delink()
        {
            if (IsDefaultLocated)
                return;

            //Inform the current managementBase about the delink.
            if(CurrentBase != null && CurrentBase is LoginViewModel loginVm)
            {
                loginVm.DelinkExternalFortress(this);
            }
        }
    }
}
