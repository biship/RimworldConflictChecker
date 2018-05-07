using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Diagnostics;
using System.Xml.Linq;

namespace RimworldConflictChecker
{
    public class Mod
    {
        public Mod(DirectoryInfo directory)
        {
            //Mod.add calls this

            //creates XMLFiles as a list of all files in all folders
            ConflictedMods = new List<Mod>();
            ConflictedDlls = new List<Mod>();
            FullDirName = directory.FullName; //dir name
            DirName = directory.Name; //dir name
            //load all files associated with this mod
            XmlFiles = GetXmlFiles(directory);
            ModXmlDetails = GetAboutDetails(XmlFiles); //populated from About.xml
            //Logger.Instance.Log("Dir: " + directory.Name + ModXmlDetails.ModName + directory.FullName);
            //Logger.Instance.DumpModFiles(ModXmlDetails.ModName, directory.FullName);
            //ModName
            //ModTargetVersion
            //ModDescription
            Checked = false;
            CoreChecked = false;
            DllChecked = false;
            ModEnabled = false;
            ModRank = 0;
            CoreOverrights = 0;
            //give count of all files, not just XML.
            //SimpleLogger.Instance.Log("\t" + XmlFiles.Count + " Total Files found. ");
        }

        //the mod object, contains these elements:
        public string FullDirName { get; set; } //actual full dir name
        public string DirName { get; set; } //actual sub dir name
        public ModDetails ModXmlDetails { get; set; } //mod details from about.xml
        public List<XmlFile> XmlFiles { get; set; } //list of all files under dir
        public List<Mod> ConflictedMods { get; set; } //list of other conflicting XML's
        public List<Mod> ConflictedDlls { get; set; } //list of other conflicting DLLs's
        public bool Checked { get; set; } //set once its XML's have been completly compared
        public bool CoreChecked { get; set; } //nameDefs checked vs Core?
        public int CoreOverrights { get; set; }
        public bool DllChecked { get; set; } //set once its DLL's have been completly compared
        public bool ModEnabled { get; set; } //is the mod in ModsConfig.xml
        public int ModRank { get; set; } //mods position in MosdConfig.xml

        public int CheckForConflicts(Mod mod, Mod otherMod)
        {
            if (ReferenceEquals(this, otherMod) || (DirName == otherMod.DirName) || (otherMod.DirName == DirName) || (DirName == "Core") || (otherMod.DirName == "Core"))
            {
                return 0;
            }

            var totalConflicts = 0;

            foreach (var xmlFile in XmlFiles)
            {
                foreach (var otherXmlFile in otherMod.XmlFiles)
                {
                    totalConflicts += CheckForFileConflicts(xmlFile, otherXmlFile, mod, otherMod);
                }
            }

            if (totalConflicts != 0)
            {
                ConflictedMods.Add(otherMod); //this will add all files from conflict
                otherMod.ConflictedMods.Add(this);
            }

            return totalConflicts;
        }

        public int CheckForCoreOverwrites(Mod mod, Mod otherMod)
        {
            if (ReferenceEquals(this, otherMod) || (DirName == otherMod.DirName) || (otherMod.DirName == DirName))
            {
                return 0;
            }

            var totalConflicts = 0;

            foreach (var xmlFile in XmlFiles)
            {
                foreach (var otherXmlFile in otherMod.XmlFiles)
                {
                    totalConflicts += CheckForFileConflicts(xmlFile, otherXmlFile, mod, otherMod);
                }
            }

            otherMod.CoreChecked = true;
            otherMod.CoreOverrights = totalConflicts;
            return totalConflicts;
        }

