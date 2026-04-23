using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

using System.Xml;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IXmlService
    {
        XmlNode ToXml(Record record, XmlDocument xd);

        XmlNode ToXml(Record record, XmlDocument xd, string useAsElementNameForFields) => ToXml(record, xd);

        XmlNode ToXml(RecordField recordField, XmlDocument xmlDocument);

        XmlNode ToXml(
          RecordField recordField,
          XmlDocument xmlDocument,
          string useAsElementNameForFields)
        {
            return ToXml(recordField, xmlDocument);
        }

        XmlNode ToXml(Form form, XmlDocument xmlDocument);

        XmlNode ToXml(Page page, XmlDocument xmlDocument);

        XmlNode ToXml(FieldSet fieldSet, XmlDocument xmlDocument);

        XmlNode ToXml(FieldsetContainer fieldsetContainer, XmlDocument xmlDocument);

        XmlNode ToXml(Field field, XmlDocument xmlDocument);
    }
}