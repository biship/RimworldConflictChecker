using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;


namespace RimworldConflictChecker
{
    /// <summary>
    /// Interaction logic for ResultsWPF2.xaml
    /// </summary>
    public partial class ResultsWPF2 : UserControl
    {
        public List<ModView> ModsView2 { get; set; }
        public ResultsWPF2()
        {
            InitializeComponent();
        }
        
        // Custom constructor to pass expense report data
        public ResultsWPF2(object data):this()
        {
            // Bind to expense report data.
            //this.DataContext = data;
            ModsView2 = new List<ModView>(); //new list of ModView

            var i = 0;

            //crashes if Mods is empty. wow what....
            try
            {
                foreach (var mod in RimworldXmlLoader.Mods)
                {
                    ModsView2.Add(new ModView());
                    ModsView2[i].Index = i;
                    ModsView2[i].Loadposition = mod.ModRank;
                    ModsView2[i].Enabled = mod.ModEnabled;
                    ModsView2[i].Version = mod.ModXmlDetails.ModTargetVersion;
                    ModsView2[i].Modname = mod.ModXmlDetails.ModName;
                    ModsView2[i].Nummodconflicts = mod.ConflictedMods.Count;
                    mod.ConflictedMods.Each((item, n) =>
                   {
                       ModsView2[i].Modconflicts.Add(item.ModXmlDetails.ModName);
                   });
                    ModsView2[i].Numdllconflicts = mod.ConflictedDlls.Count;
                    ModsView2[i].Numcoreconflicts = mod.CoreOverrights;
                    ModsView2[i].Numxmlfiles = mod.XmlFiles.Count;
                    ModsView2[i].Moddir = mod.DirName;
                    ModsView2[i].Fullmoddir = mod.FullDirName;
                    i++;
                }

                //tab1dataGrid1.ItemsSource = modsView;
                var viewmodsView = CollectionViewSource.GetDefaultView(ModsView2);
                viewmodsView.SortDescriptions.Add(new SortDescription("Loadposition", ListSortDirection.Ascending)); //initial sort

                //filtering. needs INotifyCollectionChanged to update display...
                BindingErrorTraceListener.SetTrace();
                //fix:
                //tab1dataGrid1.ItemsSource = viewmodsView; //instead of ItemsSource="{Binding viewmodsView}"
                //from datagrid1 removed Style="{DynamicResource DGHeaderStyle}"

                //modsView.CustomSort = new CustomerSorter();
                //modsView.SortDescriptions.Add(new SortDescription("loadposition", ListSortDirection.Ascending));


                //tab2dataGrid1.UpdateLayout();
                //tab2grid1.UpdateLayout();
                //foreach (DataGridColumn c in tab2dataGrid1.Columns)
                //c.Width = DataGridLength.Auto;

                //or fix:
                //MyDatagrid.ItemsSource = viewmodsView;

                //InitializeComponent();

            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("Crash in populating results form.", ex);
                //throw;
            }
        }
     }
}
