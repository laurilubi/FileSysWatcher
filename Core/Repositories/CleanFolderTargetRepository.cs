using System.Collections.Generic;
using System.Xml;
using Core.Model;
using Core.Services;

namespace Core.Repositories
{
    public class CleanFolderTargetRepository
    {
        private readonly ConfigService configService;
        private const string StartXpath = "CleanFolderService";

        public CleanFolderTargetRepository(ConfigService configService)
        {
            this.configService = configService;
        }

        public List<CleanFolderTarget> GetAll()
        {
            var ret = new List<CleanFolderTarget>();
            var startNode = configService.GetElement(StartXpath);
            if (startNode == null) return ret;

            var nodes = startNode.SelectNodes("Delete");
            if (nodes == null) return ret;

            foreach (XmlElement node in nodes)
            {
                var target = new CleanFolderTarget(node);
                ret.Add(target);
            }

            return ret;
        }

        public void SaveAll(List<CleanFolderTarget> targets)
        {
            var startNode = configService.GetAssuredElement(StartXpath);

            var childrenToRemove = new List<XmlElement>();
            foreach (XmlNode childNode in startNode.ChildNodes)
            {
                if (!(childNode is XmlElement)) continue;
                childrenToRemove.Add((XmlElement)childNode);
            }
            childrenToRemove.ForEach(_ => startNode.RemoveChild(_));

            foreach (var target in targets)
            {
                var xml = startNode.OwnerDocument.CreateElement("Delete");
                target.CopyPropertiesTo(xml);
                startNode.AppendChild(xml);
            }

            configService.Save();
        }
    }
}
