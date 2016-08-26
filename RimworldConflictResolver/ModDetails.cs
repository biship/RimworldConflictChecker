using System;

namespace RimworldConflictResolver
{
    public class ModDetails
    {
        public ModDetails(string modName, Version modTargetVersion, string modDescription)
        {
            ModName = modName;
            ModTargetVersion = modTargetVersion;
            ModDescription = modDescription;
        }

        public string ModName { get; set; }
        public Version ModTargetVersion { get; set; }
        public string ModDescription { get; set; }
    }
}