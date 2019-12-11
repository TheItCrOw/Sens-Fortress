using Prism.Mvvm;
using SensFortress.Models.ViewModels;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    public class HomeViewModel : ViewModelManagementBase
    {

        //=> testing
        public HomeViewModel()
        {
            var testVM = new TestViewModel{ Name = "Projects" };
            testVM.Children.Add(new TestSubViewModel { Name="SubProject"});
            TestCollection.Add(testVM);
        }

        public ObservableCollection<TestViewModel> TestCollection { get; set; } = new ObservableCollection<TestViewModel>(); 

        //<= testing
        
    }
}
