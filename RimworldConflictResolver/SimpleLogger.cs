using System.IO;
using System.Text;

namespace RimworldConflictResolver
{
    public class SimpleLogger
    {
        public static readonly string Outputfilename = "RimworldConflictResolver.txt";

        private static SimpleLogger instance;

        private StringBuilder buffer;

        public static SimpleLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SimpleLogger();
                }
                return instance;
            }
        }

        private SimpleLogger()
        {
            buffer = new StringBuilder();
        }

        public void Log(string message)
        {
            buffer.Append(message + "\n");
        }

        public void WriteToFile()
        {
            File.WriteAllText(Outputfilename, buffer.ToString());
        }
    }
}