using Prism.Mvvm;
using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Models.ViewModels;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SensFortress.View.Main.ViewModel
{
    public class HomeViewModel : ViewModelManagementBase
    {
        public ObservableCollection<BranchViewModel> RootNodes { get; set; } = new ObservableCollection<BranchViewModel>();

        public HomeViewModel()
        {
            try
            {
                LoadTreeView();
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error while loading HomeView: {ex}");
                ex.SetUserMessage("An error occured while trying to load data.");
                Communication.InformUserAboutError(ex);
            }
        }

        private void LoadTreeView()
        {
            var allBranchesVmDictionary = DataAccessService.Instance.GetAll<Branch>().Select(b => new BranchViewModel(b)).ToDictionary(b => b.Id, b => b);
            var allLeafesVm = DataAccessService.Instance.GetAll<Leaf>().Select(l => new LeafViewModel(l));
        }


        #region Testing
        //=> testing
        private void Testing()
        {
            var testVM = new TestViewModel { Name = "Projects", TreeType = TreeDepth.Root };
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });
            testVM.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });

            var testVM2 = new TestViewModel { Name = "Projects", TreeType = TreeDepth.Root };
            var child = new TestViewModel { Name = "SubSubProjects", TreeType = TreeDepth.Leaf };
            testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch, Children = new ObservableCollection<TestViewModel> { child } });
            testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch, Children = new ObservableCollection<TestViewModel> { child } });
            testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch, Children = new ObservableCollection<TestViewModel> { child } });
            testVM2.Children.Add(new TestViewModel { Name = "SubProject", TreeType = TreeDepth.Branch });


            TestCollection.Add(testVM);
            TestCollection.Add(testVM2);
        }

        public ObservableCollection<TestViewModel> TestCollection { get; set; } = new ObservableCollection<TestViewModel>();

        //<= testing
        #endregion
    }
}
