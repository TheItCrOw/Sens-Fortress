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
            var testVM = new TestViewModel{ Name = "Projects", TreeType=TreeDepth.Root };
            testVM.Children.Add(new TestViewModel { Name="SubProject", TreeType=TreeDepth.Branch});
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });

            var testVM2 = new TestViewModel { Name = "Projects", TreeType=TreeDepth.Root };
            var child = new TestViewModel { Name = "SubSubProjects", TreeType=TreeDepth.Leaf };
            testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType=TreeDepth.Branch,Children = new ObservableCollection<TestViewModel> { child} });
            testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch, Children = new ObservableCollection<TestViewModel> { child } });
            testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch, Children = new ObservableCollection<TestViewModel> { child } });
            testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType=TreeDepth.Branch });


            TestCollection.Add(testVM);
            TestCollection.Add(testVM2);
        }

        public ObservableCollection<TestViewModel> TestCollection { get; set; } = new ObservableCollection<TestViewModel>(); 

        //<= testing
        
    }
}
