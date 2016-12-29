using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimworldConflictChecker
{
    class Files
    {
        public string[] IgnoreNames =
            {
            "Changelog.xml",
            "Credits.xml",
            ".gitattributes",
            ".gitignore",
            "LICENSE",
            "README.md"
        };

        public string[] IncludeNames =
        {
            "About.xml"
        };

        public string[] IgnoreExtensions =
        {
                ".png",
                ".jpg",
                ".gif",
                ".jpeg",
                ".bmp",
                ".ico",
                ".txt",
                ".db",
                ".wav"
        };

        public string[] IncludeExtensions =
        {
                ".dll",
                ".xml"
        };

        public string[] IgnoreFolders = 
        {
            "Languages",
            "Source", 
            ".git", 
            ".vs",
            "_mod",
            "obj",
            "bin",
            "libraries"
        };

        public string[] IncludeFolders =
{
            "About",
            "Assemblies",
            "Defs",
            "Textures"
        };
    }
}