        public static ModDetails GetAboutDetails(List<XmlFile> xmlFiles)
        {
            var aboutXDoc = new ModDetails("", Version.Parse("0.0.0"), "");
            //var aboutXDoc = new ModDetails();
            //AboutXDoc = new string[3];
            //ModDetails AboutXDoc = new ModDetails();

            foreach (var xmlFile in xmlFiles)
            {
                //if (xmlFile.XmlFileInfo.FullName.ToUpper().Contains("\\ABOUT\\ABOUT.XML"))
                //New Compare Extension for ignoring case
                if (xmlFile.XmlFileInfo.FullName.Contains("\\About\\About.xml", StringComparison.OrdinalIgnoreCase))
                {
                    if (xmlFile.XmlDocument.Root != null)
                    {
                        try
                        {
                            foreach (var element in xmlFile.XmlDocument.Root.Elements())
                            {
                                if (element != null && !element.Value.IsNullOrEmpty())
                                {
                                    if (element.Name.ToString().Contains("NAME", StringComparison.OrdinalIgnoreCase))
                                    {
                                        aboutXDoc.ModName = element.Value;
                                        continue;
                                    }
                                    if (element.Name.ToString().Contains("TARGETVERSION", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (!element.Value.IsNullOrEmpty())
                                        {
                                            aboutXDoc.ModTargetVersion = Version.Parse(element.Value);
                                        }
                                        else
                                        {
                                            aboutXDoc.ModTargetVersion = Version.Parse("0.0.0");
                                        }
                                        continue;
                                    }
                                    if (element.Name.ToString().Contains( "DESCRIPTION", StringComparison.OrdinalIgnoreCase))
                                    {
                                        aboutXDoc.ModDescription = element.Value;
                                    }
                                }
                            }
                        }
                        //catch (ArgumentException ex)
                        catch (Exception ex)
                        {
                            //Logger.Instance.LogError("Parsing About.xml", ex);
                            Utils.LogException("Parsing About.xml", ex);
                        }
                    }
                }
            }
            return aboutXDoc;
        }

        private static int CheckForFileConflicts(XmlFile xmlFile, XmlFile otherXmlFile, Mod mod, Mod otherMod)
        {
            var totalConflicts = 0;


            //RimworldXmlLoader.weapons.Add(weapon);
            //weapons.Add(weapon);
            //if (((mod.ModEnabled == true) && (otherMod.ModEnabled == true)) && ((xmlFile.XmlDocument != null) && (otherXmlFile.XmlDocument != null) && xmlFile.XmlFileInfo.Name.Contains(".xml") && otherXmlFile.XmlFileInfo.Name.Contains(".xml")))
            if (((xmlFile.XmlDocument != null) && (otherXmlFile.XmlDocument != null) && xmlFile.XmlFileInfo.Name.Contains(".xml") && otherXmlFile.XmlFileInfo.Name.Contains(".xml")))
            {
                if (xmlFile.XmlDocument.Root != null)
                {
                    foreach (var element in xmlFile.XmlDocument.Root.Elements())
                    {
                        //string thingDefNameValue = element.Element("ThingDef")?.Value;
                        var thingDefNameValue = element.Name.ToString(); //the element text
                        var defNameValue = element.Element("defName")?.Value; //the value of the defName element (only works when on this element)
                        var thisculture = CultureInfo.CurrentCulture;

                        //element defName is null, then no point in doing this element (likely because we're not on the deName element!)
                        if (string.IsNullOrWhiteSpace(defNameValue))
                        {
                            continue;
                        }

                        //we're in a ThingDef
                        if (Program.Dps)
                        {
                            foreach (var element2 in element.Elements())
                            {
                                var weapon = new Weapon();
                                var thingDefNameValue2 = element2.Name.ToString(); //the element text
                                if (thisculture.CompareInfo.IndexOf(thingDefNameValue2, "statBases", CompareOptions.IgnoreCase) >= 0)
                                    {
                                    foreach (var element3 in element2.Elements())
                                    {
                                        var thingDefNameValue3 = element3.Name.ToString(); //the element text
                                        var defNameValue3 = element3.Value;
                                        //the value of the defName element (only works when on this element)
                                        //if (thisculture.CompareInfo.IndexOf(thingDefNameValue3, "RangedWeapon_Cooldown", CompareOptions.IgnoreCase) >= 0)
                                        //{
                                        //match
                                        //weapon.RangedWeapon_Cooldown = float.Parse(element3.Value, CultureInfo.InvariantCulture.NumberFormat);
                                        //}
                                        weapon.RangedWeaponCooldown = string.IsNullOrWhiteSpace(element3.Element("RangedWeapon_Cooldown")?.Value) ? 0f : float.Parse(element3.Element("RangedWeapon_Cooldown")?.Value, CultureInfo.InvariantCulture.NumberFormat); //crashes if null
                                    }
                                }
                            }
                        }

                        if (otherXmlFile.XmlDocument.Root != null)
                        {
                            foreach (var otherElement in otherXmlFile.XmlDocument.Root.Elements())
                            {
                                var otherDefNameValue = otherElement.Element("defName")?.Value;

                                //if null then no point in doing this element
                                if (string.IsNullOrWhiteSpace(otherDefNameValue))
                                {
                                    continue;
                                }

                                if (string.Equals(defNameValue, otherDefNameValue)) //do the elements match?
                                {
                                    //We have a conflict print some useful information about the conflict
                                    //SimpleLogger.Instance.Log("Conflict found with xml tag " + defNameValue + " in the file " + xmlFile.XmlFileInfo.Name + " in mod " + DirName + " and the file " + otherXmlFile.XmlFileInfo.Name + " in mod " + otherMod.DirName);
                                    var rootValue = xmlFile.XmlDocument.Root.Name.ToString();
                                    var otherRootValue = otherXmlFile.XmlDocument.Root.Name.ToString();
                                    //string otherThingDefNameValue = otherElement.Element("ThingDef")?.Value;
                                    var otherThingDefNameValue = otherElement.Name.ToString();
                                    //var thisculture = CultureInfo.CurrentCulture;
                                    //Console.WriteLine(rootValue);
                                    //Console.WriteLine(otherRootValue);

                                    if (string.Equals(thingDefNameValue, otherThingDefNameValue))
                                    {
                                        //entire defnames are replaced by mods further down in the load order, so no need to check params
                                        long xmlsize = 0;
                                        long otherXmlsize = 0;
                                        try
                                        {
                                            xmlsize = new FileInfo(xmlFile.XmlFileInfo.FullName).Length;
                                            otherXmlsize = new FileInfo(otherXmlFile.XmlFileInfo.FullName).Length;
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Instance.LogError("Unable to read file.", ex);
                                            //Utils.LogException("XmlReader Error", ex);
                                        }
                                        var modenabled = "";
                                        var othermodenabled = "";
                                        var linenum = 1;
                                        var otherlinenum = 1;
                                        string line;
                                        string otherline;
                                        var modposition = mod.ModRank;
                                        var othermodposition = otherMod.ModRank;
                                        //if (modposition == 0)
                                        if (!mod.ModEnabled)
                                        {
                                            modenabled = "(Not Enabled)";
                                        }
                                        //if (othermodposition == 0)
                                        if (!otherMod.ModEnabled)
                                        {
                                            othermodenabled = "(Not Enabled)";
                                        }
                                        using (var file = new StreamReader(xmlFile.XmlFileInfo.FullName))
                                        {
                                            while ((line = file.ReadLine()) != null)
                                            {
                                                if (
                                                    thisculture.CompareInfo.IndexOf(line, "<defName>" + defNameValue + "</defName>", CompareOptions.IgnoreCase) >= 0)
                                                {
                                                    break;
                                                }
                                                linenum++;
                                            }
                                            //file.Close();
                                            using (var otherfile = new StreamReader(otherXmlFile.XmlFileInfo.FullName))
                                            {
                                                while ((otherline = otherfile.ReadLine()) != null)
                                                {
                                                    if (thisculture.CompareInfo.IndexOf(otherline, "<defName>" + defNameValue + "</defName>", CompareOptions.IgnoreCase) >= 0)
                                                    {
                                                        break;
                                                    }
                                                    otherlinenum++;
                                                }
                                                //otherfile.Close();
                                                //foreach (var match in File.ReadLines(@xmlFile.XmlFileInfo.FullName)
                                                //    .Select((text, index) => new { text, lineNumber = index + 1 })
                                                //    .Where(x => x.text.Contains(defNameValue)))
                                                //{
                                                //    Console.WriteLine("{0}: {1}", match.lineNumber, match.text);
                                                //}
                                                //var match = File.ReadLines(@xmlFile.XmlFileInfo.FullName).Select((text, index) => new { text, lineNumber = index + 1 }).Where(x => x.text.Contains(defNameValue));
                                                //SimpleLogger.Instance.DumpConflict(modposition, xmlsize, 0, rootValue, thingDefNameValue, defNameValue, xmlFile.XmlFileInfo.FullName);
                                                //SimpleLogger.Instance.DumpConflict(othermodposition, otherXmlsize, 0, otherRootValue, otherThingDefNameValue, defNameValue, otherXmlFile.XmlFileInfo.FullName);
                                                if (modposition < othermodposition)
                                                {
                                                    Logger.Instance.Log("Conflicting Mods: " + mod.ModXmlDetails.ModName + " " + modenabled + " &  " + otherMod.ModXmlDetails.ModName + " " + othermodenabled);
                                                    Logger.Instance.Log(xmlFile.XmlFileInfo.FullName);
                                                    Logger.Instance.Log(otherXmlFile.XmlFileInfo.FullName);
                                                    Logger.Instance.DumpConflict(mod.ModXmlDetails.ModName, modposition, xmlsize, linenum, rootValue, thingDefNameValue, defNameValue);
                                                    Logger.Instance.DumpConflict(otherMod.ModXmlDetails.ModName, othermodposition, otherXmlsize, otherlinenum, otherRootValue, otherThingDefNameValue, defNameValue);
                                                    Logger.Instance.Log("");
                                                }
                                                else
                                                {
                                                    Logger.Instance.Log("Conflicting Mods: " + otherMod.ModXmlDetails.ModName + " " + othermodenabled + "  &  " + mod.ModXmlDetails.ModName + " " + modenabled);
                                                    Logger.Instance.Log(otherXmlFile.XmlFileInfo.FullName);
                                                    Logger.Instance.Log(xmlFile.XmlFileInfo.FullName);
                                                    Logger.Instance.DumpConflict(otherMod.ModXmlDetails.ModName, othermodposition, otherXmlsize, otherlinenum, otherRootValue, otherThingDefNameValue, defNameValue);
                                                    Logger.Instance.DumpConflict(mod.ModXmlDetails.ModName, modposition, xmlsize, linenum, rootValue, thingDefNameValue, defNameValue);
                                                    Logger.Instance.Log("");
                                                }
                                                totalConflicts++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return totalConflicts;
        }

        public List<XmlFile> GetXmlFiles(DirectoryInfo directory)
        {
            var files = new List<XmlFile>();
            FileInfo currentFile = null;
            var xmlReaderSettings = new XmlReaderSettings { CheckCharacters = false };
            var filesarray = new Files();
            try
            {
                //foreach (FileInfo xmlFile in directory.GetFiles().Where(x => string.Equals(x.Extension, ".xml")))
                foreach (var xmlFile in directory.GetFiles())
                {
                    currentFile = xmlFile;
                    //if (!xmlFile.Name.Contains("About") && !xmlFile.Name.Contains("Changelog") && !xmlFile.Name.Contains("Credits") && !xmlFile.FullName.Contains("Languages"))
                    //using lisst: http://stackoverflow.com/questions/4874371/how-to-check-if-any-word-in-my-liststring-contains-in-text
                    //if (!xmlFile.Name.Contains("Changelog") && !xmlFile.Name.Contains("Credits") && !xmlFile.FullName.Contains("\\Languages\\"))
                    //if ((ignorefiles.Names.Any(w => xmlFile.Name.Contains(w))) || (ignorefiles.Extensions.Any(w => xmlFile.Extension.Equals(w))) || (ignorefiles.Folders.Any(w => xmlFile.FullName.Contains(w))))
                    //{
                    //
                    //}
                    //if ((xmlFile.Extension.Equals(".XML", StringComparison.OrdinalIgnoreCase) || xmlFile.Extension.Equals(".DLL", StringComparison.OrdinalIgnoreCase)) && !xmlFile.FullName.Contains("\\Languages\\"))
                    //if (filesarray.IncludeExtensions.Any(w => xmlFile.Extension.Equals(w, StringComparison.OrdinalIgnoreCase)) && !filesarray.IgnoreFolders.Any(xmlFile.FullName.Contains)) //this means the dll check will miss loads of folders.
                    if ((filesarray.IncludeExtensions.Any(w => xmlFile.Extension.Equals(w, StringComparison.OrdinalIgnoreCase)) &&  //only approved extensions
                        (!filesarray.IgnoreNames.Any(w => xmlFile.Name.Equals(w, StringComparison.OrdinalIgnoreCase)))) &&          //not filenames to ignore
                        !xmlFile.FullName.Contains("\\Languages\\"))                                                                //not files in Languages folder
                    {
                        //if (xmlFile.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        if (xmlFile.Extension.Equals(".XML", StringComparison.OrdinalIgnoreCase))
                        {
                            //add the file as an XML file for later browsing
                            //files.Add(new XmlFile(xmlFile, XDocument.Load(xmlFile.FullName))); //this will crash when a xml contains non-ascii chars
                            try
                            {
                                using (XmlReader xmlReader = XmlReader.Create(xmlFile.FullName, xmlReaderSettings))
                                {
                                    xmlReader.MoveToContent();
                                    files.Add(new XmlFile(xmlFile, XDocument.Load(xmlReader)));
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Instance.Log("XmlReader Error: " + ex.Message + "\t" + xmlFile.FullName);
                                //Utils.LogException("XmlReader Error", ex);
                            }

                        }
                        else
                        {
                            //add all other files, but don't read their contents - setting xml to null
                            files.Add(new XmlFile(xmlFile, null));
                        }
                        //only log xml & dll files
                        //if ((xmlFile.Extension == ".xml") || (xmlFile.Extension == ".dll"))
                        //{
                        //Logger.Instance.Log("\t" + xmlFile.Name);
                        //}
                    }
                }
                foreach (var subDirectory in directory.GetDirectories())
                {
                    //keep looping through all subfolders, skipping language folders.
                    if (!subDirectory.FullName.Contains("\\Languages\\"))
                    {
                        files.AddRange(GetXmlFiles(subDirectory));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(currentFile == null ? "" : currentFile.Name + " " + ex.Message);
            }

            return files;
        }

        public int CheckForDllConflicts(Mod mod, Mod otherMod)
        {
            if (ReferenceEquals(this, otherMod)) // || (DirName == otherMod.DirName) || (otherMod.DirName == DirName))
            {
                return 0;
            }

            var totalConflicts = 0;

            foreach (var xmlFile in mod.XmlFiles) //changed from XmlFiles to mod.XmlFiles
            {
                foreach (var otherXmlFile in otherMod.XmlFiles)
                {
                    totalConflicts += CheckForDupDllFiles(mod, otherMod, xmlFile, otherXmlFile);
                }
            }

            if (totalConflicts != 0)
            {
                ConflictedDlls.Add(otherMod);
                otherMod.ConflictedDlls.Add(this);
            }

            return totalConflicts;
        }

        private static int CheckForDupDllFiles(Mod mod, Mod otherMod, XmlFile xmlFile, XmlFile otherXmlFile)
        {
            var totalConflicts = 0;
            if ((xmlFile.XmlFileInfo != null) && (otherXmlFile.XmlFileInfo != null))
            {
                if (xmlFile.XmlFileInfo.FullName.ContainsAll("\\Assemblies\\", ".dll")) //rare. 1/mod at most. Only care if its a DLL in the assemblies folder.
                {
                    //Logger.Instance.Log(xmlFile.XmlFileInfo.FullName + " vs " + otherXmlFile.XmlFileInfo.FullName);
                    if (otherXmlFile.XmlFileInfo.FullName.ContainsAll("\\Assemblies\\", ".dll")) //rare. 1/mod at most. Only care if its a DLL in the assemblies folder.
                    {
                        if (xmlFile.XmlFileInfo.Name == otherXmlFile.XmlFileInfo.Name) //semi rare. 1+ per mod
                        {
                            //if (xmlFile.XmlFileInfo.FullName != otherXmlFile.XmlFileInfo.FullName) //happens when a mod compares against itself.
                            //{
                            var xmlsize = new FileInfo(xmlFile.XmlFileInfo.FullName).Length;
                            var xmldate = new FileInfo(xmlFile.XmlFileInfo.FullName).LastWriteTime;
                            var otherXmlsize = new FileInfo(otherXmlFile.XmlFileInfo.FullName).Length;
                            var otherXmldate = new FileInfo(otherXmlFile.XmlFileInfo.FullName).LastWriteTime;
                            // Get the file version for the notepad.
                            //FileVersionInfo xmlversion = FileVersionInfo.GetVersionInfo(xmlFile.XmlFileInfo.FullName);
                            var xmlversion = FileVersionInfo.GetVersionInfo(xmlFile.XmlFileInfo.FullName);
                            //FileVersionInfo otherXmlversion = FileVersionInfo.GetVersionInfo(otherXmlFile.XmlFileInfo.FullName);
                            var otherXmlversion = FileVersionInfo.GetVersionInfo(otherXmlFile.XmlFileInfo.FullName);
                            if (mod.ModRank > otherMod.ModRank)
                            {
                                Logger.Instance.LogDll(otherXmlFile.XmlFileInfo.FullName, otherMod.ModRank, otherXmlsize, xmlversion.FileVersion, otherXmldate);
                                Logger.Instance.LogDll(xmlFile.XmlFileInfo.FullName, mod.ModRank, xmlsize, otherXmlversion.FileVersion, xmldate);
                            }
                            else
                            {
                                Logger.Instance.LogDll(xmlFile.XmlFileInfo.FullName, mod.ModRank, xmlsize, xmlversion.FileVersion, xmldate);
                                Logger.Instance.LogDll(otherXmlFile.XmlFileInfo.FullName, otherMod.ModRank, otherXmlsize, otherXmlversion.FileVersion, otherXmldate);
                            }
                            totalConflicts++;
                            //}
                        }
                    }
                }
            }
            return totalConflicts;
        }

        public static int CheckForMisplacedDlls(Mod mod)
        {
            var totalConflicts = 0;
            foreach (var xmlFile in mod.XmlFiles)
            {
                if (xmlFile.XmlFileInfo.Directory != null && ((xmlFile.XmlFileInfo.Name.Contains(".dll")) && (!xmlFile.XmlFileInfo.Directory.Name.Equals("Assemblies"))))
                {
                    Logger.Instance.Log("DLL not in an Assemblies folder: " + xmlFile.XmlFileInfo.FullName);
                    totalConflicts++;
                }
            }
            return totalConflicts;
        }
    }
}