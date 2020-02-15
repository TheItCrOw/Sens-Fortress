using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Guardian
{
    public static class UtilityParameters
    {
        public static string FortressPath { get; private set; }
        /// <summary>
        /// Sets the initial parameters of the UtilityProject
        /// 0 = FortressPath
        /// </summary>
        /// <param name="param"></param>
        public static void HandleFortressParameters(object[] param)
        {
            FortressPath = (string)param[0];
        }

    }
}
