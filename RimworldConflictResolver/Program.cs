using System;
using System.Linq;

namespace RimworldConflictResolver
{
    public class Program
    {
        // Satisfies rule: MarkWindowsFormsEntryPointsWithStaThread.
        [STAThread]
        private static int Main(string[] args)
        {

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

            //TODO: date modified on conflict output
            //TODO: check root, tag, defname & param are valid (from core) (XML error <invMealCount>3</invMealCount> doesn`t correspond to any field in type PawnKinddef)
            //TODO: find rimworld dll's (\RimWorld\RimWorldWin_Data) outside of there
            //TODO: pass tag check level to CheckForFileConflicts - root, parent, namedef
            //TODO: check if a mod is valid - needs about.xml, at least one XML or dll.
            //TODO: check if mod exists twice. a) based on folder name & b) based on About.xml name.
            //TODO: check if a mod's defname conflicts with another defname inside itself
            //TODO: group by conflict, not mod.
            //TODO: find source folders
            //TODO: find empty folders (empty even if just desktop.ini)
            //TODO: check if  <minCCLVersion>0.14.1</minCCLVersion> is higher then CCL: About.xml: <description>v0.14.3
            //TODO: check if CCL is compat with RimWorld. CCL: About.xml     <description>v0.14.3 <CRLF> Compatible with RimWorld builds: <CRLF> 1220, 1230, 1232, 1234, 1238, 1241, 1249
            //TODO: check XML inheritance: https://ludeon.com/forums/index.php?topic=19499.0

            string[] checksimplemented =
            {
                "\tThe same nameDef defined in 2 or more mods",
                "\tThe same DLL in 2 or more mods",
                "\tCore nameDefs overwriten by a mod",
                "\tDLL's not in the Assemblies folder",
                "\tChecks versions of mod against RimWorld version"
            };
            string[] futurechecks = {"\tTBD"};

            Logger.Instance.NewSection("Conflict Checker Started!");
            Logger.Instance.Log("Currently implemented checks:");
            checksimplemented.ToList().ForEach(i => Logger.Instance.Log(i));
            Logger.Instance.Log("Checks to be possibly added in the future:");
            futurechecks.ToList().ForEach(i => Logger.Instance.Log(i));

            var rimworldfolder = (string) Settings.Default["RimWorldFolder"];
            var modfolder1 = (string) Settings.Default["Mods1"];
            var modfolder2 = (string) Settings.Default["Mods2"];
            var modfolder3 = (string) Settings.Default["Mods3"];

#if DEBUG
            //Logger.Instance.WriteToFile();
            //return 0;
            new RimworldXmlLoader("D:\\SteamLibrary\\steamapps\\common\\RimWorld",
                "D:\\SteamLibrary\\steamapps\\workshop\\content\\294100");
            Logger.Instance.WriteToFile();
            return 0;
#endif
            //new RimworldXmlLoader("D:\\SteamLibrary\\steamapps\\common\\RimWorld\\Mods", "D:\\SteamLibrary\\steamapps\\workshop\\content\\294100");
            //return 0;

            if (args.Length == 0)
            {
                Console.WriteLine("Please enter any number of mod folders.");
                Console.WriteLine(
                    "Usage: RimworldConflictResolver <modFolderPath1> <modFolderPath2> <modFolderPath3> ...");
                return 1;
            }

            new RimworldXmlLoader(args[0]);

            return 0;
        }
    }
}