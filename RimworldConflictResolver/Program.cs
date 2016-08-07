using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimworldConflictResolver
{
    public class Program
    {
        static int Main(string[] args)
        {
            // Test if input arguments were supplied:
            if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter mods folder path. Make sure you backup your mods folder.");
                System.Console.WriteLine("Usage: RimworldConflictResolver <modFolderPath>");
                return 1;
            }

            new RimworldXmlLoader(args[0]);
            
            return 0;
        }
    }
}
