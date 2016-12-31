using System;
using System.Collections.Generic;
using System.IO;

namespace RimworldConflictChecker
{
    class RimWorld
    {
        Version _version;

        public List<FileInfo> Files { get; set; } //list of all files under dir

        public Version Version 
        {
            get
            {
                return _version;
            }
            set
            {
                if (value == null) return;
                if (value < Version.Parse("0.0.0.0")) return;
                if (value > Version.Parse("999.999.999.999")) return;

                _version = value;
            }
        }

        public RimWorld(string directory)
        {
            var di = new DirectoryInfo(directory);
            Files = GetFiles(di);
        }

        public List<FileInfo> GetFiles(DirectoryInfo directory)
        {
            var files = new List<FileInfo>();
            try
            {
                foreach (var file in directory.GetFiles())
                {
                    if ((!file.FullName.Contains("\\Mods\\") && !file.FullName.Contains("\\Source\\")))
                    {
                        files.Add(file);
                    }
                }
                foreach (var subDirectory in directory.GetDirectories())
                {
                    if ((!subDirectory.FullName.Contains("\\Mods\\") && !subDirectory.FullName.Contains("\\Source\\")))
                    {
                        files.AddRange(GetFiles(subDirectory));
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Instance.Log(file == null ? "" : file.Name + " " + e.Message);
                Logger.Instance.Log(e.Message);
            }

            return files;
        }
    }
}
