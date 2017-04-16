using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Octokit;

namespace RimworldConflictChecker
{
    public class RimworldXmlLoader
    {
        public static string[] modsconfig;
        public static bool incdisabled;
        public static List<Mod> Mods { get; set; }
        public static List<Weapon> Weapons { get; set; }
        //public static bool incdisabled = false;
        public int Rc { get; set; }
        //public List<DirectoryInfo> dirs { get; set; }

        //static GitHubClient client = new GitHubClient(new ProductHeaderValue("RimworldConflictChecker"));

        public static Version Latestver { get; set; }

        public RimworldXmlLoader(bool incDisabled, params string[] folders)
        //public RimworldXmlLoader(string rimworldfolder, string modsconfigfolder, string[] modfolders)
        //public RimworldXmlLoader(string rimworldfolder, string modsconfigfolder, string modfolder1, string modfolder2)
        {
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
            //TODO: check XML inheritance: http://ludeon.com/forums/index.php?topic=19499.0
            //TODO: set core to rimWorldVersion
            //TODO: report issues when using UI

            Rc = 1; //assume this is gonna crash

            string[] checksimplemented =
            {
                "\tThe same nameDef defined in 2 or more mods",
                "\tThe same DLL in 2 or more mods",
                "\tCore nameDefs overwriten by a mod",
                "\tDLL's not in the Assemblies folder",
                "\tChecks versions of mod against RimWorld version",
                "\tIdentifies possibly corrupt mods (partially implemented)"
            };
            string[] futurechecks =
            {
                "\tCheck XML inheritance: http://ludeon.com/forums/index.php?topic=19499.0",
                "\tCheck if CCL is compat with RimWorld.",
                "\tCheck if root, tag, defname are valid (by parsing core)",
                "\tCheck if a mod is valid - needs about.xml, at least one XML or dll.",
                "\tCheck if mod exists twice. a) based on folder name & b) based on About.xml name.",
                "\tCheck if a mod's defname conflicts with another defname of the same mod",
                "\tChange output to tabbed tables in a Windows Form",
                "\tand more..."
            };
            //List<DirectoryInfo> dirs = new List<DirectoryInfo>(new DirectoryInfo(folders[0]).EnumerateDirectories());
            var dirs = new List<DirectoryInfo>();
            var baddirs = new List<DirectoryInfo>();
            var strCompTime = Properties.Resources.BuildDate;
            //var weapons = new List<Weapon>();
            RimworldXmlLoader.incdisabled = incDisabled;

            //welcome
            Logger.Instance.NewSection("Rimworld Conflict Checker Started!");

            Logger.Instance.Log("Code: https://github.com/biship/RimworldConflictChecker");
            Logger.Instance.Log("Details: https://ludeon.com/forums/index.php?topic=25305");
            Logger.Instance.Log("Please report any bugs either on GitHub, or the Ludeon forum thread.");
            Logger.Instance.Log("");
#if DEBUG
            Logger.Instance.Log("Build: DEBUG " + strCompTime);
#else
            Logger.Instance.Log("Build: RELEASE " + strCompTime);
#endif
            Logger.Instance.Log("Results of the checks are written to file RCC.txt in this folder.");
            Logger.Instance.Log("");
            Logger.Instance.Log("Currently implemented checks:");
            //checksimplemented.ToList().ForEach(j => Logger.Instance.Log(j));
            checksimplemented.ToList().ForEach(Logger.Instance.Log);
            Logger.Instance.Log("Checks to be possibly added in the future:");
            //futurechecks.ToList().ForEach(j => Logger.Instance.Log(j));
            futurechecks.ToList().ForEach(Logger.Instance.Log);

            Logger.Instance.NewSection("Checking GitHub for updates. This will timeout after 10s");
            Console.WriteLine("Checking GitHub for updates. This will timeout after 10s");
            //Latestver = Version.Parse("0.0.0.6"); //why set this...
            Latestver = Version.Parse("0.0.0.0");
#if !DEBUG
            Task.Run(async () => { await GetRepoAsync(); }).Wait(10000);
#endif
            var thisversion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            Logger.Instance.Log("You are running version : " + thisversion);
            Logger.Instance.Log("Latest version on GitHub: " + Latestver);

            if (Latestver > thisversion)
            {
                Logger.Instance.Log("There is a newer version on GitHub. You can get it here: https://github.com/biship/RimworldConflictChecker/releases");
                Console.WriteLine("There is a newer version on GitHub. You can get it here: https://github.com/biship/RimworldConflictChecker/releases");
            }

            if (Program.Formrc != 0)
            {
                //esc, X or quit pressed on form. so quit
                Logger.Instance.Log("");
                Logger.Instance.Log("Esc, X or close pressed on form, Quitting");
                Console.WriteLine("Esc, X or close pressed on form, Quitting");
                Rc = 1;
                return;
            }

            //parse folders
            Logger.Instance.NewSection("Parameters and folders:");

            //testing throwing exception
            //throw new ArgumentException("ha-ha");

            Logger.Instance.Log("Command Line Parameters:");
            Program.Allargs.Each((item, n) =>
            {
                if (!String.IsNullOrEmpty(item))
                {
                    Logger.Instance.Log($"{n+1} : {item}");
                }
            });

            Logger.Instance.Log("");
            Logger.Instance.Log("Consider every installed mods as active: " + incdisabled);
            Logger.Instance.Log("(switchable on config form, or with -all command line switch)");

            Logger.Instance.Log("");
            Logger.Instance.Log("Folders:");

            folders.Each((item, n) =>
            {
                if (!String.IsNullOrEmpty(item))
                {
                    if (!Utils.IsFullPath(item))
                    {
                        //relative path. Try to fix.
                        var tempdir = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), item); //relative to fullpath
                        if (Utils.IsFullPath(tempdir))
                        {
                            Logger.Instance.Log($"Changing relative folder {item} to full path {tempdir}");
                            folders[n] = tempdir;
                        }
                    }
                }
            });

