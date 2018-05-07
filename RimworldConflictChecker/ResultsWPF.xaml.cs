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

    public partial class ResultsWpf
    {
        public List<ModView> ModsView { get; set; }
        private ICommand _saveCommand;
        public ResultsWpf()
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
                    ModsView[i].Index = i;
                    ModsView[i].Loadposition = mod.ModRank;
                    ModsView[i].Enabled = mod.ModEnabled;
                    ModsView[i].Version = mod.ModXmlDetails.ModTargetVersion;
                    ModsView[i].Modname = mod.ModXmlDetails.ModName;
                    ModsView[i].Nummodconflicts = mod.ConflictedMods.Count;
                    mod.ConflictedMods.Each((item, n) =>
                   {
                       ModsView[i].Modconflicts.Add(item.ModXmlDetails.ModName);
                   });
                    ModsView[i].Numdllconflicts = mod.ConflictedDlls.Count;
                    ModsView[i].Numcoreconflicts = mod.CoreOverrights;
                    ModsView[i].Numxmlfiles = mod.XmlFiles.Count;
                    ModsView[i].Moddir = mod.DirName;
                    ModsView[i].Fullmoddir = mod.FullDirName;
                    i++;
                }

                //tab1dataGrid1.ItemsSource = modsView;
                var viewmodsView = CollectionViewSource.GetDefaultView(ModsView);
                viewmodsView.SortDescriptions.Add(new SortDescription("Loadposition", ListSortDirection.Ascending)); //initial sort

                //filtering. needs INotifyCollectionChanged to update display...
                BindingErrorTraceListener.SetTrace();
                tab1dataGrid1.ItemsSource = viewmodsView; //instead of ItemsSource="{Binding viewmodsView}"
                //from datagrid1 removed Style="{DynamicResource DGHeaderStyle}"

                //modsView.CustomSort = new CustomerSorter();
                //modsView.SortDescriptions.Add(new SortDescription("loadposition", ListSortDirection.Ascending));


                //tab2dataGrid1.UpdateLayout();
                //tab2grid1.UpdateLayout();
                //foreach (DataGridColumn c in tab2dataGrid1.Columns)
                //c.Width = DataGridLength.Auto;

                MyDatagrid.ItemsSource = viewmodsView;

                //InitializeComponent();

            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("Crash in populating results form.", ex);
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
                        param => SaveObject(),
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
                using (var excelFile = new StreamWriter("C:\\Users\\" + Environment.UserName + "\\Desktop\\exportedcompanies.xls"))
                {
                    excelFile.WriteLine(result.Replace(',', ' '));
                    //excelFile.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("Crash in copy all command on results form.", ex);
            }
        }
    }
}