using System.IO;
using System.Xml.Linq;

namespace RimworldConflictResolver
{
    public class XmlFile
    {

        public FileInfo XmlFileInfo { get; private set; }
        public XDocument XmlDocument { get; private set; }

        public XmlFile(FileInfo xmlFileInfo, XDocument xmlDocument)
        {
            XmlFileInfo = xmlFileInfo;
            XmlDocument = xmlDocument;
        }
    }
}