            Logger.Instance.Log("");

            //Folder[0] RimWorld exe folder (and loc of Version.txt)
            if (Utils.FileOrDirectoryExists(folders[0] + "\\RimWorldWin.exe"))
            {
                Logger.Instance.Log("Folder 1 : Found RimWorldWin.exe in : " + folders[0]);
            }
            else
            {
                Logger.Instance.Log("Folder 1 : Not able to find RimWorldWin.exe in : " + folders[0]);
                Console.WriteLine("Not able to find RimWorldWin.exe in : " + folders[0]);
                Logger.Instance.Log("Quitting.");
                Console.WriteLine("Quitting.");
                Rc = 1;
                return;
            }

            //Folder[1] RimWorld local app dir (loc of ModsConfig.xml)
            if (string.IsNullOrEmpty(folders[1]))
                {
                //not passed a folder to check for ModsConfig.xml
                Logger.Instance.Log("Folder 2 : Not passed a ModsConfig.xml folder from command line");
                Logger.Instance.Log("Folder 2 : Will search for ModsConfig.xml later before checks.");
            }
            else
            {
                if (Utils.FileOrDirectoryExists(folders[1] + "\\ModsConfig.xml"))
                {
                    Logger.Instance.Log("Folder 2 : Found ModsConfig.xml in : " + folders[1]);
                }
                else
                {
                    Logger.Instance.Log("Folder 2 : Not able to find ModsConfig.xml in : " + folders[1]);
                    Logger.Instance.Log("Folder 2 : Will search for ModsConfig.xml later before checks.");
                    Console.WriteLine("Not able to find ModsConfig.xml in : " + folders[1]);
                    Console.WriteLine("Will search for ModsConfig.xml later before checks.");
                    //Logger.Instance.Log("Quitting.");
                    //Console.WriteLine("Quitting.");
                    //Rc = 1;
                    //return;
                }
            }

