using SensFortress.Utility.Extensions;
using SensFortress.Utility.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SensFortress.Utility.Exceptions
{
    /// <summary>
    /// Interaktionslogik für SensibleDataView.xaml
    /// </summary>
    public partial class SensibleDataView : Window
    {
        private bool _showsSensible;
        private byte[] _encryptedData;
        public SensibleDataView(byte[] encryptedData, string infoText, string title = "Info:")
        {
            InitializeComponent();

            Title_Textblock.Text = title;
            Sensible_Textblock.Text = "*********************************";
            Info_Textblock.Text = infoText;
            _encryptedData = encryptedData;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Topmost = true;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            Sensible_Textblock.Text = string.Empty;
            _encryptedData = null;
            Close();
        }

        private void ShowSensible_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_showsSensible)
                {
                    var decrypted = CryptMemProtection_ForUtilityProject.DecryptInMemoryData(_encryptedData);
                    Sensible_Textblock.Text = ByteHelper.ByteArrayToString(decrypted);
                    decrypted = null;
                    _showsSensible = true;
                }
                else
                {
                    Sensible_Textblock.Text = "*********************************";
                    _showsSensible = false;
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Couldn't handle sensible data: {ex}");
                ex.SetUserMessage(WellKnownExceptionMessages.DataExceptionMessage());
                Communication.InformUserAboutError(ex);
                _encryptedData = null;
                Close();
            }
        }

    }
}
