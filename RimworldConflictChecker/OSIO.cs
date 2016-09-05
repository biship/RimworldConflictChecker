using System.IO;

namespace RimworldConflictChecker
{
    internal static class Osio
    {
        internal static bool FileOrDirectoryExists(string name)
        {
            return Directory.Exists(name) || File.Exists(name);
        }
    }
}