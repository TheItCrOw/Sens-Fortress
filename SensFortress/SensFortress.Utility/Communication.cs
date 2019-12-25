using SensFortress.Utility.Exceptions;
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

            if (askForAnswerView.DialogResult == true)
                return true;
            else if(askForAnswerView.DialogResult == false)
                return false;

            return false;
        }

    }
}
