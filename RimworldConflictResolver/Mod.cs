using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace RimworldConflictResolver
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
            ModEnabled = false;
            ModRank = 0;
            CoreOverrights = 0;
            //give count of all files, not just XML.
            //SimpleLogger.Instance.Log("\t" + XmlFiles.Count + " Total Files found. ");
        }

        //the mod object, contains these elements:
        public string FullDirName { get; set; } //actuall full dir name
        public string DirName { get; set; } //actuall sub dir name
        public ModDetails ModXmlDetails { get; set; } //mod name from about.xml
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
                ConflictedMods.Add(otherMod);
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
                    //CheckForFileConflicts(xmlFile, otherXmlFile, otherMod);
                }
            }

            otherMod.CoreChecked = true;
            otherMod.CoreOverrights = totalConflicts;
            //if (totalConflicts != 0)
            //{
            //ConflictedMods.Add(otherMod);
            //otherMod.ConflictedMods.Add(this);
            //}
            return totalConflicts;
        }

        public ModDetails GetAboutDetails(List<XmlFile> xmlFiles)
        {
            var aboutXDoc = new ModDetails("", Version.Parse("0.0.0"), "");
            //var aboutXDoc = new ModDetails();
            //AboutXDoc = new string[3];
            //ModDetails AboutXDoc = new ModDetails();

            foreach (var xmlFile in xmlFiles)
            {
                if (xmlFile.XmlFileInfo.FullName.Contains("\\About\\About.xml"))
                {
                    if (xmlFile.XmlDocument.Root != null)
                    {
                        try
                        {
                            foreach (var element in xmlFile.XmlDocument.Root.Elements())
                            {
                                if (element.Name.ToString().ToUpper() == "NAME")
                                {
                                    aboutXDoc.ModName = element.Value;
                                    continue;
                                }
                                //crashes if this is null
                                if ((element.Name.ToString().ToUpper() == "TARGETVERSION") && (element.Value != null))
                                {
                                    aboutXDoc.ModTargetVersion = Version.Parse(element.Value);
                                    //aboutXDoc.ModTargetVersion = element.Value;
                                    continue;
                                }
                                if (element.Name.ToString().ToUpper() == "DESCRIPTION")
                                {
                                    aboutXDoc.ModDescription = element.Value;
                                }
                            }
                        }
                        catch (ArgumentException e)
                        {
                            Logger.Instance.LogError("Parsing About.xml", e.GetType().Name, e.Message, e.StackTrace);
                        }
                    }
                }
            }
            return aboutXDoc;
        }

        private int CheckForFileConflicts(XmlFile xmlFile, XmlFile otherXmlFile, Mod mod, Mod otherMod)
        {
            var totalConflicts = 0;

            if ((xmlFile.XmlDocument != null) && (otherXmlFile.XmlDocument != null) && xmlFile.XmlFileInfo.Name.Contains(".xml") && otherXmlFile.XmlFileInfo.Name.Contains(".xml"))
            {
                if (xmlFile.XmlDocument.Root != null)
                {
                    foreach (var element in xmlFile.XmlDocument.Root.Elements())
                    {
                        //string thingDefNameValue = element.Element("ThingDef")?.Value;
                        var thingDefNameValue = element.Name.ToString();
                        var defNameValue = element.Element("defName")?.Value;

                        //SimpleLogger.Instance.Log(xmlFile.XmlFileInfo.FullName + " : " + element.Name.ToString());
                        //if null then no point in doing this element
                        if (string.IsNullOrWhiteSpace(defNameValue))
                        {
                            continue;
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
                                    //Console.WriteLine(rootValue);
                                    //Console.WriteLine(otherRootValue);

                                    if (string.Equals(thingDefNameValue, otherThingDefNameValue))
                                    {
                                        //entire defnames are replaced by mods further down in the load order, so no need to check params
                                        var modenabled = "";
                                        var othermodenabled = "";
                                        var linenum = 1;
                                        var otherlinenum = 1;
                                        string line;
                                        string otherline;
                                        var xmlsize = new FileInfo(xmlFile.XmlFileInfo.FullName).Length;
                                        var otherXmlsize = new FileInfo(otherXmlFile.XmlFileInfo.FullName).Length;
                                        //var modposition = Array.IndexOf(RimworldXmlLoader.Activemods, DirName) + 1;
                                        var modposition = mod.ModRank;
                                        //var othermodposition = Array.IndexOf(RimworldXmlLoader.Activemods, otherMod.DirName) + 1;
                                        var othermodposition = otherMod.ModRank;
                                        if (modposition == 0)
                                        {
                                            modenabled = "(Not Enabled)";
                                        }
                                        if (othermodposition == 0)
                                        {
                                            othermodenabled = "(Not Enabled)";
                                        }
                                        var file = new StreamReader(xmlFile.XmlFileInfo.FullName);
                                        var thisculture = CultureInfo.CurrentCulture;
                                        while ((line = file.ReadLine()) != null)
                                        {
                                            if (
                                                thisculture.CompareInfo.IndexOf(line,
                                                    "<defName>" + defNameValue + "</defName>",
                                                    CompareOptions.IgnoreCase) >= 0)
                                            {
                                                break;
                                            }
                                            linenum++;
                                        }
                                        file.Close();
                                        var otherfile =
                                            new StreamReader(otherXmlFile.XmlFileInfo.FullName);
                                        while ((otherline = otherfile.ReadLine()) != null)
                                        {
                                            if (
                                                thisculture.CompareInfo.IndexOf(otherline,
                                                    "<defName>" + defNameValue + "</defName>", CompareOptions.IgnoreCase) >=
                                                0)
                                            {
                                                break;
                                            }
                                            otherlinenum++;
                                        }
                                        otherfile.Close();
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
                                            //TODO:fix dirname
                                            //Logger.Instance.Log("Conflicting Mods: " + DirName + " " + modenabled + " &  " + otherMod.DirName + " " + othermodenabled);
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

            return totalConflicts;
        }

        private List<XmlFile> GetXmlFiles(DirectoryInfo directory)
        {
            var files = new List<XmlFile>();
            FileInfo currentFile = null;
            try
            {
                //foreach (FileInfo xmlFile in directory.GetFiles().Where(x => string.Equals(x.Extension, ".xml")))
                foreach (var xmlFile in directory.GetFiles())
                {
                    currentFile = xmlFile;
                    //if (!xmlFile.Name.Contains("About") && !xmlFile.Name.Contains("Changelog") && !xmlFile.Name.Contains("Credits") && !xmlFile.FullName.Contains("Languages"))
                    if (!xmlFile.Name.Contains("Changelog") && !xmlFile.Name.Contains("Credits") && !xmlFile.FullName.Contains("\\Languages\\"))
                    {
                        //TODO: fix. this will crash when a xml contains non-ascii chars
                        //if (xmlFile.Extension == ".xml")
                        if (xmlFile.FullName.Contains(".xml")) //faster?
                        {
                            //add the file as an XML file for later browsing
                            files.Add(new XmlFile(xmlFile, XDocument.Load(xmlFile.FullName)));
                        }
                        else
                        {
                            //add all other files, setting xml to null
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
                    //keep looping through all subfolders
                    files.AddRange(GetXmlFiles(subDirectory));
                }
            }
            catch (Exception excpt)
            {
                Logger.Instance.Log(currentFile == null ? "" : currentFile.Name + " " + excpt.Message);
            }

            return files;
        }

        public int CheckForDllConflicts(Mod mod, Mod otherMod)
        {
            // if (ReferenceEquals(this, otherMod) || (DirName == otherMod.DirName) || (otherMod.DirName == DirName))
            //{
            //return 0;
            //}

            var totalConflicts = 0;
            //for the same mod, it checks all files against all files.
            foreach (var xmlFile in XmlFiles)
            {
                foreach (var otherXmlFile in otherMod.XmlFiles)
                {
                    totalConflicts += CheckForDupDllFiles(xmlFile, otherXmlFile);
                }
            }

            if (totalConflicts != 0)
            {
                ConflictedDlls.Add(otherMod);
                otherMod.ConflictedDlls.Add(this);
            }

            return totalConflicts;
        }

        private int CheckForDupDllFiles(XmlFile xmlFile, XmlFile otherXmlFile)
        {
            var totalConflicts = 0;

            //if ((xmlFile.XmlFileInfo.Name == otherXmlFile.XmlFileInfo.Name) && (xmlFile.XmlFileInfo.Extension == ".dll")) //slow
            if (((xmlFile.XmlFileInfo.Name == otherXmlFile.XmlFileInfo.Name) && xmlFile.XmlFileInfo.Name.Contains(".dll")) && (xmlFile.XmlFileInfo.FullName != otherXmlFile.XmlFileInfo.FullName) 
                && ((xmlFile.XmlFileInfo.FullName.Contains("\\Assemblies\\")) && (otherXmlFile.XmlFileInfo.FullName.Contains("\\Assemblies\\"))))
            {
                Logger.Instance.Log(xmlFile.XmlFileInfo.FullName);
                Logger.Instance.Log(otherXmlFile.XmlFileInfo.FullName);
                totalConflicts++;
            }
            return totalConflicts;
        }

        public int CheckForMisplacedDlls(Mod mod)
        {
            var totalConflicts = 0;
            foreach (var xmlFile in XmlFiles)
            {
                if ((xmlFile.XmlFileInfo.Name.Contains(".dll")) && (!xmlFile.XmlFileInfo.Directory.Name.Equals("Assemblies")))
                {
                    Logger.Instance.Log("DLL not in an Assemblies folder: " + xmlFile.XmlFileInfo.FullName);
                    totalConflicts++;
                }
            }
            return totalConflicts;
        }
    }
}