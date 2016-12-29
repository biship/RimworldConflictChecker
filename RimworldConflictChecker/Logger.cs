using System;
using System.IO;

namespace RimworldConflictChecker
    {
    public class Logger : IDisposable
    {

        private static Logger _instance;

        private static readonly string Outputfilename = "RCC.txt";

        //private readonly StringBuilder _buffer;
        private StreamWriter _buffer;

        private Logger()
        {
            //_buffer = new StringBuilder();
            try
            {
                //_buffer = new StreamWriter(File.Open(Outputfilename, FileMode.Create, FileAccess.Write, FileShare.None));
                _buffer = new StreamWriter(File.Open(Outputfilename, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to create log file " + Outputfilename + " - " + ex.Message);
            }
        }

        public static Logger Instance
        {
            //called on every logger.instance
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                _buffer.Close();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Log(string message)
        {
            var now = $"{DateTime.Now:HH:mm:ss} ";
#if DEBUG
            //when do i ever need this?
            //Console.WriteLine(now + message);
#endif
            //_buffer.AppendLine(message);
            try
            {
                // do the actual work
                _buffer.WriteLine(message);
            }
            catch (Exception)
            {
                // very simplified exception logic... Might want to expand this
                if (_buffer != null)
                {
                    _buffer.Dispose();
                }
            }
        }

        public void NewSection(string message)
        {
            if (_buffer != null)
            {   //force a write
                _buffer.Flush();
            }
            var now = $"{DateTime.Now:MM-dd-yyyy HH:mm:ss} ";
            Log("===============================================================================================================================" + "\n" + now + "\n" + message + "\n");
        }

        public void DumpModHeader(string modname, string folder)
        {
            Log($"Mod name: {modname,-50} Mod Folder: {folder,-120}");
        }

        public void DumpModFiles(string filename)
        {
            Log($"\t{filename,-50}");
        }

        public void DumpConflict(string modname, int loadPosition, long size, int line, string tag1, string tag2,
            string tag3)
        {
            Log($"{modname,-46} Load Position: {loadPosition,-3} FileSize: {size,-6} Line: {line,-5} RootElement: {tag1,-23} Element: {tag2,-39} defName: {tag3,-36}");
        }

        public void LogDLL(string modname, int loadPosition, long size, string fileversion, DateTime date)
        {
            Log($"{modname,-95} Load Position: {loadPosition,-3} FileSize: {size,-8} Version: {fileversion, -10} Date: {date,-10}");
        }

        public void DumpMods(bool enabled, int loadPosition, string dirname, Version version, string modname)
        {
            var isenabled = "Not Enabled";
            if (enabled)
            {
                isenabled = "Enabled";
            }
            Log($"{isenabled,-12} Load Position: {loadPosition,-4} Version: {version,-10} Directory: {dirname,-48}  Name: {modname,-48}");
        }

        public void LogError(string what, Exception e)
        {
            Log("***************** ERROR START ***************");
            Log("Occurred during: " + e.Source);
            Log("Comments: " + what);
            Log("Error: " + e.GetType().Name);
            Log("Error Message: " + e.Message);
            Log("Stacktrace: " + e.StackTrace);
            Log("***************** ERROR FIN *****************");
        }

        public void WriteToFile()
        {
            //File.WriteAllText(Outputfilename, _buffer.ToString());
            if (_buffer != null)
            {
                _buffer.Dispose();
                _buffer = null;
            }
        }
    }
}