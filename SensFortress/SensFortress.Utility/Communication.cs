using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Extensions;
using SensFortress.View.Main.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Utility
{
    /// <summary>
    /// Links the messages between backend and frontend.
    /// </summary>
    public static class Communication
    {
        /// <summary>
        /// Informs the user via error popup about a given exception.
        /// </summary>
        /// <param name="ex"></param>
        public static void InformUserAboutError(Exception ex)
        {
            var exceptionView = new ExceptionView(ex);
            exceptionView.Show();
        }

        /// <summary>
        /// Informs the user via info popup about the given situation.
        /// </summary>
        /// <param name="message"></param>
        public static void InformUser(string message)
        {
            var infoView = new InfoView(message);
            infoView.Show();
        }

        /// <summary>
        /// Asks the user via UI about a given question. Return true or false.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool AskForAnswer(string message)
        {
            var askForAnswerView = new AskForAnswerView(message);
            askForAnswerView.ShowDialog();

            return askForAnswerView.DialogResult.Value;
        }

        /// <summary>
        /// Takes in a string shows them safely on demand for the user.
        /// </summary>
        /// <param name="encryptedData"></param>
        public static void ShowSensibleData(string data)
        {
            // Implement this.
            var decryptedData = CryptMemProtection_ForUtilityProject.DecryptInMemoryData(new byte[21]);
        }

    }
}
