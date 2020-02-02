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
using SensFortress.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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
        public ObservableCollection<WebsiteViewModel> Websites { get; set; } = new ObservableCollection<WebsiteViewModel>();
        public DelegateCommand ShowHidePasswordCommand => new DelegateCommand(ShowHidePassword);
        public DelegateCommand ShowUnlockCardCommand => new DelegateCommand(ShowUnlockCard);
        public DelegateCommand EditPasswordCommand => new DelegateCommand(EditPassword);
        public DelegateCommand CopyPasswordToClipboardCommand => new DelegateCommand(CopyPasswordToClipboard);
        public DelegateCommand CopyUsernameToClipboardCommand => new DelegateCommand(CopyUsernameToClipboard);
        public DelegateCommand OpenUrlWithLoginCommand => new DelegateCommand(OpenUrlWithLogin);
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
            if (selectedLeaf.CurrentViewModel is LeafViewModel leafVm)
            {
                CurrentItem = selectedLeaf;
                var storeIsDirty = CurrentItem.IsDirty;
                _pwIsHidden = false;
                _currentBase = (HomeViewModel)currentBase;
                Username = leafVm.Username;
                Description = leafVm.Description;
                Initialize();
                CurrentItem.IsDirty = storeIsDirty;
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
            LoadWebsites();
        }

        /// <summary>
        /// This is currently not finished. It always opens amazon for now.
        /// </summary>
        private void OpenUrlWithLogin()
        {
            var adress = new Uri("https://www.amazon.de/ap/signin?showRememberMe=false&openid.pape.max_auth_age=0&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&pageId=deflex&ignoreAuthState=1&openid.return_to=https%3A%2F%2Fwww.amazon.de%2F%3Fref_%3Dnav_signin&prevRID=T028TS6C1NMGVZ4Y8B9B&openid.assoc_handle=deflex&openid.mode=checkid_setup&openid.ns.pape=http%3A%2F%2Fspecs.openid.net%2Fextensions%2Fpape%2F1.0&prepopulatedLoginId=eyJjaXBoZXIiOiJ0d1VzTS9wRTQrcGM4WFY1NzQrdFp3PT0iLCJ2ZXJzaW9uIjoxLCJJViI6Ims3Sm9GMWJncVFjS29kaWIyQk1RK2c9PSJ9&failedSignInCount=0&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&timestamp=1579545861000");
            var browser = new BrowserView(adress, Username, _encryptedPassword);
            browser.Show();
        }

        /// <summary>
        /// Copies the password to the clipboard.
        /// </summary>
        private void CopyPasswordToClipboard()
        {
            Clipboard.SetText(ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(_encryptedPassword)));
            TaskLogger.Instance.Track($"{CurrentItem.Name}: Password has been copied.");
        }
        /// <summary>
        /// Copies the username to the clipboard.
        /// </summary>
        private void CopyUsernameToClipboard()
        {
            Clipboard.SetText(Username);
            TaskLogger.Instance.Track($"{CurrentItem.Name}: Username has been copied.");
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
                    if (CurrentItem.CurrentViewModel is LeafViewModel leafVm)
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
            if (CurrentItem.CurrentViewModel is LeafViewModel leafVm)
            {
                // If the password has been edited, we need to use the PasswordCopy. Otherwise we always load the old one stored in the DC!
                if (leafVm.LeafPasswordCopy != null)
                    _encryptedPassword = leafVm.LeafPasswordCopy.EncryptedValue;
                else
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
            }
        }
        /// <summary>
        /// Not finished - later the website models should be saved in the DC
        /// </summary>
        private void LoadWebsites()
        {
            var amazon = new Website("https://www.amazon.de/ap/signin?showRememberMe=false&openid.pape.max_auth_age=0&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&pageId=deflex&ignoreAuthState=1&openid.return_to=https%3A%2F%2Fwww.amazon.de%2F%3Fref_%3Dnav_signin&prevRID=T028TS6C1NMGVZ4Y8B9B&openid.assoc_handle=deflex&openid.mode=checkid_setup&openid.ns.pape=http%3A%2F%2Fspecs.openid.net%2Fextensions%2Fpape%2F1.0&prepopulatedLoginId=eyJjaXBoZXIiOiJ0d1VzTS9wRTQrcGM4WFY1NzQrdFp3PT0iLCJ2ZXJzaW9uIjoxLCJJViI6Ims3Sm9GMWJncVFjS29kaWIyQk1RK2c9PSJ9&failedSignInCount=0&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&timestamp=1579545861000");
            amazon.Name = "Amazon.com";
            amazon.LogoSource = "pack://application:,,,/Logos/amazon-logo.png";

            var amazonVm = new WebsiteViewModel(amazon, this);
            Websites.Add(amazonVm);
            Websites.Add(amazonVm);
            Websites.Add(amazonVm);
            Websites.Add(amazonVm);
            Websites.Add(amazonVm);
            Websites.Add(amazonVm);
            Websites.Add(amazonVm);
            Websites.Add(amazonVm);
            Websites.Add(amazonVm);
            Websites.Add(amazonVm);
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
