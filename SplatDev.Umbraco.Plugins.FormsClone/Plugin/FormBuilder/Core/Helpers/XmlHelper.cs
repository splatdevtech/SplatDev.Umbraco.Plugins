using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;

using System.Text.RegularExpressions;
using System.Xml;

namespace FormBuilder.Extension.Forms.Core.Helpers
{
#pragma warning disable CS0618 // Type or member is obsolete

    public partial class XmlHelper
    {
        private static readonly Regex s_invalidXmlChars = NodePattern();

        public static XmlNode XmlRecordsRootNode(XmlDocument xd, Form form)
        {
            XmlNode xmlNode = Umbraco.Cms.Core.Xml.XmlHelper.AddTextNode(xd, "uformrecords", string.Empty);
            if (xmlNode.Attributes is null)
                return xmlNode;
            if (form is not null)
            {
                xmlNode.Attributes.Append(Umbraco.Cms.Core.Xml.XmlHelper.AddAttribute(xd, "formname", form.Name));
                xmlNode.Attributes.Append(Umbraco.Cms.Core.Xml.XmlHelper.AddAttribute(xd, "formid", form.Id.ToString()));
            }
            xmlNode.Attributes.Append(Umbraco.Cms.Core.Xml.XmlHelper.AddAttribute(xd, "created", DateTime.Now.ToString()));
            return xmlNode;
        }

        public static XmlNode ConvertRecordsToXml(
          IXmlService xmlService,
          Form form,
          List<Record> records,
          XmlDocument xd)
        {
            XmlNode xml = XmlHelper.XmlRecordsRootNode(xd, form);
            foreach (Record record in records)
                xml.AppendChild(xmlService.ToXml(record, xd));
            return xml;
        }

        public static string XmlName(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            string input1 = input;
            if (char.IsNumber(input1[..1].ToCharArray()[0]))
                input1 = "n" + input1;
            return XmlNamePattern().Replace(input1, string.Empty).ToLower();
        }

        public static string CleanInvalidXmlChars(string? text) => !string.IsNullOrEmpty(text) ? XmlHelper.s_invalidXmlChars.Replace(text, string.Empty) : string.Empty;

        public static XmlAttribute AddAttribute(
          XmlDocument xmlDocument,
          string name,
          string value)
        {
            XmlAttribute attribute = xmlDocument.CreateAttribute(name);
            attribute.Value = value;
            return attribute;
        }

        public static XmlNode AddTextNode(XmlDocument xmlDocument, string name, string value)
        {
            XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, name, string.Empty);
            node.AppendChild(xmlDocument.CreateTextNode(value));
            return node;
        }

        public static XmlNode AddCDataNode(XmlDocument xmlDocument, string name, string value)
        {
            XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, name, string.Empty);
            node.AppendChild(xmlDocument.CreateCDataSection(value));
            return node;
        }

        public static string GetNodeValue(XmlNode n) => n is null || n.FirstChild is null ? string.Empty : n.FirstChild.Value ?? string.Empty;

        [GeneratedRegex("(?<![\\uD800-\\uDBFF])[\\uDC00-\\uDFFF]|[\\uD800-\\uDBFF](?![\\uDC00-\\uDFFF])|[\\x00-\\x08\\x0B\\x0C\\x0E-\\x1F\\x7F-\\x9F\\uFEFF\\uFFFE\\uFFFF]", RegexOptions.Compiled)]
        public static partial Regex NodePattern();

        [GeneratedRegex("\\W*")]
        public static partial Regex XmlNamePattern();
    }

#pragma warning restore CS0618 // Type or member is obsolete
}