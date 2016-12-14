using System.IO;
using System.Xml;
using Core.Tools;

namespace Core.Services
{
    public class ConfigService
    {
        private readonly string configFilePath;
        private XmlDocument doc;
        private readonly FileSystemWatcher configWatcher;

        public ConfigService(string configFilePath)
        {
            this.configFilePath = configFilePath;
            CreateMissingConfig();

            doc = new XmlDocument();
            doc.Load(configFilePath);

            var configFileInfo = new FileInfo(configFilePath);
            configWatcher = new FileSystemWatcher(configFileInfo.Directory.FullName, configFileInfo.Name);
        }

        private void CreateMissingConfig()
        {
            if (File.Exists(configFilePath)) return;

            var configTemplateFilePath = configFilePath.Replace("config.xml", "config.template.xml");
            File.Copy(configTemplateFilePath, configFilePath);
        }

        public XmlNode GetNode(string xpath)
        {
            return doc.SelectSingleNode(xpath);
        }

        public XmlElement GetElement(string relativeXpath)
        {
            return doc.DocumentElement.SelectSingleNode(relativeXpath) as XmlElement;
        }

        public XmlElement GetAssuredElement(string simpleXpath)
        {
            return XmlTools.AssureElement(doc.DocumentElement, simpleXpath);
        }

        public void Save()
        {
            doc.Save(configFilePath);
            configWatcher.Changed += ConfigWatcher_Changed;
        }

        private void ConfigWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public event FileSystemEventHandler OnConfigChanged
        {
            add
            {
                configWatcher.Changed += value;
            }
            remove
            {
                configWatcher.Changed -= value;
            }
        }
    }
}
