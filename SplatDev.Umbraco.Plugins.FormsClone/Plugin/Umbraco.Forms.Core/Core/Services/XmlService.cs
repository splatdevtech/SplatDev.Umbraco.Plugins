
// Type: Umbraco.Forms.Core.Services.XmlService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;

using System.Xml;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;



#nullable enable
namespace Umbraco.Forms.Core.Services
{
    internal sealed class XmlService : IXmlService
    {
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IFieldPreValueSourceService _fieldPreValueSourceService;
        private readonly IFieldPreValueSourceTypeService _fieldPreValueSourceTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        private readonly IPublishedUrlProvider _publishedUrlProvider;
        private readonly IMemberService _memberService;
        private readonly ILogger<XmlService> _logger;

        public XmlService(
          IFieldTypeStorage fieldTypeStorage,
          IFieldPreValueSourceService fieldPreValueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
          IUmbracoContextAccessor umbracoContextAccessor,

          IPublishedUrlProvider publishedUrlProvider,
          IMemberService memberService,
          ILogger<XmlService> logger)
        {
            this._fieldTypeStorage = fieldTypeStorage;
            this._fieldPreValueSourceService = fieldPreValueSourceService;
            this._fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
            this._umbracoContextAccessor = umbracoContextAccessor;

            this._publishedUrlProvider = publishedUrlProvider;
            this._memberService = memberService;
            this._logger = logger;
        }

        public XmlNode ToXml(Record record, XmlDocument xd) => this.ToXml(record, xd, "Caption");

        public XmlNode ToXml(Record record, XmlDocument xd, string useAsElementNameForFields)
        {
            XmlNode xml = XmlHelper.AddTextNode(xd, "uformrecord", string.Empty);
            xml.AppendChild(XmlHelper.AddTextNode(xd, "state", record.State.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xd, "created", record.Created.ToString("s")));
            xml.AppendChild(XmlHelper.AddTextNode(xd, "updated", record.Updated.ToString("s")));
            xml.AppendChild(XmlHelper.AddTextNode(xd, "id", record.Id.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xd, "uniqueId", record.UniqueId.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xd, "ip", record.IP));
            xml.AppendChild(XmlHelper.AddTextNode(xd, "culture", record.Culture));
            XmlNode newChild1 = XmlHelper.AddTextNode(xd, "pageid", record.UmbracoPageId.ToString());
            string str1 = string.Empty;
            IUmbracoContext umbracoContext;
            if (!this._umbracoContextAccessor.TryGetUmbracoContext(out umbracoContext))
                throw new InvalidOperationException("Could not get Umbraco context");
            string url = this._publishedUrlProvider.GetUrl(record.UmbracoPageId);
            if (!string.IsNullOrEmpty(url))
            {
                IPublishedContent byId = umbracoContext.Content?.GetById(record.UmbracoPageId);
                if (byId != null)
                    str1 = byId.Name ?? string.Empty;
            }
            newChild1.Attributes?.Append(XmlHelper.AddAttribute(xd, "url", url));
            newChild1.Attributes?.Append(XmlHelper.AddAttribute(xd, "name", str1));
            xml.AppendChild(newChild1);
            string input = string.Empty;
            string str2 = string.Empty;
            string str3 = string.Empty;
            try
            {
                if (record.MemberKey != null)
                {
                    if (!string.IsNullOrEmpty(record.MemberKey))
                    {
                        input = record.MemberKey;
                        IMember byKey = this._memberService.GetByKey(Guid.Parse(input));
                        if (byKey != null)
                        {
                            str2 = byKey.Name ?? string.Empty;
                            str3 = byKey.Email;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error when serializing record to XML & trying to retrieve member by key {MemberKey}", record.MemberKey);
                throw;
            }
            XmlNode newChild2 = XmlHelper.AddTextNode(xd, "memberkey", input);
            newChild2.Attributes.Append(XmlHelper.AddAttribute(xd, "email", str3));
            newChild2.Attributes.Append(XmlHelper.AddAttribute(xd, "login", str2));
            xml.AppendChild(newChild2);
            XmlNode newChild3 = XmlHelper.AddTextNode(xd, "fields", string.Empty);
            foreach (RecordField recordField in record.RecordFields.Values)
                newChild3.AppendChild(this.ToXml(recordField, xd, useAsElementNameForFields));
            xml.AppendChild(newChild3);
            return xml;
        }

        public XmlNode ToXml(RecordField recordField, XmlDocument xd) => this.ToXml(recordField, xd, "Caption");

        public XmlNode ToXml(
          RecordField recordField,
          XmlDocument xmlDocument,
          string useAsElementNameForFields)
        {
            string fieldElementName = XmlService.GetFieldElementName(recordField, useAsElementNameForFields);
            XmlNode xml = XmlHelper.AddTextNode(xmlDocument, fieldElementName, string.Empty);
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "key", recordField.Key.ToString()));
            if (xml.Attributes == null)
                return xml;
            xml.Attributes.Append(XmlHelper.AddAttribute(xmlDocument, "record", recordField.Record.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "fieldKey", recordField.Field?.Id.ToString() ?? Guid.Empty.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "caption", recordField.Field?.Caption ?? fieldElementName));
            if (recordField.Field != null)
            {
                FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(recordField.Field);
                if (fieldTypeByField != null)
                {
                    XmlHelper.AddTextNode(xmlDocument, "datatype", fieldTypeByField.DataType.ToString()).Attributes?.Append(XmlHelper.AddAttribute(xmlDocument, "fieldtypeid", fieldTypeByField.Id.ToString()));
                    xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "datatype", fieldTypeByField.DataType.ToString()));
                }
            }
            XmlNode newChild1 = XmlHelper.AddTextNode(xmlDocument, "values", string.Empty);
            if (recordField.Field != null)
            {
                FieldPreValueSource byId1 = this._fieldPreValueSourceService.GetById(recordField.Field.PreValueSourceId);
                if (byId1 != null)
                {
                    FieldPreValueSourceType byId2 = this._fieldPreValueSourceTypeService.GetById(byId1.FieldPreValueSourceTypeId);
                    if (byId1 != null && byId2 != null)
                        byId2.LoadSettings(byId1);
                }
            }
            foreach (object obj in recordField.Values)
            {
                string str1 = obj.ToString();
                string str2 = XmlHelper.CleanInvalidXmlChars(str1?.Replace("\"", "&quot;"));
                XmlNode newChild2 = XmlHelper.AddCDataNode(xmlDocument, "value", str2.Trim());
                newChild2.Attributes?.Append(XmlHelper.AddAttribute(xmlDocument, "key", recordField.Key.ToString()));
                newChild1.AppendChild(newChild2);
            }
            xml.AppendChild(newChild1);
            return xml;
        }

