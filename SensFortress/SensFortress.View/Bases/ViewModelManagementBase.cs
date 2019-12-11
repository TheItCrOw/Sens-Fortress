using Prism.Mvvm;
using SensFortress.View.Main.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.View.Bases
{
    public class ViewModelManagementBase : BindableBase
    {

        public void InformUserAboutError(Exception ex)
        {
            var exceptionView = new ExceptionView(ex);
            exceptionView.Show();
        }

    }
}
