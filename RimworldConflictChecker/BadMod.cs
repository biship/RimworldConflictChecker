using System.Collections.Generic;
using System.IO;

namespace RimworldConflictChecker
{
    class BadMod
    {
        public BadMod(DirectoryInfo directory)
        {
            //BadMod.add calls this

            //creates XMLFiles as a list of all files in all folders
            FullDirName = directory.FullName; //dir name
            DirName = directory.Name; //dir name
            //load all files associated with this mod
            //XmlFiles = Mod.GetXmlFiles(directory); //fix
            //ModXmlDetails = Mod.GetAboutDetails(XmlFiles); //populated from About.xml //fix
            //Logger.Instance.Log("Dir: " + directory.Name + ModXmlDetails.ModName + directory.FullName);
            //Logger.Instance.DumpModFiles(ModXmlDetails.ModName, directory.FullName);
            //ModName
            //ModTargetVersion
            //ModDescription
            //ModEnabled = false;
            //ModRank = 0;
            //CoreOverrights = 0;
            //give count of all files, not just XML.
            //SimpleLogger.Instance.Log("\t" + XmlFiles.Count + " Total Files found. ");
        }

        public ModDetails ModXmlDetails { get; set; }

        public List<XmlFile> XmlFiles { get; set; }

        public string DirName { get; set; }

        public string FullDirName { get; set; }
    }
}
