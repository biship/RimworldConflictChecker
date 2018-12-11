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
        private string _lastTxtbRimworldFolder;
        private string _lastTxtbModFolder1;
        private string _lastTxtbModFolder2;
        private string _lastTxtbModsConfigFolder;

        public static int ReturnValue1 { get; set; }

        public OptionsForm()
        {
            InitializeComponent();
            //testing
            //reset_settings();
            txtb_RimworldFolder.Text = Settings.Default.RimWorldFolder;
            _lastTxtbRimworldFolder = Settings.Default.RimWorldFolder;
            txtb_ModFolder1.Text = Settings.Default.ModFolder1;
            _lastTxtbModFolder1 = Settings.Default.ModFolder1;
            txtb_ModFolder2.Text = Settings.Default.ModFolder2;
            _lastTxtbModFolder2 = Settings.Default.ModFolder2;
            txtb_ModsConfigFolder.Text = Settings.Default.ModsConfigFolder;
            _lastTxtbModsConfigFolder = Settings.Default.ModsConfigFolder;
            checkBox1.Checked = Settings.Default.incDisabled;
            checkBox2.Checked = Settings.Default.showDetails;
            ReturnValue1 = 2;
            UpdateFoldersDisplay(txtb_RimworldFolder); //run on every = above
            UpdateFoldersDisplay(txtb_ModFolder1); //run on every = above
            UpdateFoldersDisplay(txtb_ModFolder2); //run on every = above
            UpdateFoldersDisplay(txtb_ModsConfigFolder); //run on every = above
            //UpdateFoldersDisplay(); //not needed as it runs on every text change above
            //form now displays
        }


        private void Btn_RimworldFolder_Click(object sender, EventArgs e)
        {
            //old
            //FolderBrowserDialog chooseDialog = new FolderBrowserDialog();
            //chooseDialog.SelectedPath = txtb_RimworldFolder.Text;
            //chooseDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            //chooseDialog.SelectedPath = last_txtb_RimworldFolder;
            //chooseDialog.Description = "Please select the folder that contains RimWorldWin.exe";
            //DialogResult result = chooseDialog.ShowDialog();
            //last_txtb_RimworldFolder = chooseDialog.SelectedPath; //move down?

            using (var chooseDialog = new OpenFileDialog())
            {
                //chooseDialog.FileName = "RimWorldWin.exe";
                chooseDialog.Filter = "RimWorld|RimWorldWin*.exe";
                chooseDialog.CheckFileExists = true;
                chooseDialog.CheckPathExists = true;
                //chooseDialog.AutoUpgradeEnabled = false; //def:true. false = old windowsXP style. lol
                //chooseDialog.RestoreDirectory = true; //def:false. Makes sure that the value in Environment.CurrentDirectory will be reset before the OpenFileDialog closes.
                //If RestoreDirectory is set to false, then Environment.CurrentDirectory will be set to whatever directory the OpenFileDialog was last open to.
                //they can't store or use relative dirs here, so its safe to ignore.
                //chooseDialog.InitialDirectory = chooseDialog.FileName.Remove(chooseDialog.FileName.LastIndexOf("\\"));// THIS LINE IS IMPORTANT. Why?
                if (Utils.FileOrDirectoryExists(_lastTxtbRimworldFolder))
                {
                    //chooseDialog.InitialDirectory = last_txtb_RimworldFolder ?? "C:\\"; //do these cause crashes?
                    chooseDialog.InitialDirectory = _lastTxtbRimworldFolder; //do these cause crashes?
                }
                else
                {
                    if (Utils.FileOrDirectoryExists(txtb_RimworldFolder.Text))
                    {
                        chooseDialog.InitialDirectory = txtb_RimworldFolder.Text; //do these cause crashes?
                    }
                }

                //chooseDialog.CustomPlaces.Add(last_txtb_RimworldFolder ?? "C:\\"); //adds to the dropdown //do these cause crashes?
                //chooseDialog.CustomPlaces.Add(txtb_RimworldFolder.Text ?? "C:\\"); //adds to the dropdown //do these cause crashes?
                chooseDialog.Title = "Please select RimWorldWin Executable";
                var result = chooseDialog.ShowDialog();
                //OK = Filename = full path & RimWorldWin.exe
                //Cancel = Filename = RimWorldWin.exe

                if (result == DialogResult.OK)
                {
                    //can only get here if they clicked ok & if RimWorldWin.exe was found
                    //Filename = full path & RimWorldWin.exe
                    //no need to check if file or folder exists
                    _lastTxtbRimworldFolder = GetDirectoryName(chooseDialog.FileName);
                    //txtb_RimworldFolder_status.Text = "OK";
                    //txtb_RimworldFolder_status.ForeColor = Color.Green;
                    var rimworldExeFolder = GetDirectoryName(chooseDialog.FileName);
                    txtb_RimworldFolder.Text = rimworldExeFolder;
                    //if (Utils.FileOrDirectoryExists(RimworldExeFolder + "\\Mods"))
                    //{
                    //auto-set mod folder
                    //txtb_ModFolder1_status.Text = "OK";
                    //txtb_ModFolder1_status.ForeColor = Color.Green;
                    txtb_ModFolder1.Text = rimworldExeFolder + "\\Mods";
                    //}
                    if (rimworldExeFolder.Contains("steamapps"))
                    {
                        var steamModFolder = GetDirectoryName(GetDirectoryName(rimworldExeFolder)) + "\\workshop\\content\\294100";
                        //if (Utils.FileOrDirectoryExists(SteamModFolder))
                        //{
                        //auto-set steam mod folder
                        //txtb_ModFolder2_status.Text = "OK";
                        //txtb_ModFolder2_status.ForeColor = Color.Green;
                        txtb_ModFolder2.Text = steamModFolder;
                        //}
                    }
                }
            }
            //UpdateFoldersDisplay();
        }

        private void Btn_ModFolder1_Click(object sender, EventArgs e)
        {
            using (var chooseDialog = new FolderBrowserDialog())
            {
                chooseDialog.RootFolder = Environment.SpecialFolder.MyComputer; //start browsing here
                if (Utils.FileOrDirectoryExists(_lastTxtbModFolder1))
                {
                    //chooseDialog.SelectedPath = last_txtb_RimworldFolder ?? "C:\\"; //do these cause crashes?
                    chooseDialog.SelectedPath = _lastTxtbModFolder1; //do these cause crashes?
                }
                else
                {
                    if (Utils.FileOrDirectoryExists(txtb_ModFolder1.Text))
                    {
                        chooseDialog.SelectedPath = txtb_ModFolder1.Text; //do these cause crashes?
                    }
                }
                chooseDialog.Description = "Please select the folder that contains the RimWorld Core Mods folder";
                var result = chooseDialog.ShowDialog();
                if (chooseDialog.SelectedPath != null)
                {
                    _lastTxtbModFolder1 = chooseDialog.SelectedPath; //move down?
                }

                if (result == DialogResult.OK)
                {
                    //if ((chooseDialog.SelectedPath.EndsWith("\\Mods")) && (Utils.FileOrDirectoryExists(chooseDialog.SelectedPath)))
                    //{
                    txtb_ModFolder1.Text = chooseDialog.SelectedPath;
                    //}
                    //else
                    //{
                    //tell user it doesnt exist.
                    //}
                }
            }
            //UpdateFoldersDisplay();
        }

        private void Btn_ModFolder2_Click(object sender, EventArgs e)
        {
            using (var chooseDialog = new FolderBrowserDialog())
            {
                chooseDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                if (Utils.FileOrDirectoryExists(_lastTxtbModFolder2))
                {
                    //chooseDialog.SelectedPath = last_txtb_RimworldFolder ?? "C:\\"; //do these cause crashes?
                    chooseDialog.SelectedPath = _lastTxtbModFolder2; //do these cause crashes?
                }
                else
                {
                    if (Utils.FileOrDirectoryExists(txtb_ModFolder2.Text))
                    {
                        chooseDialog.SelectedPath = txtb_ModFolder2.Text; //do these cause crashes?
                    }
                }
                chooseDialog.Description = "Please select the folder that contains the RimWorld steam workshop mod folder\n";
                var result = chooseDialog.ShowDialog();
                if (chooseDialog.SelectedPath != null)
                {
                    _lastTxtbModFolder2 = chooseDialog.SelectedPath; //move down?
                }

                if (result == DialogResult.OK)
                {
                    //if ((chooseDialog.SelectedPath.EndsWith("\\294100")) && (Utils.FileOrDirectoryExists(chooseDialog.SelectedPath)))
                    //{
                    txtb_ModFolder2.Text = chooseDialog.SelectedPath;
                    //}
                    //else
                    //{
                    //tell user it doesnt exist.
                    //}
                }
            }
            //UpdateFoldersDisplay();
        }

        private void Btn_ModsConfigFolder_Click(object sender, EventArgs e)
        {
            using (var chooseDialog = new OpenFileDialog())
            {
                chooseDialog.Filter = "ModsConfig|ModsConfig.xml";
                chooseDialog.CheckFileExists = true;
                chooseDialog.CheckPathExists = true;
                if (Utils.FileOrDirectoryExists(_lastTxtbModsConfigFolder))
                {
                    //chooseDialog.InitialDirectory = last_txtb_ModsConfigFolder ?? "C:\\"; //do these cause crashes?
                    chooseDialog.InitialDirectory = _lastTxtbModsConfigFolder; //do these cause crashes?
                }
                else
                {
                    if (Utils.FileOrDirectoryExists(txtb_ModsConfigFolder.Text))
                    {
                        chooseDialog.InitialDirectory = txtb_ModsConfigFolder.Text; //do these cause crashes?
                    }
                }

                //chooseDialog.CustomPlaces.Add(last_txtb_ModsConfigFolder ?? "C:\\"); //adds to the dropdown //do these cause crashes?
                //chooseDialog.CustomPlaces.Add(txtb_RimworldFolder.Text ?? "C:\\"); //adds to the dropdown //do these cause crashes?
                chooseDialog.Title = "Please select ModsConfig.xml";
                var result = chooseDialog.ShowDialog();
                //OK = Filename = full path & RimWorldWin.exe
                //Cancel = Filename = RimWorldWin.exe

                if (result == DialogResult.OK)
                {
                    //can only get here if they clicked ok & if RimWorldWin.exe was found
                    //Filename = full path & RimWorldWin.exe
                    //no need to check if file or folder exists
                    _lastTxtbModsConfigFolder = GetDirectoryName(chooseDialog.FileName);
                    //txtb_RimworldFolder_status.Text = "OK";
                    //txtb_RimworldFolder_status.ForeColor = Color.Green;
                    //var ModsConfigFolder = GetDirectoryName(chooseDialog.FileName);
                    //txtb_ModsConfigFolder.Text = ModsConfigFolder;
                    txtb_ModsConfigFolder.Text = _lastTxtbModsConfigFolder;
                }
            }
        }

        private void Btn_quit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            ReturnValue1 = 1;
            Close();
            //Application.Exit();  //closes WFM
            //Environment.Exit(1); //closes console app
        }

        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            //var rimworldfolder = (string)Settings.Default["RimWorldFolder"];
            //Properties.Settings.Default["SomeProperty"] = "Some Value";

            //set settings & save them (to xml in localappdata)
            Settings.Default.RimWorldFolder = txtb_RimworldFolder.Text;
            Settings.Default.ModFolder1 = txtb_ModFolder1.Text;
            Settings.Default.ModFolder2 = txtb_ModFolder2.Text;
            Settings.Default.ModsConfigFolder = txtb_ModsConfigFolder.Text;
            Settings.Default.incDisabled = checkBox1.Checked;
            Settings.Default.showDetails = checkBox2.Checked;
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
                //return; //redundant
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

        private void Btn_reset_Click(object sender, EventArgs e)
        {
            Reset_settings();
        }

        private void Reset_settings()
        {
            Settings.Default.Reset(); //removed all settings from users xml!
            Settings.Default.Save(); //saves empty nothing
            txtb_RimworldFolder.Text = Settings.Default.RimWorldFolder;
            _lastTxtbRimworldFolder = Settings.Default.RimWorldFolder;
            txtb_ModFolder1.Text = Settings.Default.ModFolder1;
            _lastTxtbModFolder1 = Settings.Default.ModFolder1;
            txtb_ModFolder2.Text = Settings.Default.ModFolder2;
            _lastTxtbModFolder2 = Settings.Default.ModFolder2;
            txtb_ModsConfigFolder.Text = Settings.Default.ModsConfigFolder;
            _lastTxtbModsConfigFolder = Settings.Default.ModsConfigFolder;
            checkBox1.Checked = Settings.Default.incDisabled;
            checkBox2.Checked = Settings.Default.showDetails;
            ReturnValue1 = 2;
        }

        private void UpdateFoldersDisplay(TextBox sender)
        {
            switch (sender.Name)
            {
                case nameof(txtb_RimworldFolder):
                    if (!string.IsNullOrEmpty(sender.Text))
                    {
                        if ((Utils.FileOrDirectoryExists(sender.Text)) && ((File.Exists(sender.Text + "\\RimWorldWin.exe")) || (File.Exists(sender.Text + "\\RimWorldWin64.exe"))))
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
                    break;
                case nameof(txtb_ModFolder1):
                    if (!string.IsNullOrEmpty(sender.Text))
                    {
                        if ((Utils.FileOrDirectoryExists(sender.Text)) && (sender.Text.EndsWith("\\Mods", StringComparison.OrdinalIgnoreCase)))
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
                    break;
                case nameof(txtb_ModFolder2):
                    if (!string.IsNullOrEmpty(sender.Text))
                    {
                        if ((Utils.FileOrDirectoryExists(sender.Text)) && (sender.Text.EndsWith("\\294100", StringComparison.OrdinalIgnoreCase)))
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
                    break;
                case nameof(txtb_ModsConfigFolder):
                    if (!string.IsNullOrEmpty(sender.Text))
                    {
                        if (Utils.FileOrDirectoryExists(sender.Text + "\\ModsConfig.xml"))
                        {
                            txtb_ModsConfigFolder_status.Text = Resources.OK;
                            txtb_ModsConfigFolder_status.ForeColor = Color.Green;
                        }
                        else
                        {
                            txtb_ModsConfigFolder_status.Text = Resources.InvalidFolder;
                            txtb_ModsConfigFolder_status.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        txtb_ModsConfigFolder_status.Text = "";
                    }
                    break;
                default:
                    break;
            }
        }

        private void Txtb_RimworldFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateFoldersDisplay(sender as TextBox);
        }

        private void Txtb_ModFolder1_TextChanged(object sender, EventArgs e)
        {
            UpdateFoldersDisplay(sender as TextBox);
        }

        private void Txtb_ModFolder2_TextChanged(object sender, EventArgs e)
        {
            UpdateFoldersDisplay(sender as TextBox);
        }

        private void Txtb_ModsConfigFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateFoldersDisplay(sender as TextBox);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //does this exist yet??
            //RimworldXmlLoader.Incdisabled = !RimworldXmlLoader.Incdisabled;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //does this exist yet??
            //RimworldXmlLoader.Showdetails = !RimworldXmlLoader.Showdetails;
        }

    }
}