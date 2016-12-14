using System.Xml;

namespace Core.Tools
{
    public static class XmlTools
    {
        public static XmlElement AssureElement(XmlElement relativeTo, string simpleXpath)
        {
            var nodeNames = simpleXpath.Split('/');
            foreach (var nodeName in nodeNames)
            {
                var nextNode = relativeTo.SelectSingleNode(nodeName);
                if (nextNode != null && nextNode is XmlElement)
                {
                    relativeTo = (XmlElement)nextNode;
                    continue;
                }

                nextNode = relativeTo.OwnerDocument.CreateElement(nodeName);
                relativeTo.AppendChild(nextNode);
                relativeTo = (XmlElement)nextNode;
            }
            return relativeTo;
        }
    }
}