            //Folder[2] Mod folder 1 (Core)
            if (Utils.FileOrDirectoryExists(folders[2] + "\\Core"))
            {
                Logger.Instance.Log("Folder 3 : Found Mod folder : " + folders[2]);
            }
            else
            {
                Logger.Instance.Log("Folder 3 : Not able to find subfolder Core in : " + folders[2]);
                Console.WriteLine("Not able to find subfolder Core in : " + folders[2]);
                Logger.Instance.Log("Quitting.");
                Console.WriteLine("Quitting.");
                Rc = 1;
                return;
            }

            //Folder[3] Mod folder 2 (user mods)
            if (!string.IsNullOrEmpty(folders[3]))
            {
                if (Utils.FileOrDirectoryExists(folders[3]))
                {
                    Logger.Instance.Log("Folder 4 : Found Mod folder : " + folders[3]);
                }
                else
                {
                    Logger.Instance.Log("Folder 4 : Not able to find mod folder : " + folders[3]);
                    Console.WriteLine("Not able to find mod folder: " + folders[3]);
                    Logger.Instance.Log("Quitting.");
                    Console.WriteLine("Quitting.");
                    Rc = 1;
                    return;
                }
            }

            //get game version
            Logger.Instance.NewSection("Getting RimWorld game version");
            //var rimWorldVersion = Version.Parse("0.0.0");
            var RimWorld = new RimWorld(folders[0])
            {
                Version = Version.Parse("0.0.0")
            };
            Logger.Instance.Log("Reading " + folders[0] + "\\Version.txt");
            if (Utils.FileOrDirectoryExists(folders[0] + "\\Version.txt"))
            {
                string line;
                var file = new StreamReader(folders[0] + "\\Version.txt");
                while ((line = file.ReadLine()) != null)
                {
                    Logger.Instance.Log("Read Line: " + line);
                    var firstline = line.Split(' ');
                    RimWorld.Version = Version.Parse(firstline[0]);
                    break;
                }
                file.Close();
            }
            else
            {
                Logger.Instance.Log("Not able to find " + folders[0] + "\\Version.txt");
                Logger.Instance.Log("Not able to determine if mods will or will not load. Setting RimWorld game version to 0.0.0");
                //RimWorld.Version = Version.Parse("0.0.0");
            }
            Logger.Instance.Log("RimWorld game version: " + RimWorld.Version);

            Console.WriteLine("Running checks...");

            Logger.Instance.NewSection("Adding subfolders of each Mod folder to the list of folders to search");

            //check if we got here without any folders?
            foreach (var folder in folders.Skip(2))
            {
                if (folder.Length != 0)
                {
                    //create dirs2 to hold all the subfolders of each mod folder
                    var dirs2 = new List<DirectoryInfo>(new DirectoryInfo(folder).EnumerateDirectories());
                    if (!dirs2.IsNullOrEmpty())
                    {
                        Logger.Instance.Log("Searching " + dirs2.Count + " sub-folders of : " + folder);
                        foreach (var folderx in dirs2)
                        {
                            //add every subfolder
                            if (folderx.GetDirectories("About").Length > 0)
                            {
                                dirs.Add(folderx);
                            }
                            else
                            {
                                baddirs.Add(folderx); //TODO: check and display
                                Logger.Instance.Log("Folder does not contain an 'About' subfolder, not a valid Mod folder, NOT added to search list : " + folderx.FullName);
                            }
                        }
                        Logger.Instance.Log("Folders searched.");
                    }
                    else
                    {
                        Logger.Instance.Log("Folder empty, not searching : " + folder);
                    }
                }
                else
                {
                    Logger.Instance.Log("Folder empty, not searching : " + folder);
                }
            }
            //dirs is now a DirectoryInfo list of all folders
            if (dirs.IsNullOrEmpty())
            {
                Logger.Instance.Log(@"Unable to find any mods! Please check your paths, or report this on GitHub/Ludeon (see RCC.txt)");
                Console.WriteLine(@"Unable to find any mods! Please check your paths, or report this on GitHub/Ludeon (see RCC.txt)");
                Logger.Instance.Log("Quitting.");
                Console.WriteLine("Quitting.");
                Rc = 1;
                return;
            }

            //get game-ver was here

