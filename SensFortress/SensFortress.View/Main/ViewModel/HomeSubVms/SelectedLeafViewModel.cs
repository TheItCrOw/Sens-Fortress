using Prism.Commands;
using SensFortress.Data.Database;
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
using System.Collections.Specialized;
using System.Text;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    public class SelectedLeafViewModel : ViewModelManagementBase
    {
        private TreeItemViewModel _currentItem;
        private bool _pwIsHidden;
        private string _password;
        private bool _isLocked;
        private bool _showContent;
        private byte[] _encryptedPassword;
        private HomeViewModel _currentBase;
        private string _userName;
        private string _description;

        public DelegateCommand ShowHidePasswordCommand => new DelegateCommand(ShowHidePassword);
        public DelegateCommand ShowUnlockCardCommand => new DelegateCommand(ShowUnlockCard);
        public DelegateCommand EditPasswordCommand => new DelegateCommand(EditPassword);
        public TreeItemViewModel CurrentItem
        {
            get => _currentItem;
            set
            {
                SetProperty(ref _currentItem, value);
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
            }
        }
        public string Username
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, value);
                CurrentItem.HandleChangeableProperties(nameof(Username), Username);
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
                CurrentItem.HandleChangeableProperties(nameof(Description), Description);
            }
        }

        /// <summary>
        /// Determines whether the fortress is currently locked.
        /// </summary>
        public override bool IsLocked
        {
            get => _isLocked;
            set
            {
                SetProperty(ref _isLocked, value);
                ShowContent = !IsLocked;
            }
        }
        /// <summary>
        /// Determines whether the user may see/use the password content
        /// </summary>
        public bool ShowContent
        {
            get => _showContent;
            set
            {
                SetProperty(ref _showContent, value);
                _pwIsHidden = false;
                ShowHidePassword();
            }
        }

        public SelectedLeafViewModel(TreeItemViewModel selectedLeaf, ViewModelManagementBase currentBase)
        {
            if(selectedLeaf.CurrentViewModel is LeafViewModel leafVm)
            {
                CurrentItem = selectedLeaf;
                _pwIsHidden = false;
                _currentBase = (HomeViewModel)currentBase;
                Username = leafVm.Username;
                Description = leafVm.Description;
                Initialize();
                CurrentItem.IsDirty = false;
            }
            else
            {
                Logger.log.Error("A non LeafViewModel has been found in the SelectedLeafViewModel. Aborting task.");
                Communication.InformUser("There was a problem executing the logic of the selected item.");
            }
        }

        private void Initialize()
        {
            LoadPassword();
            ShowHidePassword();
        }

        /// <summary>
        /// Give user oppurtunity to unlock fortress
        /// </summary>
        private void ShowUnlockCard() => Navigation.HomeManagementInstance.LockUnlockFortressCommand.Execute();

        /// <summary>
        /// When user wants to edit the password. We handle that explicitly.
        /// </summary>
        private void EditPassword()
        {
            try
            {
                var changePassordView = new ChangePasswordView(_encryptedPassword);
                changePassordView.ShowDialog();
                if (changePassordView.DialogResult == true)
                {
                    // Encrypt pw again
                    _encryptedPassword = changePassordView.ChangedPasswordEncrypted;
                    _pwIsHidden = !_pwIsHidden;
                    // For saving's sake: Fill the LeafPassword of the current leafViewModel with the new pw. But encrypted!
                    if(CurrentItem.CurrentViewModel is LeafViewModel leafVm)
                    {
                        var leafPw = new LeafPassword { ForeignId = CurrentItem.CurrentViewModel.Id, EncryptedValue = _encryptedPassword };
                        leafVm.LeafPasswordCopy = leafPw;
                    }
                    CurrentItem.IsDirty = true;
                    _currentBase.ChangesTracker++;
                    TaskLogger.Instance.Track($"{CurrentItem.Name}: Changed password.");
                    ShowHidePassword();
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while editing password: {ex}");
                ex.SetUserMessage("An error occured while trying to edit the password. The memory is being flushed to prevent any leaks.");
                Communication.InformUserAboutError(ex);
            }

        }

        /// <summary>
        /// Initially load the password
        /// </summary>
        private void LoadPassword()
        {
            if (DataAccessService.Instance.TryGetSensible<LeafPassword>(CurrentItem.CurrentViewModel.Id, out var leafPw))
            {
                _encryptedPassword = CryptMemoryProtection.EncryptInMemoryData(leafPw.Value);
                leafPw = null;
            }
            else
            {
                Communication.InformUser($"There was a problem finding the password in the {TermHelper.GetDatabaseTerm()}.");
            }
        }

        /// <summary>
        /// Shows or hides the password by getting the sensible data on demand from the secure DC
        /// </summary>
        private void ShowHidePassword()
        {
            try
            {
                if (_pwIsHidden)
                {
                    Password = ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(_encryptedPassword));
                    _pwIsHidden = false;
                }
                else
                {
                    Password = "************************";
                    _pwIsHidden = true;
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                Password = "************************";
                ex.SetUserMessage("An error occured while trying to handle the password. The memory is being flushed to prevent any leaks.");
                Communication.InformUserAboutError(ex);
            }
        }


    }
}
