using System.IO;
using System.Xml.Linq;

namespace RimworldConflictChecker
{
    public class XmlFile
    {
        public XmlFile(FileInfo xmlFileInfo, XDocument xmlDocument)
        {
            XmlFileInfo = xmlFileInfo;
            XmlDocument = xmlDocument;
        }

        public FileInfo XmlFileInfo { get; private set; }
        public XDocument XmlDocument { get; private set; }
    }
}