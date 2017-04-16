using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RimworldConflictChecker
{
    /// <summary>
    /// Interaction logic for ResultsWPF.xaml
    /// </summary>

    public partial class ResultsWPF
    {
        //public List<ModView> modsView { get; set; }
        public List<ModView> ModsView { get; set; }
        private ICommand _saveCommand;
        public ResultsWPF()
        {
            //runs first
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //runs second
            //tab1dataGrid1.ItemsSource = RimworldXmlLoader.Mods;

            //List<ModView> modsView = new List<ModView>(); //new list of ModView
            ModsView = new List<ModView>(); //new list of ModView

            var i = 0;

            //crashes if Mods is empty. wow what....
            try
            {
                foreach (var mod in RimworldXmlLoader.Mods)
                {
                    ModsView.Add(new ModView());
                    ModsView[i].index = i;
                    ModsView[i].loadposition = mod.ModRank;
                    ModsView[i].enabled = mod.ModEnabled;
                    ModsView[i].version = mod.ModXmlDetails.ModTargetVersion;
                    ModsView[i].modname = mod.ModXmlDetails.ModName;
                    ModsView[i].nummodconflicts = mod.ConflictedMods.Count;
                    mod.ConflictedMods.Each((item, n) =>
                   {
                       ModsView[i].modconflicts.Add(item.ModXmlDetails.ModName);
                   });
                    ModsView[i].numdllconflicts = mod.ConflictedDlls.Count;
                    ModsView[i].numcoreconflicts = mod.CoreOverrights;
                    ModsView[i].numxmlfiles = mod.XmlFiles.Count;
                    ModsView[i].moddir = mod.DirName;
                    ModsView[i].fullmoddir = mod.FullDirName;
                    i++;
                }

                //tab1dataGrid1.ItemsSource = modsView;
                var viewmodsView = CollectionViewSource.GetDefaultView(ModsView);
                viewmodsView.SortDescriptions.Add(new SortDescription("loadposition", ListSortDirection.Ascending)); //initial sort

                //filtering. needs INotifyCollectionChanged to update display...

                tab1dataGrid1.ItemsSource = viewmodsView; //instead of ItemsSource="{Binding viewmodsView}"

                //modsView.CustomSort = new CustomerSorter();
                //modsView.SortDescriptions.Add(new SortDescription("loadposition", ListSortDirection.Ascending));


                //tab2dataGrid1.UpdateLayout();
                //tab2grid1.UpdateLayout();
                //foreach (DataGridColumn c in tab2dataGrid1.Columns)
                //c.Width = DataGridLength.Auto;
            }
            catch (Exception Ex)
            {
                Logger.Instance.LogError("Crash in populating results form.", Ex);
                //throw;
            }
        }

        public ICommand CopyAll
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        param => this.SaveObject(),
                        param => CanSave()
                    );
                }
                return _saveCommand;
            }
        }

        private static bool CanSave()
        {
            // Verify command can be executed here
            return true;
        }

        private void SaveObject()
        {
            // Save command execution logic
            try
            {
                tab1dataGrid1.SelectAllCells();
                tab1dataGrid1.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, tab1dataGrid1);
                var resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                var result = (string)Clipboard.GetData(DataFormats.Text);
                tab1dataGrid1.UnselectAllCells();
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                using (var excelFile = new StreamWriter(@"C:\Users\" + Environment.UserName + @"\Desktop\exportedcompanies.xls"))
                {
                    excelFile.WriteLine(result.Replace(',', ' '));
                    //excelFile.Close();
                }
            }
            catch (Exception Ex)
            {
                Logger.Instance.LogError("Crash in copy all command on results form.", Ex);
            }
        }
    }
}