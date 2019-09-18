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
        //public static bool incdisabled = false;
        public static bool Dps;
        public static DateTime Starttime;

        //Win10 Insider fix 07/19/17
        //Tools -> Options -> Debugging -> General ->Uncheck: Enable UI Debugging Tools for XAML

        // Satisfies rule: MarkWindowsFormsEntryPointsWithStaThread.
        [STAThread]
        private static int Main(string[] args)
        {
            Allargs = args;
            var rimworldfolder = "";
            var modfolder1 = "";
            var modfolder2 = "";
            var modsconfigfolder = "";
            var incdisabled = false;
            var showdetails = false;
            Starttime = DateTime.Now;

            // Replace NBUG with https://github.com/google/breakpad one day?
            // Uncomment the following after testing to see that NBug is working as configured
            NBug.Settings.ReleaseMode = true;

#if DEBUG
            NBug.Settings.WriteLogToDisk = true;
#endif

            var list = new List<NBug.Core.Util.Storage.FileMask> {"RCC.txt"};
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
                    if (Utils.FileOrDirectoryExists(arg + "\\RimWorldWin.exe") || Utils.FileOrDirectoryExists(arg + "\\RimWorldWin64.exe") || Utils.FileOrDirectoryExists(arg + "\\RimWorld2150win64.exe"))
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
                        incdisabled = true;
                        Settings.Default.incDisabled = true;
                    }
                    if (arg == "-details")
                    {
                        showdetails = true;
                        Settings.Default.showDetails = true;
                    }
                    Dps |= arg == "-dps";
                    modfolder2 = arg;
                    Formrc = 0;
                }
            }

            //if (args.Length == 0 || ((incdisabled) && (args.Length == 1)))
            if (args.Length == 0)
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
                incdisabled = (bool)Settings.Default["incDisabled"];
                showdetails = (bool)Settings.Default["showDetails"];
                Formrc = OptionsForm.ReturnValue1;
                //Settings.Default.Upgrade();
            }
            //testing throwing exception
            //throw new ArgumentException("ha-ha");

            //this is terrible code.
            //TODO: Put all this into the RimWorld object
            var mainprogram = new RimworldXmlLoader(showdetails, incdisabled, rimworldfolder, modsconfigfolder, modfolder1, modfolder2);
            Logger.Instance.WriteToFile();
            if (mainprogram.Rc == 0)
            {
                RunWpf();
            }

            //testing throwing exception
            //throw new ArgumentException("ha-ha");
#if DEBUG
            //Process.Start(@"RCC.txt");
#endif
            return mainprogram.Rc;
        }

        // All WPF applications should execute on a single-threaded apartment (STA) thread
        [STAThread]
        public static void RunWpf()
        {

            //Windows forms:
            //Application.Run(new ResultsForm());
            //System.Windows.Forms.Application.Run(new ResultsForm());

            //WPF
            var form = new System.Windows.Application();
            form.Run(new ResultsWpf());
        }
    }
}