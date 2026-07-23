
// Type: Umbraco.Forms.Core.Data.Helpers.XsltHelper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Umbraco.Cms.Core.IO;


#nullable enable
namespace Umbraco.Forms.Core.Data.Helpers
{
  public class XsltHelper
  {
    public static string TransformXML(string xml, string xsltFile)
    {
      XslCompiledTransform xslTransform = new XslCompiledTransform();
      xslTransform.Load(xsltFile);
      return XsltHelper.TransformXML(xml, xslTransform);
    }

    public static string TransformXML(
      string xml,
      string xsltFile,
      MediaFileManager mediaFileManager)
    {
      if (!mediaFileManager.FileSystem.FileExists(xsltFile))
        return xml;
      XslCompiledTransform xslTransform = new XslCompiledTransform();
      using (Stream input = mediaFileManager.FileSystem.OpenFile(xsltFile))
      {
        using (XmlReader stylesheet = XmlReader.Create(input))
        {
          xslTransform.Load(stylesheet);
          return XsltHelper.TransformXML(xml, xslTransform);
        }
      }
    }

    private static string TransformXML(string xml, XslCompiledTransform xslTransform)
    {
      using (StringReader input1 = new StringReader(xml))
      {
        using (XmlReader input2 = XmlReader.Create((TextReader) input1))
        {
          using (StringWriter results = new StringWriter())
          {
            xslTransform.Transform(input2, (XsltArgumentList) null, (TextWriter) results);
            return results.ToString();
          }
        }
      }
    }

    private static XsltArgumentList CreateXsltArgumentList(
      Dictionary<string, object> settings)
    {
      XsltArgumentList xsltArgumentList = new XsltArgumentList();
      if (settings != null)
      {
        foreach (KeyValuePair<string, object> setting in settings)
          xsltArgumentList.AddParam(setting.Key, string.Empty, setting.Value);
      }
      return xsltArgumentList;
    }
  }
}
