namespace RimworldConflictChecker
{
    class Files
    {
        public readonly string[] IgnoreNames =
            {
            "Changelog.xml",
            "Credits.xml",
            ".gitattributes",
            ".gitignore",
            "LICENSE",
            "README.md"
        };

        public readonly string[] IncludeNames =
        {
            "About.xml"
        };

        public readonly string[] IgnoreExtensions =
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

        public readonly string[] IncludeExtensions =
        {
                ".dll",
                ".xml"
        };

        public readonly string[] IgnoreFolders =
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

        public readonly string[] IncludeFolders =
{
            "About",
            "Assemblies",
            "Defs",
            "Textures"
        };
    }
}
