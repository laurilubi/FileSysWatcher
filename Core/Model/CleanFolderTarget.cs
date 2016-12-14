using System.Xml;
using Core.Tools;

namespace Core.Model
{
    public class CleanFolderTarget
    {
        public bool IsEnabled { get; set; }

        public string FolderPath { get; set; }

        public int KeepFilesInDays { get; set; }

        public CleanFolderTarget()
        { }

        public CleanFolderTarget(XmlElement node)
            : this()
        {
            IsEnabled = BoolTools.GetBoolOrDefault(node.GetAttribute("active"));
            FolderPath = StringTools.GetValueOrDefault(node.GetAttribute("folder"));
            KeepFilesInDays = NumberTools.GetIntOrDefault(node.GetAttribute("maxAge"), 1000000);
        }

        public void CopyPropertiesTo(XmlElement node)
        {
            node.SetAttribute("active", IsEnabled.ToString().ToLower());
            node.SetAttribute("folder", FolderPath);
            node.SetAttribute("maxAge", KeepFilesInDays.ToString());
        }
    }
}
