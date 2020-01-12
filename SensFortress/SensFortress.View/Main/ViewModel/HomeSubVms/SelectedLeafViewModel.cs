using Prism.Commands;
using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
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

        public DelegateCommand ShowHidePasswordCommand => new DelegateCommand(ShowHidePassword);
        public TreeItemViewModel CurrentItem
        {
            get
            {
                return _currentIten;
            }
            set
            {
                SetProperty(ref _currentIten, value);
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                SetProperty(ref _password, value);
            }
        }
        /// <summary>
        /// Determines whether the fortress is currently locked.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return _isLocked;
            }
            set
            {
                SetProperty(ref _isLocked, value);
            }
        }
        /// <summary>
        /// Determines whether the user may see/use the password content
        /// </summary>
        public bool ShowContent
        {
            get
            {
                return _showContent;
            }
            set
            {
                SetProperty(ref _showContent, value);
                _pwIsHidden = false;
                Initialize();
            }
        }

        public SelectedLeafViewModel(TreeItemViewModel selectedLeaf)
        {
            //IsLocked = DataAccessService.FortressIsLocked;
            CurrentItem = selectedLeaf;
            IsLocked = true;
            ShowContent = false;
            if(!IsLocked)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            ShowHidePassword();
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
                    if (DataAccessService.Instance.TryGetSensible<LeafPassword>(CurrentItem.CurrentViewModel.Id, out var leafPw))
                    {
                        Password = ByteHelper.ByteArrayToString(leafPw.Value);
                        leafPw = null;
                    }
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
