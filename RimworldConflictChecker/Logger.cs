using System;
using System.IO;
using System.Text;

namespace RimworldConflictChecker
    {
    public class Logger
    {
        public static readonly string Outputfilename = "RCC.txt";

        private static Logger _instance;

        private readonly StringBuilder _buffer;

        private Logger()
        {
            _buffer = new StringBuilder();
        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        public void Log(string message)
        {
            var now = string.Format("{0:HH:mm:ss} ", DateTime.Now);
            //buffer.Append(now + message + "\n");
            //buffer.AppendFormat(now + message + "{0}", Environment.NewLine);
#if DEBUG
            Console.WriteLine(now + message);
#endif
            //buffer.AppendLine(now + message);
            _buffer.AppendLine(message);
        }

        public void NewSection(string message)
        {
            var now = string.Format("{0:MM-dd-yyyy HH:mm:ss} ", DateTime.Now);
            Log("===============================================================================================================================" + "\n" + now + "\n" + message + "\n");
        }

        public void DumpModHeader(string modname, string folder)
        {
            Log(string.Format("Mod name: {0,-50} Mod Folder: {1,-120}", modname, folder));
        }

        public void DumpModFiles(string filename)
        {
            Log(string.Format("\t{0,-50}", filename));
        }

        public void DumpConflict(string modname, int loadPosition, long size, int line, string tag1, string tag2,
            string tag3)
        {
            Log(
                string.Format("{0,-50} Load Position: {1,-3} FileSize: {2,-6} Line: {3,-5} RootElement: {4,-23} Element: {5,-45} defName: {6,-42}", modname, loadPosition, size, line, tag1, tag2, tag3));
        }

        public void DumpMods(bool enabled, int loadPosition, string dirname, Version version, string modname)
        {
            var isenabled = "Not Enabled";
            if (enabled)
            {
                isenabled = "Enabled";
            }
            Log(string.Format("{0,-12} Load Position: {1,-4} Version: {2,-10} Directory: {3,-50}  Name: {4,-50}", isenabled, loadPosition, version, dirname, modname));
            //Logger.Instance.Log("Enabled: " + moddirs.ModEnabled + "\tLoad Position: " + moddirs.ModRank + "\tMod Directory: " + moddirs.DirName + "\tMod Target Version: " + moddirs.ModXmlDetails.ModTargetVersion + "\tMod Name:" + moddirs.ModXmlDetails.ModName);
        }

        public void LogError(string what, string error, string errormsg, string stacktrace)
        {
            Log("***************** ERROR START ***************");
            Log("Occurred during: " + what);
            Log("Error: " + error);
            Log("Error Message: " + errormsg);
            Log("Stacktrace: " + stacktrace);
            Log("***************** ERROR FIN *****************");
        }

        public void WriteToFile()
        {
            File.WriteAllText(Outputfilename, _buffer.ToString());
        }
    }
}