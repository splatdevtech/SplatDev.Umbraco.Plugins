
// Type: Umbraco.Forms.Core.Services.IXmlService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Xml;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public interface IXmlService
  {
    XmlNode ToXml(Record record, XmlDocument xd);

    XmlNode ToXml(Record record, XmlDocument xd, string useAsElementNameForFields) => this.ToXml(record, xd);

    XmlNode ToXml(RecordField recordField, XmlDocument xmlDocument);

    XmlNode ToXml(
      RecordField recordField,
      XmlDocument xmlDocument,
      string useAsElementNameForFields)
    {
      return this.ToXml(recordField, xmlDocument);
    }

    XmlNode ToXml(Form form, XmlDocument xmlDocument);

    XmlNode ToXml(Page page, XmlDocument xmlDocument);

    XmlNode ToXml(FieldSet fieldSet, XmlDocument xmlDocument);

    XmlNode ToXml(FieldsetContainer fieldsetContainer, XmlDocument xmlDocument);

    XmlNode ToXml(Field field, XmlDocument xmlDocument);
  }
}
