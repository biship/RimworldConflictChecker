using System;
using System.Configuration;
using System.Windows.Forms;

namespace RimworldConflictChecker
{
    public partial class OptionsForm : Form
    {

        public static int ReturnValue1 { get; set; }

        public OptionsForm()
        {
            InitializeComponent();
            txtb_RimworldFolder.Text = Settings.Default.RimWorldFolder;
            txtb_ModFolder1.Text = Settings.Default.ModFolder1;
            txtb_ModFolder2.Text = Settings.Default.ModFolder2;
            ReturnValue1 = 2;
        }


        private void txtb_RimworldFolder_TextChanged(object sender, EventArgs e)
        {
        }

        private void btn_RimworldFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog chooseDialog = new FolderBrowserDialog();
            chooseDialog.SelectedPath = txtb_RimworldFolder.Text;
            chooseDialog.Description = "Please select the folder that contains RimWorldWin.exe";
            DialogResult result = chooseDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtb_RimworldFolder.Text = chooseDialog.SelectedPath;
                if (chooseDialog.SelectedPath.EndsWith("\\"))
                {
                    txtb_ModFolder1.Text = chooseDialog.SelectedPath + "Mods";
                }
                else
                {
                    txtb_ModFolder1.Text = chooseDialog.SelectedPath + "\\Mods";
                }
            }
        }

        private void btn_ModFolder1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog chooseDialog = new FolderBrowserDialog();
            chooseDialog.SelectedPath = txtb_ModFolder1.Text;
            chooseDialog.Description = "Please select the folder that contains the RimWorld Core mod folder";
            DialogResult result = chooseDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtb_ModFolder1.Text = chooseDialog.SelectedPath;
            }
        }

        private void btn_quit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            ReturnValue1 = 1;
            Close(); 
            //Application.Exit();  //closes WFM 
            //Environment.Exit(1); //closes console app
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            //var rimworldfolder = (string)Settings.Default["RimWorldFolder"];
            //Properties.Settings.Default["SomeProperty"] = "Some Value";

            //set settings & save them (to xml ion localappdata)
            Settings.Default.RimWorldFolder = txtb_RimworldFolder.Text;
            Settings.Default.ModFolder1 = txtb_ModFolder1.Text;
            Settings.Default.ModFolder2 = txtb_ModFolder2.Text;
            Settings.Default.Save();
            ReturnValue1 = 0;
            Close();
            //Application.Exit(); //closes WFM
        }

        public static void AddOrUpdateAppSettings(string key, string value)
        {
            //may use this one day
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void btn_ModFolder2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog chooseDialog = new FolderBrowserDialog();
            chooseDialog.SelectedPath = txtb_ModFolder2.Text;
            chooseDialog.Description = "Please select the folder that contains the second RimWorld mod folder\n";
            DialogResult result = chooseDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtb_ModFolder2.Text = chooseDialog.SelectedPath;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //runs on ok, quit, X and esc
            //base.OnFormClosing(e);

            //if (e.CloseReason == CloseReason.WindowsShutDown) return;
            
            //if (e.CloseReason == CloseReason.ApplicationExitCall)
            if (ReturnValue1 == 2)
            {
                ReturnValue1 = 1;
                return;
            }

            // Confirm user wants to close
            //switch (MessageBox.Show(this, "Are you sure you want to Quit?", "Quit", MessageBoxButtons.YesNo))
            //{
                //case DialogResult.No:
                    //e.Cancel = true;
                    //break;
                //default:
                    //Application.Exit();  //closes WFM
                    //Environment.Exit(1); //closes console app
                  //  break;
            //}
        }
    }
}