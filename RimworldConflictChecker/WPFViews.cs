using System;
using System.Collections.Generic;

namespace RimworldConflictChecker
{
    public class ModView
    {
        public int index { get; set; }
        public int loadposition { get; set; }
        public bool enabled { get; set; }
        public Version version { get; set; }
        public string modname { get; set; }
        public int nummodconflicts { get; set; }
        public int numxmlconflicts { get; set; }
        public int numdllconflicts { get; set; }
        public int numcoreconflicts { get; set; }
        public int numxmlfiles { get; set; }
        public string moddir { get; set; }
        public string fullmoddir { get; set; }
        
    }
}