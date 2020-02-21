using SensFortress.Data.Database;
using SensFortress.Models.Fortress;
using SensFortress.Security;
using SensFortress.Utility;
using SensFortress.Utility.Exceptions;
using SensFortress.Utility.Log;
using SensFortress.View.Main.Views.HomeSubViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace SensFortress.View.Main.ViewModel.HomeSubVms
{
    public class PrintViewModel
    {
        public FixedDocumentSequence Document { get; set; }
        public ObservableCollection<PrintablePasswordEntryViewModel> PrintablePasswordEntries { get; set; } = new ObservableCollection<PrintablePasswordEntryViewModel>();

        /// <summary>
        /// Visualises a leafList to a printablePasswordEntity-List
        /// </summary>
        /// <param name="allLeafVms"></param>
        public void StartPasswordEntriesPrintProcess(HashSet<LeafViewModel> allLeafVms)
        {
            try
            {
                foreach (var leafVm in allLeafVms)
                {
                    if (DataAccessService.Instance.TryGetSensible<LeafPassword>(leafVm.Id, out var leafPw))
                    {
                        var pw = string.Empty;
                        if (leafPw.Value != null)
                            pw = ByteHelper.ByteArrayToString(leafPw.Value);
                        else
                            pw = ByteHelper.ByteArrayToString(CryptMemoryProtection.DecryptInMemoryData(leafPw.EncryptedValue));

                        // Get the parent model
                        var tuples = new Tuple<string, object>[] { Tuple.Create("Id", (object)leafVm.BranchId) };
                        var parent = DataAccessService.Instance.GetExplicit<Branch>(tuples).FirstOrDefault();

                        if (parent != default)
                        {
                            var printableVm = new PrintablePasswordEntryViewModel(leafVm.Name, parent.Name, leafVm.Username, pw);
                            PrintablePasswordEntries.Add(printableVm);
                        }

                        pw = null;
                        leafPw = null;
                    }
                }

                Print();
            }
            catch (Exception ex)
            {
                ex.SetUserMessage($"Couldn't print the requested document.");
                Communication.InformUserAboutError(ex);
                Logger.log.Error($"Error while trying to print emergency sheet: {ex}");
            }
        }

        /// <summary>
        /// Print the document
        /// </summary>
        private void Print()
        {
            // We need to build the flowdocument in code behind, otherwise there will be no paging with Lists and UIBlocks....
            var flowDocument = new FlowDocument();
            flowDocument.PageWidth = 650;
            flowDocument.PageHeight = 900;
            var title = new Paragraph(new Run($"{CurrentFortressData.FortressName} emergency sheet") { FontSize = 25 });
            var subtitle = new Paragraph(new Run("Category => Name => Username => Password") { FontSize = 12 });

            flowDocument.Blocks.Add(title);
            flowDocument.Blocks.Add(subtitle);
            var list = new List();

            foreach (var printableVm in PrintablePasswordEntries)
            {
                var pwEntry = $"{printableVm.Category} - {printableVm.Name} - {printableVm.Username}:{Environment.NewLine}{printableVm.Password}";
                var para = new Paragraph(new Run(pwEntry))
                {
                    Margin = new System.Windows.Thickness(12, 0, 12, 0)
                };
                list.ListItems.Add(new ListItem(para));
            }

            flowDocument.Blocks.Add(list);

            // It is crucial, that we create the xps file in memory only and never write it onto the harddrive.
            var package = Package.Open(new MemoryStream(), FileMode.Create, FileAccess.ReadWrite);
            var packUri = new Uri("pack://temp.xps");
            PackageStore.RemovePackage(packUri);
            PackageStore.AddPackage(packUri, package);

            var xpsDocument = new XpsDocument(package, CompressionOption.SuperFast, packUri.ToString());

            var paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;

            new XpsSerializationManager(new XpsPackagingPolicy(xpsDocument), false)
            .SaveAsXaml(paginator);

            Document = xpsDocument.GetFixedDocumentSequence();

            // Show the printable Document
            var printView = new PrintView(Document);
            printView.DataContext = this;
            printView.Show();
        }
    }
}
