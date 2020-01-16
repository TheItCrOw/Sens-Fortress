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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    public class SelectedLeafViewModel : ViewModelManagementBase
    {
        private TreeItemViewModel _currentIten;
        private bool _pwIsHidden;
        private string _password;
        private bool _isLocked;
        private bool _showContent;
        private byte[] _encryptedPassword;

        public DelegateCommand ShowHidePasswordCommand => new DelegateCommand(ShowHidePassword);
        public DelegateCommand ShowUnlockCardCommand => new DelegateCommand(ShowUnlockCard);
        public DelegateCommand EditPasswordCommand => new DelegateCommand(EditPassword);
        public TreeItemViewModel CurrentItem
        {
            get => _currentIten;
            set
            {
                SetProperty(ref _currentIten, value);
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

        public SelectedLeafViewModel(TreeItemViewModel selectedLeaf)
        {
            CurrentItem = selectedLeaf;
            _pwIsHidden = false;
            Initialize();
        }

        private void Initialize()
        {
            LoadPassword();
            ShowHidePassword();
        }

        private void ShowUnlockCard() => Navigation.HomeManagementInstance.LockUnlockFortressCommand.Execute();

        private void EditPassword()
        {
            var changePassordView = new ChangePasswordView(_encryptedPassword);
            changePassordView.ShowDialog();
            if (changePassordView.DialogResult == true)
            {
                _encryptedPassword = changePassordView.ChangedPasswordEncrypted;
                _pwIsHidden = !_pwIsHidden;
                CurrentItem.IsDirty = true;
                ShowHidePassword();
            }
        }

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
