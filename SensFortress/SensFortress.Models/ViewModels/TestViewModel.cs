using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SensFortress.Models.ViewModels
{
    public class TestViewModel
    {
        public string Name { get; set; }

        public TreeDepth TreeType { get; set; }

        public ObservableCollection<TestViewModel> Children { get; set; } = new ObservableCollection<TestViewModel>();
    }

    public enum TreeDepth
    {
        Root,
        Branch,
        Leaf
    }

}
