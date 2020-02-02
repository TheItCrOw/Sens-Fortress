using SensFortress.Data.Database;
using SensFortress.Security.AES;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SensFortress.View.Main.Views
{
    /// <summary>
    /// Interaktionslogik für SaveFortressView.xaml
    /// </summary>
    public partial class SaveFortressView : Window
    {
        public SaveFortressView()
        {
            InitializeComponent();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private async void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() => InformationPanel_Textblock.Text = "Validating your rights...");
                    // Validate the key first.
                    if (DataAccessService.Instance.ValidateMasterkey(masterPasswordBox.Password))
                    {
                        var aesHelper = new AesHelper();
                        var hashedKey = aesHelper.CreateKey(masterPasswordBox.Password, 256, CurrentFortressData.Salt);
                        var secureMasterkey = new Masterkey(hashedKey);
                        hashedKey = null;

                        Application.Current.Dispatcher.Invoke(() => InformationPanel_Textblock.Text = "You have the correct keys my friend.");

                        IOPathHelper.CreateDirectory(IOPathHelper.GetBackedUpFortressDirectory()); // Make sure the directory exists.
                                                                                                   // Backup the fortress next:
                        Application.Current.Dispatcher.Invoke(() => InformationPanel_Textblock.Text = "Backup-ing your fortress...");
                        File.Copy(CurrentFortressData.FullPath, // source file...
                            System.IO.Path.Combine(IOPathHelper.GetBackedUpFortressDirectory(), $"(Backup){CurrentFortressData.FortressName}") // where to..
                            + TermHelper.GetZippedFileEnding(), // file ending...
                            true); // overwrite it...
                        Application.Current.Dispatcher.Invoke(() => InformationPanel_Textblock.Text = "Fortress backed up. Proceeding to save the fortress...");

                        // Now save the fortress.                   
                        DataAccessService.Instance.SaveFortress(secureMasterkey);

                        Application.Current.Dispatcher.Invoke(() => InformationPanel_Textblock.Text = "Fortress saved successfully.");

                        // Backup the fortress again with the newly saved changes.
                        File.Copy(CurrentFortressData.FullPath, // source file...
                            System.IO.Path.Combine(IOPathHelper.GetBackedUpFortressDirectory(), $"(Backup){CurrentFortressData.FortressName}") // where to..
                            + TermHelper.GetZippedFileEnding(), // file ending...
                            true); // overwrite it...

                        Thread.Sleep(1000); // Make the user see the result for a second.

                        Application.Current.Dispatcher.Invoke(() => DialogResult = true);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => InformationPanel_Textblock.Text = "The mission could not be executed.");
                        return;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                InformationPanel_Textblock.Text = "The mission could not be executed.";
                return;
            }
            finally
            {
                masterPasswordBox.Password = string.Empty; // Delete the password
            }
        }

    }
}
