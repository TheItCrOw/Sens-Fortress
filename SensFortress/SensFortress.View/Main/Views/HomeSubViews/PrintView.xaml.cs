using SensFortress.View.Main.ViewModel;
using SensFortress.View.Main.ViewModel.HomeSubVms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace SensFortress.View.Main.Views.HomeSubViews
{
    /// <summary>
    /// Interaktionslogik für PrintView.xaml
    /// </summary>
    public partial class PrintView : Window
    {
        public PrintView(FixedDocumentSequence printDocument)
        {
            InitializeComponent();
            Preview_Document.Document = printDocument;
        }
    }
}
