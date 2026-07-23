using System.Xml;
using System.Xml.Xsl;

using Umbraco.Cms.Core.IO;

namespace FormBuilder.Core.Helpers
{
    public class XsltHelper
    {
        public static string TransformXML(string xml, string xsltFile)
        {
            XslCompiledTransform xslTransform = new();
            xslTransform.Load(xsltFile);
            return TransformXML(xml, xslTransform);
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
                    return TransformXML(xml, xslTransform);
                }
            }
        }

        private static string TransformXML(string xml, XslCompiledTransform xslTransform)
        {
            using (StringReader input1 = new StringReader(xml))
            {
                using (XmlReader input2 = XmlReader.Create(input1))
                {
                    using (StringWriter results = new StringWriter())
                    {
                        xslTransform.Transform(input2, null, results);
                        return results.ToString();
                    }
                }
            }
        }

        private static XsltArgumentList CreateXsltArgumentList(
          Dictionary<string, object> settings)
        {
            XsltArgumentList xsltArgumentList = new XsltArgumentList();
            if (settings is not null)
            {
                foreach (KeyValuePair<string, object> setting in settings)
                    xsltArgumentList.AddParam(setting.Key, string.Empty, setting.Value);
            }
            return xsltArgumentList;
        }
    }
}