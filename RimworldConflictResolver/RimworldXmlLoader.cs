using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace RimworldConflictResolver
{
    public class RimworldXmlLoader
    {
        //public List<DirectoryInfo> dirs { get; set; }
        public static string[] Activemods;

        public RimworldXmlLoader(params string[] folders)
        {
            //List<DirectoryInfo> dirs = new List<DirectoryInfo>(new DirectoryInfo(folders[0]).EnumerateDirectories());
            var dirs = new List<DirectoryInfo>();

            Logger.Instance.NewSection("Folders passed to program:");

            if (Osio.FileOrDirectoryExists(folders[0] + "\\RimWorldWin.exe"))
            {
                Logger.Instance.Log("Folder 1 : Game folder Exists: " + folders[0]);
            }
            else
            {
                Logger.Instance.Log("Folder 1 : Game folder: Does NOT Exists: " + folders[0]);
                Logger.Instance.Log("Quitting.");
                return;
            }

            //var modfolders = folders.Skip(1).ToArray();
            var modfolders = new string[folders.Length+1];
            modfolders[0] = folders[0]; //rimworld.exe folder
            modfolders[1] = folders[0] + "\\Mods";
            Array.Copy(folders, 1, modfolders, 2, folders.Length - 1);

            //set folders to
                //0 rimworld.exe folder
                //1 mods folder
                //2+ any other passed folder
            folders = modfolders;
            modfolders = folders.Skip(1).ToArray();
            //modfolders is now just the mod folders
            int i = 2;
            foreach (var folder in modfolders)
            {
                if (Osio.FileOrDirectoryExists(folder))
                {
                    var dirs2 = new List<DirectoryInfo>(new DirectoryInfo(folder).EnumerateDirectories());
                    foreach (var folderx in dirs2)
                    {
                        dirs.Add(folderx);
                    }
                    Logger.Instance.Log("Folder " + i + " : Mod Folder Exists : " + folder);
                }
                else
                {
                    Logger.Instance.Log("Folder " + i + " : Mod Folder Does NOT Exist : " + folder);
                }
                i++;
            }
            //dirs is now a DirectoryInfo list of all folders

            //get game version
            Logger.Instance.NewSection("Getting RimWorld game version");
            Version RimWorldVersion = Version.Parse("0.0.0");
            if (Osio.FileOrDirectoryExists(folders[0] + "\\Version.txt"))
            {
                string line;
                StreamReader file = new StreamReader(folders[0] + "\\Version.txt");
                while ((line = file.ReadLine()) != null)
                {
                    var firstline = line.Split(' ');
                    RimWorldVersion = Version.Parse(firstline[0]);
                }
                file.Close();
            }
            Logger.Instance.Log("RimWorld game version: " + RimWorldVersion);

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

                Activemods = LoadModsConfigXml(); //contains dirname, not modname.
                Logger.Instance.NewSection("Active Mods:");

                //set load position
                //set if enabled
                //for all mods found in directories
                foreach (var moddirs in Mods)
                {
                    //for all mods found in ModsConfig.xml
                    foreach (var listedmod in Activemods)
                    {
                        if (moddirs.DirName == listedmod)
                        {
                            moddirs.ModEnabled = true;
                            var modposition = moddirs.ModRank = Array.IndexOf(Activemods, listedmod) + 1;
                            if (modposition == 0)
                            {
                                modposition = 0; //should never happen
                            }
                            moddirs.ModRank = modposition;
                        }
                    }
                }

                var sortedModsConfig = Mods.OrderBy(x => x.ModRank).ToList();
                foreach (var mod in sortedModsConfig)
                {
                    Logger.Instance.DumpMods(mod.ModEnabled, mod.ModRank, mod.DirName, mod.ModXmlDetails.ModTargetVersion, mod.ModXmlDetails.ModName);
                }
                Logger.Instance.Log($"{dirs.Count} mods found (including RimWorlds Core folder");

                //
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
                        if (mod.ModXmlDetails.ModTargetVersion > RimWorldVersion)
                        {
                            Logger.Instance.Log(string.Format("Mod version {0,-9} of mod " + mod.ModXmlDetails.ModName + " is above RimWorld Game version. It will not load.",
                                mod.ModXmlDetails.ModTargetVersion));
                            continue;
                        }
                        if  (mod.ModXmlDetails.ModTargetVersion.Minor < RimWorldVersion.Minor)
                        {
                            Logger.Instance.Log(string.Format("Mod version {0,-9} of mod " + mod.ModXmlDetails.ModName + " is too far below RimWorld Game version. It will not load.", mod.ModXmlDetails.ModTargetVersion));
                        }
                    }
                }
                //

                Logger.Instance.NewSection("Listing mods and their DLL's & XML's...");
                sortedModsConfig = Mods.OrderBy(x => x.ModXmlDetails.ModName).ToList();
                foreach (var mod in sortedModsConfig)
                {
                    Logger.Instance.DumpModHeader(mod.ModXmlDetails.ModName, mod.FullDirName);
                    foreach (var modXmlFile in mod.XmlFiles)
                    {
                        if ((modXmlFile.XmlFileInfo.Name.Contains(".xml") || (modXmlFile.XmlFileInfo.Name.Contains(".dll"))))
                        {
                            Logger.Instance.DumpModFiles(modXmlFile.XmlFileInfo.Name);
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
                            totalConflicts += mod.CheckForCoreOverwrites(mod, otherMod);
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

                //////

                Logger.Instance.NewSection("Checking for XML Conflicts");
                Logger.Instance.Log(
                    "Larger numbered mods are lower down in your Mod list, and will overwrite lower numbered mods.");
                Logger.Instance.Log("");

                totalConflicts = 0;

                foreach (var mod in Mods)
                {
                    foreach (var otherMod in Mods)
                    {
                        if (!otherMod.Checked)
                        {
                            totalConflicts += mod.CheckForConflicts(mod, otherMod);
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

                totalConflicts = 0;

                //TODO: add core DLLs (done???)
                //creates dup entries for the same mod (if in diff dir)
                //are DLL's loaded if out of Assemblies??
                foreach (var mod in Mods)
                {
                    foreach (var otherMod in Mods)
                    {
                        if (!otherMod.DllChecked)
                        {
                            //Logger.Instance.Log("Checking: " + mod.DirName + "  &  " + otherMod.DirName);
                            totalConflicts += mod.CheckForDllConflicts(mod, otherMod);
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
                Logger.Instance.Log("");

                totalConflicts = 0;

                foreach (var mod in Mods)
                {
                    totalConflicts += mod.CheckForMisplacedDlls(mod);
                }
                Logger.Instance.Log($"{totalConflicts} misplaced DLLs found.");

                Logger.Instance.NewSection("Conflict Checker Finished");
            }
            catch (UnauthorizedAccessException uaEx)
            {
                Logger.Instance.Log(uaEx.Message);
            }
            catch (PathTooLongException pathEx)
            {
                Logger.Instance.Log(pathEx.Message);
            }
            return;
        }

        public List<Mod> Mods { get; set; }
        public Version RimWorldVersion { get; private set; }

        public string[] LoadModsConfigXml()
        {
            var modsconfig = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                             @"Low\Ludeon Studios\RimWorld\Config\ModsConfig.xml";

            Logger.Instance.NewSection("ModsConfig.xml: " + modsconfig);

            if (Osio.FileOrDirectoryExists(modsconfig))
            {
                Logger.Instance.Log("ModsConfig.xml found");
                var modsconfigxml = XDocument.Load(modsconfig);
                Logger.Instance.Log("ModsConfig.xml loaded");
                //Logger.Instance.Log("Active Mods:");
                //string[] activemods = modsconfigxml.Descendants("activeMods").Select(element => element.Value).ToArray();
                //string[] activemods = modsconfigxml.Descendants("li").Select(x => (string)x).ToArray();
                Activemods = modsconfigxml.Descendants("li").Select(x => (string)x).ToArray();
                //activemods.ToList().ForEach(SimpleLogger.Instance.Log);
                //foreach (var item in _activemods)
                //{
                //Logger.Instance.Log("\t" + item);
                //}
            }
            else
            {
                Logger.Instance.Log("ModsConfig.xm NOT Found");
            }
            return Activemods;
        }

        
        }
}