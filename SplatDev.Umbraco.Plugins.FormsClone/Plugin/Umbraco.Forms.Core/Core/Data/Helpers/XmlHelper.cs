
// Type: Umbraco.Forms.Core.Data.Helpers.XmlHelper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Data.Helpers
{
  public class XmlHelper
  {
    private static readonly Regex s_invalidXmlChars = new Regex("(?<![\\uD800-\\uDBFF])[\\uDC00-\\uDFFF]|[\\uD800-\\uDBFF](?![\\uDC00-\\uDFFF])|[\\x00-\\x08\\x0B\\x0C\\x0E-\\x1F\\x7F-\\x9F\\uFEFF\\uFFFE\\uFFFF]", RegexOptions.Compiled);

    public static XmlNode XmlRecordsRootNode(XmlDocument xd, Form form)
    {
      XmlNode xmlNode = Umbraco.Cms.Core.Xml.XmlHelper.AddTextNode(xd, "uformrecords", string.Empty);
      if (xmlNode.Attributes == null)
        return xmlNode;
      if (form != null)
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
      if (char.IsNumber(input1.Substring(0, 1).ToCharArray()[0]))
        input1 = "n" + input1;
      return Regex.Replace(input1, "\\W*", string.Empty).ToLower();
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
      node.AppendChild((XmlNode) xmlDocument.CreateTextNode(value));
      return node;
    }

    public static XmlNode AddCDataNode(XmlDocument xmlDocument, string name, string value)
    {
      XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, name, string.Empty);
      node.AppendChild((XmlNode) xmlDocument.CreateCDataSection(value));
      return node;
    }

    public static string GetNodeValue(XmlNode n) => n == null || n.FirstChild == null ? string.Empty : n.FirstChild.Value ?? string.Empty;
  }
}
