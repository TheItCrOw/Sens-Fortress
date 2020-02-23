﻿using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    public class SelectedLeafViewModel : ViewModelManagementBase
    {
        private TreeItemViewModel _currentItem;
        private string _password;
        private bool _isLocked;
        private bool _showContent;
        private byte[] _encryptedPassword;
        private HomeViewModel _currentBase;
        private string _userName;
        private string _description;
        private Point _shieldEndPoint;
        private string _passwordStrength;
        private double _passwordStrengthValue;
        private bool _isBlackListed;
        private string _url;

        #region Properties
        public ObservableCollection<WebsiteViewModel> Websites { get; set; } = new ObservableCollection<WebsiteViewModel>();
        public DelegateCommand ShowUnlockCardCommand => new DelegateCommand(ShowUnlockCard);
        public DelegateCommand EditPasswordCommand => new DelegateCommand(EditPassword);
        public DelegateCommand EditEntryCommand => new DelegateCommand(EditEntry);
        public DelegateCommand CopyPasswordToClipboardCommand => new DelegateCommand(CopyPasswordToClipboard);
        public DelegateCommand CopyUsernameToClipboardCommand => new DelegateCommand(CopyUsernameToClipboard);
        public DelegateCommand OpenUrlCommand => new DelegateCommand(OpenUrl);
        public Point ShieldEndPoint
        {
            get => _shieldEndPoint;
            set
            {
                SetProperty(ref _shieldEndPoint, value);
            }
        }
        public string PasswordStrength
        {
            get => _passwordStrength;
            set
            {
                SetProperty(ref _passwordStrength, value);
            }
        }
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
        public string Url
        {
            get => _url;
            set
            {
                SetProperty(ref _url, value);
                CurrentItem.HandleChangeableProperties(nameof(Url), Url);
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
        public bool IsBlackListed
        {
            get => _isBlackListed;
            set
            {
                SetProperty(ref _isBlackListed, value);
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
            }
        }
        #endregion

        public SelectedLeafViewModel(TreeItemViewModel selectedLeaf, ViewModelManagementBase currentBase)
        {
            try
            {
                if (selectedLeaf.CurrentViewModel is LeafViewModel leafVm)
                {
                    CurrentItem = selectedLeaf;
                    var storeIsDirty = CurrentItem.IsDirty;
                    _currentBase = (HomeViewModel)currentBase;
                    Username = leafVm.Username;
                    Description = leafVm.Description;
                    Url = leafVm.Url;
                    Password = "**********************";
                    Initialize();
                    CurrentItem.IsDirty = storeIsDirty;
                }
                else
                {
                    Logger.log.Error("A non LeafViewModel has been found in the SelectedLeafViewModel. Aborting task.");
                    Communication.InformUser("There was a problem handling the selected item.");
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while loading the selected leaf {selectedLeaf.CurrentViewModel.Id}: {ex}");
                Communication.InformUser("There was a problem handling the selected item.");
            }
        }

        private void Initialize()
        {
            LoadPassword();
            LoadShieldUI();
        }

        /// <summary>
        /// Loads the Shield UI and PasswordStrength Property showing how strong the given PW is.
        /// </summary>
        private void LoadShieldUI() => AnimateValueFill(_passwordStrengthValue, 0.001);

        /// <summary>
        /// Opens a new edit view.
        /// </summary>
        private void EditEntry()
        {
            var manageView = new ManagePasswordEntryView()
            {
                Name = CurrentItem.Name,
                Username = this.Username,
                Url = this.Url,
                Password = ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(_encryptedPassword)),
                Description = this.Description
            };
            manageView.ShowDialog();

            if (manageView.DialogResult == true)
            {
                if (CurrentItem.Name != manageView.Name)
                    CurrentItem.Name = manageView.Name;
                if (Username != manageView.Username)
                    Username = manageView.Username;
                if (Url != manageView.Url)
                    Url = manageView.Url;
                if (Description != manageView.Description)
                    Description = manageView.Description;

                // Encrypt pw again only if changes were made
                if (ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(_encryptedPassword)) != manageView.Password)
                {
                    _encryptedPassword = CryptMemoryProtection.EncryptInMemoryData(ByteHelper.StringToByteArray(manageView.Password));
                    // Update strength level
                    _passwordStrengthValue = PasswordHelper.CalculatePasswordStrength(_encryptedPassword, out var resultTips, out var isBlackListed);
                    IsBlackListed = isBlackListed;

                    LoadShieldUI();
                    // For saving's sake: Fill the LeafPassword of the current leafViewModel with the new pw. But encrypted!
                    if (CurrentItem.CurrentViewModel is LeafViewModel leafVm)
                    {
                        var leafPw = new LeafPassword { ForeignId = CurrentItem.CurrentViewModel.Id, EncryptedValue = _encryptedPassword };
                        leafVm.LeafPasswordCopy = leafPw;
                    }
                    CurrentItem.IsDirty = true;
                    _currentBase.ChangesTracker++;
                    TaskLogger.Instance.Track($"{CurrentItem.Name}: Changed password.");
                }
            }
        }

        /// <summary>
        /// Animating the loading by filling the graph slowly.
        /// </summary>
        /// <param name="end"></param>
        /// <param name="step"></param>
        private void AnimateValueFill(double end, double step)
        {
            Task.Run(() =>
           {
               if (_passwordStrengthValue == 0)
               {
                   PasswordStrength = "This password is blacklisted.";
                   ShieldEndPoint = new Point(0, 1);
                   return;
               }
               // Strength level starts at 1 and loops until 1 - end.
               for (double i = 0; i < end; i = i + step)
               {
                   ShieldEndPoint = new Point(0, 1 - i);
                   PasswordStrength = $"Password strength: {(int)((i + step) * 100)}%";
                   Thread.Sleep(1); // This is pretty horrible...but for now I don't have a solution.   
               }
           });
        }

        /// <summary>
        /// Opens a browser starting the given url
        /// </summary>
        private void OpenUrl() => ((LeafViewModel)CurrentItem.CurrentViewModel).OpenUrlCommand.Execute();

        /// <summary>
        /// Copies the password to the clipboard.
        /// </summary>
        private void CopyPasswordToClipboard() => ((LeafViewModel)CurrentItem.CurrentViewModel).CopyPasswordCommand.Execute();

        /// <summary>
        /// Copies the username to the clipboard.
        /// </summary>
        private void CopyUsernameToClipboard() => ((LeafViewModel)CurrentItem.CurrentViewModel).CopyUsernameCommand.Execute();

        /// <summary>
        /// Give user oppurtunity to unlock fortress
        /// </summary>
        private void ShowUnlockCard() => Navigation.HomeManagementInstance.LockUnlockFortressCommand.Execute();

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
                _passwordStrengthValue = PasswordHelper.CalculatePasswordStrength(_encryptedPassword, out var resultTips, out var isBlackListed);
                IsBlackListed = isBlackListed;
            }
        }

        [Obsolete]
        /// <summary>
        /// This is currently not finished. It always opens amazon for now.
        /// </summary>
        private void OpenUrlWithLogin()
        {
            var adress = new Uri("https://www.amazon.de/ap/signin?showRememberMe=false&openid.pape.max_auth_age=0&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&pageId=deflex&ignoreAuthState=1&openid.return_to=https%3A%2F%2Fwww.amazon.de%2F%3Fref_%3Dnav_signin&prevRID=T028TS6C1NMGVZ4Y8B9B&openid.assoc_handle=deflex&openid.mode=checkid_setup&openid.ns.pape=http%3A%2F%2Fspecs.openid.net%2Fextensions%2Fpape%2F1.0&prepopulatedLoginId=eyJjaXBoZXIiOiJ0d1VzTS9wRTQrcGM4WFY1NzQrdFp3PT0iLCJ2ZXJzaW9uIjoxLCJJViI6Ims3Sm9GMWJncVFjS29kaWIyQk1RK2c9PSJ9&failedSignInCount=0&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&timestamp=1579545861000");
            var browser = new BrowserView(adress, Username, _encryptedPassword);
            browser.Show();
        }

        [Obsolete]
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
                    // Update strength level
                    _passwordStrengthValue = PasswordHelper.CalculatePasswordStrength(_encryptedPassword, out var resultTips, out var isBlackListed);
                    IsBlackListed = isBlackListed;

                    LoadShieldUI();
                    // For saving's sake: Fill the LeafPassword of the current leafViewModel with the new pw. But encrypted!
                    if (CurrentItem.CurrentViewModel is LeafViewModel leafVm)
                    {
                        var leafPw = new LeafPassword { ForeignId = CurrentItem.CurrentViewModel.Id, EncryptedValue = _encryptedPassword };
                        leafVm.LeafPasswordCopy = leafPw;
                    }
                    CurrentItem.IsDirty = true;
                    _currentBase.ChangesTracker++;
                    TaskLogger.Instance.Track($"{CurrentItem.Name}: Changed password.");
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while editing password-entry: {ex}");
                ex.SetUserMessage("An error occured while trying to edit the item. The memory is being flushed to prevent any leaks.");
                Communication.InformUserAboutError(ex);
            }
        }

        [Obsolete]
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
            Websites.Add(amazonVm);
        }

    }
}
