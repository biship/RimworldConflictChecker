using System.IO;

namespace RimworldConflictResolver
{
    internal class Osio
    {
        internal static bool FileOrDirectoryExists(string name)
        {
            return Directory.Exists(name) || File.Exists(name);
        }
    }
}