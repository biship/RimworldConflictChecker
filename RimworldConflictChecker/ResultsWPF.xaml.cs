using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace RimworldConflictChecker
{
    /// <summary>
    /// Interaction logic for ResultsWPF.xaml
    /// </summary>
    
    public partial class ResultsWPF
    {
        //public List<ModView> modsView { get; set; }
        public List<ModView> modsView { get; set; }

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
            modsView = new List<ModView>(); //new list of ModView

            var i = 0;

            //crashes if Mods is empty. wow what....
            try
            {
                foreach (var mod in RimworldXmlLoader.Mods)
                {
                    modsView.Add(new ModView());
                    modsView[i].index = i;
                    modsView[i].loadposition = mod.ModRank;
                    modsView[i].enabled = mod.ModEnabled;
                    modsView[i].version = mod.ModXmlDetails.ModTargetVersion;
                    modsView[i].modname = mod.ModXmlDetails.ModName;
                    modsView[i].nummodconflicts = mod.ConflictedMods.Count;
                    mod.ConflictedMods.Each((item, n) =>
                   {
                       modsView[i].modconflicts.Add(item.ModXmlDetails.ModName);
                   });
                    modsView[i].numdllconflicts = mod.ConflictedDlls.Count;
                    modsView[i].numcoreconflicts = mod.CoreOverrights;
                    modsView[i].numxmlfiles = mod.XmlFiles.Count;
                    modsView[i].moddir = mod.DirName;
                    modsView[i].fullmoddir = mod.FullDirName;
                    i++;
                }

                //tab1dataGrid1.ItemsSource = modsView;
                var viewmodsView = CollectionViewSource.GetDefaultView(modsView);
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
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}