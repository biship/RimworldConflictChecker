using System;
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
            string rimworldfolder = "";
            string modfolder1 = "";
            string modfolder2 = "";

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
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new OptionsForm());
                    //set folder locations based on settings
                    rimworldfolder = (string)Settings.Default["RimWorldFolder"];
                    modfolder1 = (string)Settings.Default["ModFolder1"];
                    modfolder2 = (string)Settings.Default["ModFolder2"];
                    formrc = OptionsForm.ReturnValue1;
                    break;
            }
            Console.WriteLine("Rimworld Conflict Checker Running checks...");

            // Uncomment the following after testing to see that NBug is working as configured
            // NBug.Settings.ReleaseMode = true;

            //NBug.Settings.ReleaseMode = true; 
            // Add the event handler for handling non-UI thread exceptions to the event. 
            //AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
            //Application.Current.DispatcherUnhandledException += NBug.Handler.DispatcherUnhandledException;

            //System.Windows.Application.Current.DispatcherUnhandledException += NBug.Handler.DispatcherUnhandledException;

            // Add the event handler for handling UI thread exceptions to the event.
            //Application.ThreadException += NBug.Handler.ThreadException;

            // Set the unhandled exception mode to force all Windows Forms errors
            // to go through our handler.
            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //Application.SetUnhandledExceptionMode(NBug.Handler.UnhandledException);

            //throw new ArgumentException("ha-ha");

            //if (args.Length == 0)
            //{
            //Console.WriteLine("Please enter any number of mod folders.");
            //Console.WriteLine(
            //"Usage: RCC <modFolderPath1> <modFolderPath2> <modFolderPath3> ...");
            //return 1;
            //}

            //new RimworldXmlLoader("D:\\SteamLibrary\\steamapps\\common\\RimWorld","D:\\SteamLibrary\\steamapps\\workshop\\content\\294100");
            //new RimworldXmlLoader(rimworldfolder, modfolder1, modfolder2);
            new RimworldXmlLoader(rimworldfolder, modfolder1, modfolder2);
            Logger.Instance.WriteToFile();
            return 0;
        }

        
    }
}