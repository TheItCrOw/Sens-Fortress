using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SensFortress.Models.ViewModels
{
    public class TestViewModel
    {
        public string Name { get; set; }

        public ObservableCollection<TestSubViewModel> Children { get; set; } = new ObservableCollection<TestSubViewModel>();
    }

}
