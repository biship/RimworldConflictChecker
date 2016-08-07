using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RimworldConflictResolver
{
    public class Mod
    {
        public string ModName { get; set; }
        public List<XmlFile> XMLFiles { get; set; }
        public List<Mod> ConflictedMods { get; set; } 
        public bool Checked { get; set; }

        public Mod(DirectoryInfo directory)
        {
            ConflictedMods = new List<Mod>();
            ModName = directory.Name;

            //load all the xml files assicated with this mod
            Console.WriteLine(directory.Name);
            SimpleLogger.Instance.Log(directory.Name);
            XMLFiles = GetAllFiles(directory);
            Console.WriteLine("\t" + XMLFiles.Count + " XML Files found");
            SimpleLogger.Instance.Log("\t" + XMLFiles.Count + " XML Files found");
        }

        public int CheckForConflicts(Mod otherMod)
        {
            if (ReferenceEquals(this, otherMod) || ModName.Contains(otherMod.ModName) || otherMod.ModName.Contains(ModName))
            {
                return 0;
            }

            int totalConflicts = 0;
            foreach (XmlFile xmlFile in XMLFiles)
            {
                foreach (XmlFile otherXmlFile in otherMod.XMLFiles)
                {
                    totalConflicts += CheckForFileConflicts(xmlFile, otherXmlFile, otherMod);
                }
            }

            if (totalConflicts != 0)
            {
                ConflictedMods.Add(otherMod);
                otherMod.ConflictedMods.Add(this);
            }

            return totalConflicts;
        }

        private int CheckForFileConflicts(XmlFile xmlFile, XmlFile otherXmlFile, Mod otherMod)
        {
            int totalConflicts = 0;

            foreach (var element in xmlFile.XmlDocument.Root.Elements())
            {
                string defNameValue = element.Element("defName")?.Value;

                //if null then no point in doing this element
                if (string.IsNullOrWhiteSpace(defNameValue))
                {
                    continue;
                }

                foreach (var otherElement in otherXmlFile.XmlDocument.Root.Elements())
                {
                    string otherDefNameValue = otherElement.Element("defName")?.Value;

                    //if null then no point in doing this element
                    if (string.IsNullOrWhiteSpace(otherDefNameValue))
                    {
                        continue;
                    }

                    if (string.Equals(defNameValue, otherDefNameValue))
                    {
                        //We have a conflict print some useful information about the conflict
                        SimpleLogger.Instance.Log("Conflict found with xml tag " + defNameValue + " in the file " + xmlFile.XmlFileInfo.Name + " in mod " + ModName + 
                            " and the file " + otherXmlFile.XmlFileInfo.Name + " in mod " + otherMod.ModName);
                        totalConflicts++;
                    }
                }
            }

            return totalConflicts;
        }

        private List<XmlFile> GetAllFiles(DirectoryInfo directory)
        {
            List<XmlFile> files = new List<XmlFile>();
            FileInfo currentFile = null;
            try
            {
                foreach (FileInfo xmlFile in directory.GetFiles().Where(x => string.Equals(x.Extension, ".xml")))
                {
                    currentFile = xmlFile;
                    if (!xmlFile.Name.Contains("About") && !xmlFile.Name.Contains("Changelog") && !xmlFile.Name.Contains("Credits"))
                    {
                        files.Add(new XmlFile(xmlFile, XDocument.Load(xmlFile.FullName)));
                        SimpleLogger.Instance.Log ("\t" + xmlFile.Name);
                    }
                }
                foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                {
                    files.AddRange(GetAllFiles(subDirectory));
                }
            } catch (System.Exception excpt)
            {
                Console.WriteLine(currentFile == null ? "" : currentFile.Name + " " + excpt.Message);
            }

            return files;
        }
    }
}