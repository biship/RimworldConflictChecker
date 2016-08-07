using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RimworldConflictResolver
{
    public class RimworldXmlLoader
    {
        public List<Mod> Mods { get; set; }

        public RimworldXmlLoader(string path)
        {
            Mods = new List<Mod>();
            
            //Load all the mods into the mod list
            try
            {
                List<DirectoryInfo> dirs = new List<DirectoryInfo>(new DirectoryInfo(path).EnumerateDirectories());
                
                Console.WriteLine("{0} mods found.", dirs.Count-1);
                SimpleLogger.Instance.Log($"{dirs.Count-1} mods found.");

                foreach (DirectoryInfo info in dirs.Where(x => !string.Equals(x.Name, "Core")))
                {
                    Mods.Add(new Mod(info));
                    SimpleLogger.Instance.Log(info.Name);
                }

                Console.WriteLine("Checking for Conflicts");
                SimpleLogger.Instance.Log("Checking for Conflicts");

                int totalConflicts = 0;

                foreach (Mod mod in Mods)
                {
                    foreach (Mod otherMod in Mods)
                    {
                        if (!otherMod.Checked)
                        {
                            totalConflicts += mod.CheckForConflicts(otherMod);
                        }
                    }
                    mod.Checked = true;
                }

                Console.WriteLine("{0} Conflicts found.", totalConflicts);
                SimpleLogger.Instance.Log($"{totalConflicts} Conflicts found.");

                List<Mod> sortedMods = Mods.OrderByDescending(x => x.ConflictedMods.Count).ToList();

                foreach (Mod mod in sortedMods)
                {
                    if (mod.ConflictedMods.Count != 0)
                    {
                        SimpleLogger.Instance.Log(mod.ModName + " conflicted with " + mod.ConflictedMods.Count + " other mods");
                    }
                }

                SimpleLogger.Instance.WriteToFile();
            } catch (UnauthorizedAccessException UAEx)
            {
                Console.WriteLine(UAEx.Message);
            } catch (PathTooLongException PathEx)
            {
                Console.WriteLine(PathEx.Message);
            }
        }
    }
}