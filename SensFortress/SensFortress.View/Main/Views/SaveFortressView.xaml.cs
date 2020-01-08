using SensFortress.Data.Database;
using SensFortress.Security.AES;
using SensFortress.Utility;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InformationPanel_Textblock.Text = "Validating your rights...";
                // Validate the key first.
                if (DataAccessService.Instance.ValidateMasterkey(masterPasswordBox.Password))
                {
                    var aesHelper = new AesHelper();
                    var hashedKey = aesHelper.CreateKey(masterPasswordBox.Password, 512, DataAccessService.Instance.CurrentFortressSalt);
                    var secureMasterkey = new Masterkey(hashedKey);
                    hashedKey = null;
                    InformationPanel_Textblock.Text = "You have the correct keys my friend.";

                    IOPathHelper.CreateDirectory(IOPathHelper.GetBackedUpFortressDirectory()); // Make sure the directory exists.
                                                                                               // Backup the fortress next:
                    InformationPanel_Textblock.Text = "Backup-ing your fortress...";
                    File.Copy(DataAccessService.Instance.CurrentFortressGeneralData[0], // source file...
                        System.IO.Path.Combine(IOPathHelper.GetBackedUpFortressDirectory(), $"(Backup){DataAccessService.Instance.CurrentFortressGeneralData[1]}") // where to..
                        + TermHelper.GetZippedFileEnding(), // file ending...
                        true); // overwrite it...
                    InformationPanel_Textblock.Text = "Fortress backed up. Proceeding to save the fortress...";                    

                    // Now save the fortress.                   
                    DataAccessService.Instance.SaveFortress(secureMasterkey, true);
                   
                    DialogResult = true;
                }
                else
                {
                    InformationPanel_Textblock.Text = "The mission could not be executed.";
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                InformationPanel_Textblock.Text = "The mission could not be executed.";
                return;
            }
            finally
            {
                masterPasswordBox.Password = string.Empty;
            }
        }

    }
}
