using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SensFortress.Models.ViewModels
{
    public class TestSubViewModel
    {

        public string Name { get; set; }

        public ObservableCollection<TestViewModel> Children { get; set; } = new ObservableCollection<TestViewModel>();

    }
}
