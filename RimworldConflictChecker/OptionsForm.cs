using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RimworldConflictChecker.Properties;
using static System.IO.Path;

namespace RimworldConflictChecker
{
    public partial class OptionsForm : Form
    {
        private string last_txtb_RimworldFolder;
        private string last_txtb_ModFolder1;
        private string last_txtb_ModFolder2;

        public static int ReturnValue1 { get; set; }

        public OptionsForm()
        {
            InitializeComponent();
            //testing
            //reset_settings();
            txtb_RimworldFolder.Text = Settings.Default.RimWorldFolder;
            last_txtb_RimworldFolder = Settings.Default.RimWorldFolder;
            txtb_ModFolder1.Text = Settings.Default.ModFolder1;
            last_txtb_ModFolder1 = Settings.Default.ModFolder1;
            txtb_ModFolder2.Text = Settings.Default.ModFolder2;
            last_txtb_ModFolder2 = Settings.Default.ModFolder2;
            ReturnValue1 = 2;
            //UpdateFoldersDisplay(); //not needed as it runs on every text change above
            //form now displays
        }


        private void btn_RimworldFolder_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialog chooseDialog = new FolderBrowserDialog();
            //chooseDialog.SelectedPath = txtb_RimworldFolder.Text;
            //chooseDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            //chooseDialog.SelectedPath = last_txtb_RimworldFolder;
            //chooseDialog.Description = "Please select the folder that contains RimWorldWin.exe";
            //DialogResult result = chooseDialog.ShowDialog();
            //last_txtb_RimworldFolder = chooseDialog.SelectedPath; //move down?

            OpenFileDialog chooseDialog = new OpenFileDialog();
            //chooseDialog.FileName = "RimWorldWin.exe";
            chooseDialog.Filter = "RimWorld|RimWorldWin.exe";
            chooseDialog.CheckFileExists = true;
            chooseDialog.CheckPathExists = true;
            chooseDialog.InitialDirectory = last_txtb_RimworldFolder;
            chooseDialog.CustomPlaces.Add(last_txtb_RimworldFolder); //adds to the dropdown
            chooseDialog.CustomPlaces.Add(txtb_RimworldFolder.Text); //adds to the dropdown
            chooseDialog.Title = "Please select RimWorldWin.exe";
            DialogResult result = chooseDialog.ShowDialog();
            //OK = Filename = full path & RimWorldWin.exe
            //Cancel = Filename = RimWorldWin.exe

            if (result == DialogResult.OK)
            {
                //can only get here if they clicked ok & if RimWorldWin.exe was found
                //Filename = full path & RimWorldWin.exe
                //no need to check if file or folder exists
                last_txtb_RimworldFolder = GetDirectoryName(chooseDialog.FileName);
                //txtb_RimworldFolder_status.Text = "OK";
                //txtb_RimworldFolder_status.ForeColor = Color.Green;
                var RimworldExeFolder = GetDirectoryName(chooseDialog.FileName);
                txtb_RimworldFolder.Text = RimworldExeFolder;
                //if (Osio.FileOrDirectoryExists(RimworldExeFolder + "\\Mods"))
                //{
                //auto-set mod folder
                //txtb_ModFolder1_status.Text = "OK";
                //txtb_ModFolder1_status.ForeColor = Color.Green;
                txtb_ModFolder1.Text = RimworldExeFolder + "\\Mods";
                //}
                if (RimworldExeFolder.Contains("steamapps"))
                {
                    var SteamModFolder = GetDirectoryName(GetDirectoryName(RimworldExeFolder)) + "\\workshop\\content\\294100";
                    //if (Osio.FileOrDirectoryExists(SteamModFolder))
                    //{
                    //auto-set steam mod folder
                    //txtb_ModFolder2_status.Text = "OK";
                    //txtb_ModFolder2_status.ForeColor = Color.Green;
                    txtb_ModFolder2.Text = SteamModFolder;
                    //}                    
                }
            }
            //UpdateFoldersDisplay();
        }

        private void btn_ModFolder1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog chooseDialog = new FolderBrowserDialog();
            chooseDialog.RootFolder = Environment.SpecialFolder.MyComputer; //start browsing here
            chooseDialog.SelectedPath = last_txtb_ModFolder1;
            chooseDialog.Description = "Please select the folder that contains the RimWorld Core Mods folder";
            DialogResult result = chooseDialog.ShowDialog();
            last_txtb_ModFolder1 = chooseDialog.SelectedPath; //move down?

