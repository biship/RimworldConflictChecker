using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RimworldConflictChecker
{
    public class Program
    {
        public static int Formrc { get; set; }
        public static string[] Allargs { get; set; }
        public static bool incDisabled = false;
        // Satisfies rule: MarkWindowsFormsEntryPointsWithStaThread.
        [STAThread]
        private static int Main(string[] args)
        {
            Allargs = args;
            var rimworldfolder = "";
            //string[] modfolders = { };
            var modfolder1 = "";
            var modfolder2 = "";
            var modsconfigfolder = "";
            //var incDisabled = false;

            // Uncomment the following after testing to see that NBug is working as configured
            NBug.Settings.ReleaseMode = true;

#if DEBUG
            NBug.Settings.WriteLogToDisk = true;
#endif

            var list = new List<NBug.Core.Util.Storage.FileMask>
            {
                "RCC.txt"
            };
            NBug.Settings.AdditionalReportFiles = list;

            //NBug.Exceptions.Dispatch();

            // Sample NBug configuration for console applications
            AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
            TaskScheduler.UnobservedTaskException += NBug.Handler.UnobservedTaskException;

            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += NBug.Handler.ThreadException;

            // Sample NBug configuration for WinForms applications
            // all set above
            //AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
            //Application.ThreadException += NBug.Handler.ThreadException;
            //TaskScheduler.UnobservedTaskException += NBug.Handler.UnobservedTaskException;

            if (args.Length == 1)
            {
                if (args[0] == "--help" || args[0] == "-help" || args[0] == "-h" || args[0] == "--h" || args[0] == "/?")
                {
                    Console.WriteLine("Rimworld Conflict Checker");
                    Console.WriteLine();
                    Console.WriteLine("Usage:");
                    Console.WriteLine("     RCC.exe [-all] [path(s)]");
                    Console.WriteLine();
                    Console.WriteLine("     -all   : Run as if all mods are enabled (ignoring what is set in ModsConfig.xml)");
                    Console.WriteLine("     [path] : Path(s) (each within quotes) seperated by spaces");
                    Console.WriteLine("              Where paths are : (required) Rimworld.exe location ");
                    Console.WriteLine("                                (optional) Rimworld Game Mod Folder");
                    Console.WriteLine("                                (optional) Steam Mod Folder");
                    Console.WriteLine("                                (optional) ModsConfig.xml location");
                    //Console.WriteLine();
                    Console.WriteLine("Example:");
                    Console.WriteLine("     RCC.exe \"D:\\SteamLibrary\\steamapps\\common\\RimWorld\" \"D:\\SteamLibrary\\steamapps\\workshop\\content\\294100\"");
                    Console.WriteLine();
                    Console.WriteLine("or just run RCC.exe without parameters to get a folder chooser");
                    return 1;
                }
            }
            Console.WriteLine("Rimworld Conflict Checker Starting");
            Formrc = 2;

            if (args.Length != 0)
            {
                foreach (string arg in args)
                {
                    if (Utils.FileOrDirectoryExists(arg + "\\RimWorldWin.exe"))
                    {
                        rimworldfolder = arg;
                        Formrc = 0;
                        continue;
                    }
                    if (Utils.FileOrDirectoryExists(arg + "\\Core"))
                    {
                        modfolder1 = arg + "\\Mods";
                        Formrc = 0;
                        continue;
                    }
                    if (Utils.FileOrDirectoryExists(arg + "\\ModsConfig.xml"))
                    {
                        modsconfigfolder = arg;
                        Formrc = 0;
                        continue;
                    }
                    if (arg == "-all")
                    {
                        incDisabled = true;
                    }
                    modfolder2 = arg;
                    Formrc = 0;
                }
            }

            if (args.Length == 0 || ((incDisabled == true) && (args.Length == 1)))
           { 
                //run folder picker
                if (Settings.Default.UpgradeRequired)
                {
                    Settings.Default.Upgrade();
                    Settings.Default.UpgradeRequired = false;
                    Settings.Default.Save();
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (var optionsForm = new OptionsForm())
                {
                    Application.Run(optionsForm);
                }
                //set folder locations based on settings
                rimworldfolder = (string)Settings.Default["RimWorldFolder"];
                modfolder1 = (string)Settings.Default["ModFolder1"];
                modfolder2 = (string)Settings.Default["ModFolder2"];
                modsconfigfolder = (string)Settings.Default["ModsConfigFolder"];
                Formrc = OptionsForm.ReturnValue1;
                //Settings.Default.Upgrade();
            }
            //testing throwing exception
            //throw new ArgumentException("ha-ha");

            //this is terrible code. 
            //TODO: Put all this into the RimWorld object
            var mainprogram = new RimworldXmlLoader(incDisabled, rimworldfolder, modsconfigfolder, modfolder1, modfolder2);
            Logger.Instance.WriteToFile();
            if (mainprogram.Rc == 0)
            {
                RunWPF();
            }

            //testing throwing exception
            //throw new ArgumentException("ha-ha");

            return mainprogram.Rc;
        }

        // All WPF applications should execute on a single-threaded apartment (STA) thread
        [STAThread]
        public static void RunWPF()
        {

            //Windows forms:
            //Application.Run(new ResultsForm());
            //System.Windows.Forms.Application.Run(new ResultsForm());

            //WPF
            var form = new System.Windows.Application();
            form.Run(new ResultsWPF());
        }
    }
}