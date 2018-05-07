using System;
using System.Collections.Generic;

namespace RimworldConflictChecker
{
    public class ModView
    {
        public int Index { get; set; }
        public int Loadposition { get; set; }
        public bool Enabled { get; set; }
        public Version Version { get; set; }
        public string Modname { get; set; }
        public int Nummodconflicts { get; set; }
        public List<string> Modconflicts { get; set; }
        public int Numxmlconflicts { get; set; }
        public int Numdllconflicts { get; set; }
        public int Numcoreconflicts { get; set; }
        public int Numxmlfiles { get; set; }
        public string Moddir { get; set; }
        public string Fullmoddir { get; set; }
        public ModView()
        {
            Modconflicts = new List<string>();
        }
    }
}