            //this works
            //List<DirectoryInfo> dirs = new List<DirectoryInfo>(new DirectoryInfo(folder).EnumerateDirectories());
            //string folder2 = "D:\\SteamLibrary\\steamapps\\common\\RimWorld\\Mods";
            //List<DirectoryInfo> dirs2 = new List<DirectoryInfo>(new DirectoryInfo(folder2).EnumerateDirectories());
            //// dirs needs to be a list of dirs
            //foreach (DirectoryInfo folderx in dirs2)
            //    {
            //    dirs.Add(folderx);
            //    }

            Mods = new List<Mod>();

            //Load all the mods into the mod list
            try
            {
                //List <DirectoryInfo> dirs = OSIO.CreateDirList(folders);

                //foreach (var info in dirs.Where(x => !string.Equals(x.Name, "Core")))
                Logger.Instance.NewSection("Creating list of all MOD files & reading all XML's");
                Logger.Instance.Log("Any XML errors here may prevent the XML from loading correctly.");
                foreach (var info in dirs)
                {
                    Mods.Add(new Mod(info));
                }

                if (Mods.IsNullOrEmpty())
                {
                    Logger.Instance.Log(@"Unable to find any mods! Please check your paths, or report this on GitHub/Ludeon (see RCC.txt)");
                    Console.WriteLine(@"Unable to find any mods! Please check your paths, or report this on GitHub/Ludeon (see RCC.txt)");
                    Logger.Instance.Log("Quitting.");
                    Console.WriteLine("Quitting.");
                    Rc = 1;
                    return;
                }

                modsconfig = LoadModsConfigXml(folders[1]); //contains just dirname
                Logger.Instance.NewSection("Mods Found:");

                //set load position
                //set if enabled
                //for all mods found in directories
                foreach (var moddirs in Mods)
                {
                    //if (!Utils.IsNullOrEmpty(modsconfig))
                    if (!modsconfig.IsNullOrEmpty())
                        {
                        //for all mods found in ModsConfig.xml
                        foreach (var listedmod in modsconfig)
                        {
                            if (moddirs.DirName == listedmod)
                            {
                                moddirs.ModEnabled = true;
                                var modposition = moddirs.ModRank = Array.IndexOf(modsconfig, listedmod) + 1;
                                moddirs.ModRank = modposition;
                            }
                        }
                    }
                }

                var sortedModsConfig = Mods.OrderBy(x => x.ModRank).ToList();
                foreach (var mod in sortedModsConfig)
                {
                    Logger.Instance.DumpMods(mod.ModEnabled, mod.ModRank, mod.DirName, mod.ModXmlDetails.ModTargetVersion, mod.ModXmlDetails.ModName);
                }
                Logger.Instance.Log($"{dirs.Count} mods found (including RimWorlds Core folder)");

                Logger.Instance.NewSection("Checking Mod versions against RimWorld Game Version.");
                foreach (var mod in sortedModsConfig)
                {
                    if ((mod.ModXmlDetails.ModTargetVersion != null) && (mod.DirName != "Core"))
                    {

                        if (mod.ModXmlDetails.ModTargetVersion == null)
                        {
                            Logger.Instance.Log("Version missing for mod " + mod.ModXmlDetails.ModName + " is above RimWorld Game version. It will not load.");
                            continue;
                        }
                        if (mod.ModXmlDetails.ModTargetVersion > RimWorld.Version)
                        {
                            Logger.Instance.Log(string.Format("Mod version {0,-9} of mod " + mod.ModXmlDetails.ModName + " is above RimWorld Game version. It will not load.",
                                mod.ModXmlDetails.ModTargetVersion));
                            continue;
                        }
                        if (mod.ModXmlDetails.ModTargetVersion.Minor < RimWorld.Version.Minor)
                        {
                            Logger.Instance.Log(string.Format("Mod version {0,-9} of mod " + mod.ModXmlDetails.ModName + " is too far below RimWorld Game version. It will not load.", mod.ModXmlDetails.ModTargetVersion));
                        }
                    }
                }
                //TODO: set core ver

                Logger.Instance.NewSection("Listing mods and their DLL's & XML's...");
                sortedModsConfig = Mods.OrderBy(x => x.ModXmlDetails.ModName).ToList();
                foreach (var mod in sortedModsConfig)
                {
                    if (mod.ModEnabled || incdisabled)
                    {
                        Logger.Instance.DumpModHeader(mod.ModXmlDetails.ModName, mod.FullDirName);
                        foreach (var modXmlFile in mod.XmlFiles)
                        {
                            //if ((modXmlFile.XmlFileInfo.Name.Contains(".xml") || (modXmlFile.XmlFileInfo.Name.Contains(".dll"))))
                            //{
                            Logger.Instance.DumpModFiles(modXmlFile.XmlFileInfo.Name);
                            //}
                        }
                    }
                }

                Logger.Instance.NewSection("Checking for every mod's XML that overwrites Core (game default) XML");
                Logger.Instance.Log("These are not conflicts, or typically a problem. It just means the following mods change (overwrite) the standard default game provided object definitions.");
                Logger.Instance.Log("");

                var totalConflicts = 0;

                foreach (var mod in Mods)
                {
                    if (mod.DirName == "Core")
                    {
                        foreach (var otherMod in Mods)
                        {
                            if (otherMod.ModEnabled || incdisabled)
                            {
                                totalConflicts += mod.CheckForCoreOverwrites(mod, otherMod);
                            }
                        }
                    }

                }

                Logger.Instance.Log($"{totalConflicts} Core XMLs overwrites found.");

                if (totalConflicts > 0)
                {
                    Logger.Instance.NewSection("Mods overwriting Core XML nameDefs:");

                    //var sortedMods = Mods.OrderByDescending(x => x.ConflictedMods.Count).ToList();
                    var sortedMods = Mods.OrderByDescending(x => x.CoreOverrights).ToList();

                    foreach (var mod in sortedMods)
                    {
                        if (mod.ModEnabled || incdisabled)
                        {
                            if (mod.CoreOverrights == 1)
                            {
                                Logger.Instance.Log(mod.ModXmlDetails.ModName + " overwrote " + mod.CoreOverrights +
                                                    " core nameDef");
                            }
                            else if (mod.CoreOverrights >= 1)
                            {
                                Logger.Instance.Log(mod.ModXmlDetails.ModName + " overwrote " + mod.CoreOverrights +
                                                    " core nameDefs");
                            }
                        }
                    }
                }

                //////

                Logger.Instance.NewSection("Checking for XML Conflicts");
                Logger.Instance.Log("Mods listed here will conflict with each other as they change the same object (defName).");
                Logger.Instance.Log("Larger numbered mods are lower down in your Mod list, and will take precidence over lower numbered mods.");
                Logger.Instance.Log("For example, Load Position 10 will overwrite & take precidence over Load Position 5");
                Logger.Instance.Log("");

                totalConflicts = 0;

                foreach (var mod in Mods)
                {
                    if (mod.ModEnabled || incdisabled)
                    {
                        foreach (var otherMod in Mods)
                        {
                            if (otherMod.ModEnabled || incdisabled)
                            {
                                if (!otherMod.Checked)
                                {
                                    totalConflicts += mod.CheckForConflicts(mod, otherMod);
                                }
                            }
                        }
                    }
                    mod.Checked = true;
                }

                Logger.Instance.Log($"{totalConflicts} XML Conflicts found.");
                if (totalConflicts > 0)
                {
                    Logger.Instance.NewSection("Mods with XML Conflicts:");

                    var sortedMods = Mods.OrderByDescending(x => x.ConflictedMods.Count).ToList();

                    foreach (var mod in sortedMods)
                    {
                        if (mod.ConflictedMods.Count == 1)
                        {
                            Logger.Instance.Log(mod.ModXmlDetails.ModName + " conflicted with " + mod.ConflictedMods.Count +
                                                " other mod");
                        }
                        else if (mod.ConflictedMods.Count >= 1)
                        {
                            Logger.Instance.Log(mod.ModXmlDetails.ModName + " conflicted with " + mod.ConflictedMods.Count +
                                                " other mods");
                        }
                    }
                }

                Logger.Instance.NewSection("Checking for duplicate DLL's...");
                Logger.Instance.Log("DLL's listed here exist in more than one mod.");
                Logger.Instance.Log("The largest Load Position DLL will load last and take presidence over lower numbered Load Position");
                Logger.Instance.Log("Many mods now use common library DLL's such as 0Harmony.dll & $HugsLibChecker.dll");
                Logger.Instance.Log("It's ok if these common libraries are in multiple Mod folders");
                Logger.Instance.Log("It's not good if they are different versions.");

                totalConflicts = 0;

                //creates dup entries for the same mod (if in diff dir)
                //are DLL's loaded if out of Assemblies - no
                foreach (var mod in Mods)
                {
                    if (mod.ModEnabled || incdisabled)
                    {
                        foreach (var otherMod in Mods)
                        {
                            if (otherMod.ModEnabled || incdisabled)
                            {
                                if (!otherMod.DllChecked)
                                {
                                    //Logger.Instance.Log("Checking: " + mod.DirName + "  &  " + otherMod.DirName);
                                    totalConflicts += mod.CheckForDllConflicts(mod, otherMod);
                                }
                            }
                        }
                    }
                    mod.DllChecked = true;
                }
                Logger.Instance.Log($"{totalConflicts} sets of duplicate DLLs found.");
                if (totalConflicts > 0)
                {
                    Logger.Instance.NewSection("Mods with the same DLL:");

                    var sortedDllMods = Mods.OrderByDescending(x => x.ConflictedDlls.Count).ToList();

                    foreach (var mod in sortedDllMods)
                    {
                        if (mod.ConflictedDlls.Count == 1)
                        {
                            Logger.Instance.Log(mod.ModXmlDetails.ModName + " has the same DLL as " + mod.ConflictedDlls.Count +
                                                " other mod");
                        }
                        else if (mod.ConflictedDlls.Count >= 1)
                        {
                            Logger.Instance.Log(mod.ModXmlDetails.ModName + " has the same DLL as " + mod.ConflictedDlls.Count +
                                                " other mods");
                        }
                    }
                }

                Logger.Instance.NewSection("Checking for misplaced DLL's.");
                Logger.Instance.Log("This doesn't cause a problem, as they are not loaded anyway, as they are not in the Assemblies subfolder of the mod.");
                Logger.Instance.Log("However this IS a problem if this is the Mod's DLL that the mod needs (then it should be in the Assemblies folder)");

                totalConflicts = 0;

                foreach (var mod in Mods)
                {
                    if (mod.ModEnabled || incdisabled)
                    {
                        totalConflicts += Mod.CheckForMisplacedDlls(mod);
                    }
                }
                Logger.Instance.Log($"{totalConflicts} misplaced DLLs found.");

                //Logger.Instance.NewSection("Rimworld Conflict Checker creating forms with results.");
                //Console.WriteLine("Rimworld Conflict Checker creating forms with results.");

                //testing throwing exception
                //throw new ArgumentException("ha-ha");

                Logger.Instance.NewSection("Rimworld Conflict Checker Finished");
                var duration = DateTime.Now - Program.starttime;
                Logger.Instance.Log("Total run time : " + duration.Seconds + " seconds");
                //Logger.Instance.Log("Results of the checks are written to file RCC.txt in this folder.");
                Console.WriteLine("Results of the checks are written to file RCC.txt in this folder.");
                Console.WriteLine("Rimworld Conflict Checker Complete");
                Rc = 0; //we got this far!
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Instance.Log(ex.Message);
            }
            catch (PathTooLongException ex)
            {
                Logger.Instance.Log(ex.Message);
            }
        }


        public static string[] LoadModsConfigXml(string modsconfigfolder)
        {
            //if (rimworldversion.Minor >= 16)
            string modsconfig = null;
            if (!modsconfigfolder.IsNullOrEmpty())
            {
                //path is not empty, append xml
                modsconfig = modsconfigfolder + "\\ModsConfig.xml";
            }
            if (!Utils.FileOrDirectoryExists(modsconfig))
            {
                //set to new loc
                modsconfig = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\Ludeon Studios\RimWorld by Ludeon Studios\Config\ModsConfig.xml";
                if (!Utils.FileOrDirectoryExists(modsconfig))
                {
                    //new is missing, set to old loc
                    modsconfig = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\Ludeon Studios\RimWorld\Config\ModsConfig.xml";
                    if (!Utils.FileOrDirectoryExists(modsconfig))
                    {
                        //can't find it - give up
                        modsconfig = "";
                    }
                }
            }

            Logger.Instance.NewSection("ModsConfig.xml: " + modsconfig);

            if (Utils.FileOrDirectoryExists(modsconfig))
            {
                Logger.Instance.Log("ModsConfig.xml found");
                var modsconfigxml = XDocument.Load(modsconfig);
                Logger.Instance.Log("ModsConfig.xml loaded");
                //Logger.Instance.Log("Active Mods:");
                //string[] activemods = modsconfigxml.Descendants("activeMods").Select(element => element.Value).ToArray();
                //string[] activemods = modsconfigxml.Descendants("li").Select(x => (string)x).ToArray();
                RimworldXmlLoader.modsconfig = modsconfigxml.Descendants("li").Select(x => (string)x).ToArray();
                //activemods.ToList().ForEach(SimpleLogger.Instance.Log);
                //foreach (var item in _activemods)
                //{
                //Logger.Instance.Log("\t" + item);
                //}
            }
            else
            {
                Logger.Instance.Log("ModsConfig.xml NOT Found");
                Logger.Instance.Log("Assuming all mods are active.");
                RimworldXmlLoader.incdisabled = true;
            }
            return RimworldXmlLoader.modsconfig;
        }


        static async Task GetRepoAsync()
        {
            try
            {
                //string clientId = "4cd857fbb4403f81c83b";
                //string clientSecret = "1114a5310ca5e4932ea949d3adab8b3317b49b6a";
                var client = new GitHubClient(new ProductHeaderValue(nameof(RimworldConflictChecker)));

                // NOTE: this is not required, but highly recommended!
                // ask the ASP.NET Membership provider to generate a random value
                // and store it in the current user's session
                //string csrf = new System.Web.Security.Membership.GeneratePassword(24, 3);
                //var csrf = System.Web.Security.Membership.GeneratePassword(24, 3);

                //Session["CSRF:State"] = csrf;

                //var request = new OauthLoginRequest(clientId)
                //{
                //Scopes = { "user", "notifications" },
                //State = csrf
                //};

                // NOTE: user must be navigated to this URL
                //var oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);

                //var tokenAuth = new Credentials("token"); // NOTE: not real token
                //client.Credentials = tokenAuth;

                //var user = await client.User.Get("biship");
                //var repo = await client.Repository.Get("biship", "RimworldConflictChecker");
                //var repo2 = await client.Repository.Release.GetLatest("biship", "RimworldConflictChecker"); //fails...

                var repo = await client.Repository.Release.GetAll("biship", nameof(RimworldConflictChecker));
                //if (repo.Count > 0)
                if (repo.Any())
                {
                    Logger.Instance.Log("Successfully got version data from GitHub");
                }
                else
                {
                    Logger.Instance.Log("Not able to get version data from GitHub");
                }
                //var highestver = new Latestver();
                //int index = -1; //the release that has the highest version
                foreach (var release in repo)
                {
                    //Console.WriteLine("Found {0} : {1}", release.TagName, release.Name);
                    var thisNum = Version.Parse(release.TagName); //ver is in tagname of release
                    if (thisNum != null)
                    {
                        if (thisNum > Latestver)
                        {
                            Latestver = thisNum;
                            //index++;
                        }
                    }
                }
                //Console.WriteLine("{0} in {1}", highestrelease, index);
            }

            catch (Exception ex)
            {
                Logger.Instance.LogError("Failed to get repo info from GitHub", ex);
            }
        }
    }
}