        private static string GetFieldElementName(
          RecordField recordField,
          string useAsElementNameForFields)
        {
            string fieldElementName;
            if (!(useAsElementNameForFields == "Alias"))
            {
                if (useAsElementNameForFields == "Caption")
                    ;
                fieldElementName = XmlHelper.XmlName(recordField.Field?.Caption);
            }
            else
                fieldElementName = XmlHelper.XmlName(recordField.Field?.Alias);
            if (string.IsNullOrEmpty(fieldElementName))
                fieldElementName = "fieldcaption";
            return fieldElementName;
        }

        public XmlNode ToXml(Form form, XmlDocument xmlDocument)
        {
            XmlNode xml = XmlHelper.AddTextNode(xmlDocument, nameof(form), string.Empty);
            foreach (Page page in form.Pages)
                xml.AppendChild(this.ToXml(page, xmlDocument));
            return xml;
        }

        public XmlNode ToXml(Page page, XmlDocument xmlDocument)
        {
            XmlNode xml = XmlHelper.AddTextNode(xmlDocument, nameof(page), string.Empty);
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "id", page.Id.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "caption", page.Caption ?? string.Empty));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "sortorder", page.SortOrder.ToString()));
            foreach (FieldSet fieldSet in page.FieldSets)
                xml.AppendChild(this.ToXml(fieldSet, xmlDocument));
            return xml;
        }

        public XmlNode ToXml(FieldSet fieldSet, XmlDocument xmlDocument)
        {
            XmlNode xml = XmlHelper.AddTextNode(xmlDocument, "fieldset", string.Empty);
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "id", fieldSet.Id.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "caption", fieldSet.Caption ?? string.Empty));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "sortorder", fieldSet.SortOrder.ToString()));
            foreach (FieldsetContainer container in fieldSet.Containers)
                xml.AppendChild(this.ToXml(container, xmlDocument));
            return xml;
        }

        public XmlNode ToXml(FieldsetContainer fieldsetContainer, XmlDocument xmlDocument)
        {
            XmlNode xml = XmlHelper.AddTextNode(xmlDocument, "fieldsetcontainer", string.Empty);
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "width", fieldsetContainer.Width.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "caption", fieldsetContainer.Caption ?? string.Empty));
            foreach (Field field in fieldsetContainer.Fields)
                xml.AppendChild(this.ToXml(field, xmlDocument));
            return xml;
        }

        public XmlNode ToXml(Field field, XmlDocument xmlDocument)
        {
            XmlNode xml = XmlHelper.AddTextNode(xmlDocument, nameof(field), string.Empty);
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "id", field.Id.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "caption", field.Caption));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "mandatory", field.Mandatory.ToString()));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "regex", field.RegEx ?? string.Empty));
            xml.AppendChild(XmlHelper.AddTextNode(xmlDocument, "fieldtype", field.FieldTypeId.ToString()));
            return xml;
        }
    }
}