            if (result == DialogResult.OK)
            {
                //if ((chooseDialog.SelectedPath.EndsWith("\\Mods")) && (Osio.FileOrDirectoryExists(chooseDialog.SelectedPath)))
                //{
                txtb_ModFolder1.Text = chooseDialog.SelectedPath;
                //}
                //else
                //{
                //tell user it doesnt exist.
                //}
            }
            //UpdateFoldersDisplay();
        }

        private void btn_ModFolder2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog chooseDialog = new FolderBrowserDialog();
            chooseDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            chooseDialog.SelectedPath = last_txtb_ModFolder2;
            chooseDialog.Description = "Please select the folder that contains the RimWorld steam workshop mod folder\n";
            DialogResult result = chooseDialog.ShowDialog();
            last_txtb_ModFolder2 = chooseDialog.SelectedPath; //move down?

            if (result == DialogResult.OK)
            {
                //if ((chooseDialog.SelectedPath.EndsWith("\\294100")) && (Osio.FileOrDirectoryExists(chooseDialog.SelectedPath)))
                //{
                    txtb_ModFolder2.Text = chooseDialog.SelectedPath;
                //}
                //else
                //{
                    //tell user it doesnt exist.
                //}
            }
            //UpdateFoldersDisplay(); 
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

        private void btn_reset_Click(object sender, EventArgs e)
        {
            reset_settings();
        }

        private void reset_settings()
        {
            Settings.Default.Reset(); //removed all settings from users xml!
            Settings.Default.Save(); //saves empty nothing
            txtb_RimworldFolder.Text = Settings.Default.RimWorldFolder;
            last_txtb_RimworldFolder = Settings.Default.RimWorldFolder;
            txtb_ModFolder1.Text = Settings.Default.ModFolder1;
            last_txtb_ModFolder1 = Settings.Default.ModFolder1;
            txtb_ModFolder2.Text = Settings.Default.ModFolder2;
            last_txtb_ModFolder2 = Settings.Default.ModFolder2;
            ReturnValue1 = 2;
            UpdateFoldersDisplay();
        }

        private void UpdateFoldersDisplay()
        {
            if (!string.IsNullOrEmpty(txtb_RimworldFolder.Text))
            {
                if ((Osio.FileOrDirectoryExists(txtb_RimworldFolder.Text)) && (File.Exists(txtb_RimworldFolder.Text + "\\RimWorldWin.exe")))
                {
                    txtb_RimworldFolder_status.Text = Resources.OK;
                    txtb_RimworldFolder_status.ForeColor = Color.Green;
                }
                else
                {
                    txtb_RimworldFolder_status.Text = Resources.InvalidFolder;
                    txtb_RimworldFolder_status.ForeColor = Color.Red;
                }
            }
            else
            {
                txtb_RimworldFolder_status.Text = Resources.MissingFolder;
                txtb_RimworldFolder_status.ForeColor = Color.Red;
            }
            if (!string.IsNullOrEmpty(txtb_ModFolder1.Text))
            {
                if ((Osio.FileOrDirectoryExists(txtb_ModFolder1.Text)) && (txtb_ModFolder1.Text.EndsWith("\\Mods")))
                {
                    txtb_ModFolder1_status.Text = Resources.OK;
                    txtb_ModFolder1_status.ForeColor = Color.Green;
                }
                else
                {
                    txtb_ModFolder1_status.Text = Resources.InvalidFolder;
                    txtb_ModFolder1_status.ForeColor = Color.Red;
                }
            }
            else
            {
                txtb_ModFolder1_status.Text = Resources.MissingFolder;
                txtb_ModFolder1_status.ForeColor = Color.Red;
            }
            if (!string.IsNullOrEmpty(txtb_ModFolder2.Text))
            {
                if ((Osio.FileOrDirectoryExists(txtb_ModFolder2.Text)) && (txtb_ModFolder2.Text.EndsWith("\\294100")))
                {
                    txtb_ModFolder2_status.Text = Resources.OK;
                    txtb_ModFolder2_status.ForeColor = Color.Green;
                }
                else
                {
                    txtb_ModFolder2_status.Text = Resources.InvalidFolder;
                    txtb_ModFolder2_status.ForeColor = Color.Red;
                }
            }
            else
            {
                txtb_ModFolder2_status.Text = "";
            }
        }

        private void txtb_RimworldFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateFoldersDisplay();
        }
        private void txtb_ModFolder1_TextChanged(object sender, EventArgs e)
        {
            UpdateFoldersDisplay();
        }

        private void txtb_ModFolder2_TextChanged_1(object sender, EventArgs e)
        {
            UpdateFoldersDisplay();
        }
    }
}