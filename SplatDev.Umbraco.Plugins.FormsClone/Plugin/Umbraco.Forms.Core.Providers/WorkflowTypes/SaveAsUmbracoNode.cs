
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.SaveAsUmbracoNode
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Logging;

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers.Models;
using Umbraco.Forms.Core.Providers.PreValues;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public class SaveAsUmbracoNode : WorkflowType
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IContentService _contentService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IDictionaryItemService _dictionaryItemService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDynamicRootContentLocator _dynamicRootContentLocator;
        private readonly IIdKeyMap _idKeyMap;
        private readonly ILogger<SaveAsUmbracoNode> _logger;

        public SaveAsUmbracoNode(
          IContentTypeService contentTypeService,
          IContentService contentService,
          IDataTypeService dataTypeService,
          IDictionaryItemService dictionaryItemService,
          ILoggerFactory loggerFactory,
          ILogger<SaveAsUmbracoNode> logger,
          IDynamicRootContentLocator dynamicRootContentLocator,
          IIdKeyMap idKeyMap)
        {
            this.Id = new Guid("89FB1E31-9F36-4E08-9D1B-AF1180D340DB");
            this.Name = "Save as Umbraco content node";
            this.Alias = "saveAsUmbracoContentNode";
            this.Description = "Saves the form values as an content node using a specified document type";
            this.Icon = "icon-documents";
            this.Group = "Legacy";
            this._contentTypeService = contentTypeService;
            this._contentService = contentService;
            this._dataTypeService = dataTypeService;
            this._dictionaryItemService = dictionaryItemService;
            this._loggerFactory = loggerFactory;
            this._logger = logger;
            this._dynamicRootContentLocator = dynamicRootContentLocator;
            this._idKeyMap = idKeyMap;
        }

        [Setting("Document Type", Description = "Map document type", DisplayOrder = 10, IsMandatory = true, SupportsPlaceholders = true, View = "Forms.PropertyEditorUi.DocumentMapper")]
        public virtual string Fields { get; set; } = string.Empty;

        [Setting("Publish", Description = "Publish node instantly", DisplayOrder = 20, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string Publish { get; set; } = string.Empty;

        [Setting("Where to save", Description = "Choose the location for where to save the created content node", DisplayOrder = 30, View = "Umb.PropertyEditorUi.ContentPicker.Source")]
        public virtual string RootNode { get; set; } = string.Empty;

        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            DocumentMapper maps = JsonSerializer.Deserialize<DocumentMapper>(this.Fields, FormsJsonSerializerOptions.Default);
            if (maps == null)
            {
                this._logger.LogError("There was a problem saving a node from workflow associated to Form {FormName} with unique id {FormId}. Unable to deserialize the document type mapping.", context.Form.Name, context.Form.Id);
                return WorkflowExecutionStatus.Failed;
            }
            string nodeName = "NodeName";
            if (!string.IsNullOrEmpty(maps.NameStaticValue))
                nodeName = maps.NameStaticValue;
            else if (!string.IsNullOrEmpty(maps.NameField))
                nodeName = context.Record.GetRecordField(new Guid(maps.NameField))?.ValuesAsString(false) ?? string.Empty;
            Dictionary<string, string> mappings = new Dictionary<string, string>();
            foreach (DocumentMapping property in maps.Properties)
            {
                if (property.HasValue)
                {
                    string str = property.StaticValue;
                    if (!string.IsNullOrEmpty(property.Field))
                    {
                        RecordField recordField = context.Record.GetRecordField(new Guid(property.Field));
                        if (recordField != null)
                            str = recordField.ValuesAsString(false);
                        else
                            continue;
                    }
                    mappings.Add(property.Alias, str);
                }
            }
            IContentType contentType = this._contentTypeService.Get(maps.DocTypeAlias);
            if (contentType == null)
                return WorkflowExecutionStatus.Completed;
            IContent content = this._dynamicRootContentLocator.CreateContent(this.RootNode, nodeName, contentType.Alias, context.Record.UmbracoPageId).Result;
            if (content == null)
                return WorkflowExecutionStatus.Completed;
            if (content.Properties.Count != 0)
            {
                foreach (IProperty property1 in (IEnumerable<IProperty>)content.Properties)
                {
                    IProperty property = property1;
                    try
                    {
                        if (mappings.ContainsKey(property.Alias))
                        {
                            IPropertyType propertyType = contentType.CompositionPropertyTypes.FirstOrDefault<IPropertyType>(x => string.Equals(x.Alias, property.Alias, StringComparison.InvariantCultureIgnoreCase));
                            if (propertyType != null)
                            {
                                IDataType dataType = await this._dataTypeService.GetAsync(propertyType.DataTypeKey).ConfigureAwait(false);
                                if (dataType != null)
                                {
                                    string s = mappings[property.Alias].Trim();
                                    switch (dataType.DatabaseType)
                                    {
                                        case ValueStorageType.Integer:
                                            string editorAlias = dataType.EditorAlias;
                                            ConfiguredTaskAwaitable<int> configuredTaskAwaitable;
                                            if (!(editorAlias == "Umbraco.DropDown.Flexible"))
                                            {
                                                if (editorAlias == "Umbraco.RadioButtonList")
                                                {
                                                    configuredTaskAwaitable = this.GetPublishingKey(dataType.Id, s).ConfigureAwait(false);
                                                    property.SetValue(await configuredTaskAwaitable);
                                                    break;
                                                }
                                                string str = s;
                                                if (str.Equals("true", StringComparison.InvariantCultureIgnoreCase) || str.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    bool result;
                                                    if (bool.TryParse(str, out result))
                                                    {
                                                        property.SetValue(result);
                                                        break;
                                                    }
                                                    break;
                                                }
                                                int result1;
                                                if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result1))
                                                {
                                                    property.SetValue(result1);
                                                    break;
                                                }
                                                break;
                                            }
                                            configuredTaskAwaitable = this.GetPublishingKey(dataType.Id, s).ConfigureAwait(false);
                                            property.SetValue(await configuredTaskAwaitable);
                                            break;
                                        case ValueStorageType.Date:
                                            if (!string.IsNullOrEmpty(s))
                                            {
                                                DateTime result;
                                                if (!DateTime.TryParse(s, out result))
                                                {
                                                    property.SetValue(s);
                                                    break;
                                                }
                                                property.SetValue(result);
                                                break;
                                            }
                                            break;
                                        default:
                                            if (dataType.EditorAlias == "Umbraco.DropDown.Flexible")
                                            {
                                                property.SetValue(await this.GetPublishingKeys(dataType.Id, s).ConfigureAwait(false));
                                                break;
                                            }
                                            property.SetValue(s);
                                            break;
                                    }
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        }
                        else
                            continue;
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "There was a problem saving a node of DocumentType {DocumentType} from workflow associated to Form {FormName} with unique id {FormId}", maps.DocTypeAlias, context.Form.Name, context.Form.Id);
                        return WorkflowExecutionStatus.Failed;
                    }
                }
            }
            this._contentService.Save(content);
            if (this.Publish == true.ToString())
                this._contentService.Publish(content, new string[1]
                {
          "*"
                });
            return WorkflowExecutionStatus.Completed;
        }

        private async Task<int> GetPublishingKey(int dataTypeId, string value)
        {
            foreach (PreValue preValue in await new UmbracoPreValuesReadOnly(this._dataTypeService, this._loggerFactory.CreateLogger<UmbracoPreValuesReadOnly>(), this._idKeyMap).GetPreValues(dataTypeId.ToString()).ConfigureAwait(false))
            {
                if (preValue.Value.StartsWith('#'))
                {
                    IDictionaryItem dictionaryItem = await this._dictionaryItemService.GetAsync(preValue.Value.Remove(0, 1)).ConfigureAwait(false);
                    if (dictionaryItem != null)
                    {
                        IDictionaryTranslation dictionaryTranslation = dictionaryItem.Translations.FirstOrDefault<IDictionaryTranslation>(x => x.LanguageIsoCode == Thread.CurrentThread.CurrentUICulture.Name);
                        if (dictionaryTranslation != null && string.Equals(dictionaryTranslation.Value, value, StringComparison.InvariantCultureIgnoreCase))
                            return int.Parse(preValue.Id);
                    }
                    else
                        this._logger.LogError("Could not find dictionary item or any translations when mapping a PreValue that is a dictionary item back to a PreValue ID for key {PreValue}", preValue.Value);
                }
                if (string.Equals(preValue.Value, value, StringComparison.InvariantCultureIgnoreCase))
                    return int.Parse(preValue.Id);
            }
            return 0;
        }

        private async Task<string> GetPublishingKeys(int dataTypeId, string value)
        {
            List<PreValue> source = await new UmbracoPreValuesReadOnly(this._dataTypeService, this._loggerFactory.CreateLogger<UmbracoPreValuesReadOnly>(), this._idKeyMap).GetPreValues(dataTypeId.ToString()).ConfigureAwait(false);
            IEnumerable<string> values;
            if (value.Contains(","))
                values = value.Split(',').Select<string, string>(p => p.Trim());
            else
                values = (new string[1]
                {
          value
                });
            return JsonSerializer.Serialize<IEnumerable<string>>(source.Where<PreValue>(p => values.Contains<string>(p.Value)).Select<PreValue, string>(q => q.Value), FormsJsonSerializerOptions.Default);
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            if (string.IsNullOrEmpty(this.Fields))
                exceptionList.Add(new Exception("'Document type' setting with field mapping has not been completed."));
            return exceptionList;
        }
    }
}
