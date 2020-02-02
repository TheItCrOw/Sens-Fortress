using Prism.Commands;
using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using SensFortress.View.Helper;
using SensFortress.View.Main.ViewModel.HomeSubVms;
using SensFortress.View.TaskLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SensFortress.View.Main.ViewModel
{
    public class LeafViewModel : ViewModelBase
    {
        private string _name;
        private string _description;
        private string _userName;
        private int _quickbarOrder;

        public DelegateCommand CopyPasswordCommand => new DelegateCommand(CopyPassword);
        public DelegateCommand CopyUsernameCommand => new DelegateCommand(CopyUsername);
        public DelegateCommand DeleteFromQuickbarCommand => new DelegateCommand(DeleteFromQuickbar);

        public LeafViewModel(Leaf model, ViewModelManagementBase currentBase)
        {
            Model = model;
            Id = model.Id;
            BranchId = model.BranchId;
            Name = model.Name;
            Description = model.Description;
            CurrentBase = currentBase;
            Username = model.Username;
            QuickbarOrder = model.QuickbarOrder;
        }
        public Guid BranchId { get; set; }

        /// <summary>
        /// This has only value if the password has been edited!
        /// </summary>
        public LeafPassword LeafPasswordCopy { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
            }
        }
        public string Username
        {
            get => _userName;            
            set
            {
                SetProperty(ref _userName, value);
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
            }
        }
        public int QuickbarOrder
        {
            get => _quickbarOrder;
            set
            {
                SetProperty(ref _quickbarOrder, value);
                // Inform the model about the changed property. We do this very easily, since nothing needs to be informed in this case.
                ((Leaf)Model).QuickbarOrder = QuickbarOrder;
            }
        }
        private void DeleteFromQuickbar()
        {
            if(Navigation.HomeManagementInstance.GetCurrentHub().DataContext is HubViewModel hubVm)
            {
                hubVm.RemoveQuickBarItemCommand.Execute(this);
            }
            else
            {
                var ex = new Exception();
                ex.SetUserMessage($"Item couldn't be removed from Quickbar. Maybe it has already been deleted - Try to navigate to your home hub.");
                Communication.InformUserAboutError(ex);
                Logger.log.Error($"Error while trying to remove {Id} from Quickbar: {ex}");
            }
        }
        private void CopyUsername() => Clipboard.SetText(Username);
        private void CopyPassword()
        {
            if(LeafPasswordCopy != null)
                Clipboard.SetText(ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(LeafPasswordCopy.EncryptedValue)));
            else
            {
                if (DataAccessService.Instance.TryGetSensible<LeafPassword>(Id, out var leafPw))
                {
                    var encryptedPassword = CryptMemoryProtection.EncryptInMemoryData(leafPw.Value);
                    Clipboard.SetText(ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(encryptedPassword)));
                    encryptedPassword = null;
                    leafPw = null;
                }
            }
            TaskLogger.Instance.Track($"{Name}: Password has been copied.");
        }
    }
}
