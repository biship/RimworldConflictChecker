using System.Windows.Forms;

namespace RimworldConflictChecker
{
    public partial class ResultsForm : Form
    {
        public ResultsForm()
        {
            Load += ResultsForm_Load;
            InitializeComponent();
            //dataGridView1.Sort(this.dataGridView1.Columns["modRankDataGridViewTextBoxColumn"], ListSortDirection.Ascending);
        }
        private void ResultsForm_Load(object sender, System.EventArgs e)
        {
            //SortableBindingList ?
            dataGridView1.DataSource = RimworldXmlLoader.Mods;
         }
    }
}
