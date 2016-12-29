using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RimworldConflictChecker
{
    public class Program
    {
        public static int formrc { get; set; }

        // Satisfies rule: MarkWindowsFormsEntryPointsWithStaThread.
        [STAThread]
        private static int Main(string[] args)
        {
            var rimworldfolder = "";
            var modfolder1 = "";
            var modfolder2 = "";

            // Uncomment the following after testing to see that NBug is working as configured
            NBug.Settings.ReleaseMode = true;

            var list = new List<NBug.Core.Util.Storage.FileMask>();
            list.Add("RCC.txt");
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
                if (args[0] == "--help" || args[0] == "-help" || args[0] == "-h" || args[0] == "/?")
                {
                    Console.WriteLine("Rimworld Conflict Checker");
                    Console.WriteLine();
                    Console.WriteLine("Usage:");
                    Console.WriteLine("\tRCC.exe \"<path.to.RimWorldWin.exe>\" (optional)\"<path.to.steam.mod.folder>\"");
                    Console.WriteLine();
                    Console.WriteLine("Example:");
                    Console.WriteLine("\tRCC.exe \"D:\\SteamLibrary\\steamapps\\common\\RimWorld\" \"D:\\SteamLibrary\\steamapps\\workshop\\content\\294100\"");
                    Console.WriteLine();
                    Console.WriteLine("or just run RCC.exe without parameters to get a folder picker & config saver");
                    return 1;
                }
            }
            Console.WriteLine("Rimworld Conflict Checker Starting");
            formrc = 2;

            switch (args.Length)
            {
                case 1:
                    rimworldfolder = args[0];
                    modfolder1 = args[0] +"\\Mods";
                    formrc = 0;
                    //modfolder1 (no steam folder)
                    break;
                case 2:
                    rimworldfolder = args[0];
                    modfolder1 = args[0] + "\\Mods";
                    modfolder2 = args[1];
                    formrc = 0;
                    break;
                default:
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
                    formrc = OptionsForm.ReturnValue1;
                    //Settings.Default.Upgrade();
                    break;
            }
            //testing throwing exception
            //throw new ArgumentException("ha-ha");

            var mainprogram = new RimworldXmlLoader(rimworldfolder, modfolder1, modfolder2);
            Logger.Instance.WriteToFile();
            if (mainprogram.Rc == 0)
            {
                runWPF();
            }

            //testing throwing exception
            //throw new ArgumentException("ha-ha");

            return mainprogram.Rc;
        }

        // All WPF applications should execute on a single-threaded apartment (STA) thread
        [STAThread]
        public static void runWPF()
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