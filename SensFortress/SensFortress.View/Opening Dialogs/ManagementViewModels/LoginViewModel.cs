using Microsoft.Win32;
using Prism.Commands;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using SensFortress.View.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SensFortress.View.Opening_Dialogs.ViewModels
{
    public class LoginViewModel : ViewModelManagementBase
    {
        #region fields
        private FortressViewModel _selectedFortess;
        private bool _isLocked;
        #endregion

        /// <summary>
        /// Shows all loaded <see cref="FortressViewModel"/> in UI.
        /// </summary>
        public ObservableCollection<FortressViewModel> Fortresses { get; set; } = new ObservableCollection<FortressViewModel>();

        /// <summary>
        /// Command to add an externally selected fortress to the fortressListConfigFile that holds linked paths.
        /// </summary>
        public DelegateCommand LinkExternalFortressCommand => new DelegateCommand(LinkExternalFortress);

        /// <summary>
        /// Holds the currently selected FortressViewModel
        /// </summary>
        public FortressViewModel SelectedFortress
        {
            get { return _selectedFortess; }
            set
            {
                SetProperty(ref _selectedFortess, value);
            }
        }

        public override bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                SetProperty(ref _isLocked, value);
            }
        }

        public LoginViewModel()
        {
            Navigation.LoginManagementInstance = this;
            LoadFortresses();
        }

        /// <summary>
        /// Delinks an externally added fortress from the list.
        /// </summary>
        /// <param name="deletableVm"></param>
        public void DelinkExternalFortress(FortressViewModel deletableVm)
        {
            try
            {
                if (!File.Exists(IOPathHelper.GetLinkedFortressListFile()))
                    return;

                var linkedFortessFile = File.ReadAllLines(IOPathHelper.GetLinkedFortressListFile()).ToList();

                if (linkedFortessFile.Contains(deletableVm.FullName))
                {
                    linkedFortessFile.Remove(deletableVm.FullName);
                }

                File.WriteAllLines(IOPathHelper.GetLinkedFortressListFile(), linkedFortessFile);
                Fortresses.Remove(deletableVm);
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while de-linking a fortress: {ex}");
                ex.SetUserMessage($"Couldn't de-link fortress - An error occured while trying to de-link it. The problem could be solved by restarting the program.");
                Communication.InformUserAboutError(ex);
            }
        }

        /// <summary>
        /// Adds an external fortress to the FotressListConfig file.
        /// </summary>
        private void LinkExternalFortress()
        {
            try
            {
                // Open a file dialog
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = $"Fortress (*{TermHelper.GetZippedFileEnding()}) | *{TermHelper.GetZippedFileEnding()}";
                openFileDialog.ShowDialog();

                if (openFileDialog.FileNames.Count() == 0)
                    return;

                if (File.Exists(IOPathHelper.GetLinkedFortressListFile()))
                {
                    var linkedFortressesFile = File.ReadAllLines(IOPathHelper.GetLinkedFortressListFile()).ToList();

                    foreach (var path in openFileDialog.FileNames)
                    {
                        if (linkedFortressesFile.Contains(path))
                        {
                            Communication.InformUser("You already added this fortress.");
                            return;
                        }
                        linkedFortressesFile.Add(path);
                    }
                    File.WriteAllLines(IOPathHelper.GetLinkedFortressListFile(), linkedFortressesFile);
                }
                // If the linkedFortressesFile doesn't exist - write it.
                else
                {
                    IOPathHelper.CreateDirectory(IOPathHelper.GetLinkedFortressListDirectory());
                    File.WriteAllLines(IOPathHelper.GetLinkedFortressListFile(), openFileDialog.FileNames);
                }

                Logger.log.Info($"Linked external fortress: {openFileDialog.FileName}.");
                LoadFortresses();
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while linking a fortress: {ex}");
                ex.SetUserMessage($"Couldn't add fortress - An error occured while trying to link it. Make sure the selected file ends with {TermHelper.GetZippedFileEnding()} or try to restart the program.");
                Communication.InformUserAboutError(ex);
            }
        }

        /// <summary>
        /// Loads all fortresses: Default and linked.
        /// </summary>
        public void LoadFortresses()
        {
            try
            {
                // If the fortress folder doesn't exist, a fortress hasn't been created into the default location
                if (!Directory.Exists(IOPathHelper.GetDefaultFortressDirectory()) && !File.Exists(IOPathHelper.GetLinkedFortressListFile()))
                    return;

                var allFortresses = new List<string>();

                // First get all fortresses in the default location
                var defaultFortresses = Directory.GetFiles(IOPathHelper.GetDefaultFortressDirectory()).Where(f => f.EndsWith(TermHelper.GetZippedFileEnding())).ToList();
                allFortresses.AddRange(defaultFortresses);

                // Now look for externally added fortress in the fortress config
                if (File.Exists(IOPathHelper.GetLinkedFortressListFile()))
                {
                    var linkedFortresses = File.ReadAllLines(IOPathHelper.GetLinkedFortressListFile()).ToList();
                    var emptyPaths = new List<string>();

                    foreach (var path in linkedFortresses)
                    {
                        // If the file exists, we add it to the UI.
                        if (File.Exists(path))
                        {
                            allFortresses.Add(path);
                        }
                        // If the given path does not exist anymore, because the fortress got moved, delete it.
                        else
                        {
                            emptyPaths.Add(path);
                        }
                    }

                    // When empty paths have been found, delete them and tell the User.
                    if (emptyPaths.Count > 0)
                    {
                        foreach (var path in emptyPaths)
                        {
                            linkedFortresses.Remove(path);
                        }
                        File.WriteAllLines(IOPathHelper.GetLinkedFortressListFile(), linkedFortresses);
                        Communication.InformUser($"Old or corrupted paths have been found - they were de-linked from the fortress list.");
                    }
                }

                Fortresses.Clear();

                foreach (var fortress in allFortresses)
                {
                    // If the files does not have the default database ending, skip it.
                    if (!fortress.EndsWith(TermHelper.GetZippedFileEnding()))
                        break;

                    var created = File.GetCreationTime(fortress);
                    var modified = File.GetLastWriteTime(fortress);
                    var fortressVm = new FortressViewModel(fortress, created, modified, this);
                    if (!fortress.Contains(IOPathHelper.GetDefaultFortressDirectory()))
                        fortressVm.IsDefaultLocated = false;

                    Fortresses.Add(fortressVm);
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while loading the fortress list: {ex}");
                ex.SetUserMessage("An error occured while trying to load all known fortresses. If the fortress has been moved, try to select it again or restart the program.");
                Communication.InformUserAboutError(ex);
            }
        }

    }
}
