using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers.Models;
using FormBuilder.Core.Providers.Prevalues;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.Logging;

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides a     /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
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
            Id = new Guid("89FB1E31-9F36-4E08-9D1B-AF1180D340DB");
            Name = "Save as Umbraco content node";
            Alias = "saveAsUmbracoContentNode";
            Description = "Saves the form values as an content node using a specified document type";
            Icon = "icon-documents";
            Group = "Legacy";
            _contentTypeService = contentTypeService;
            _contentService = contentService;
            _dataTypeService = dataTypeService;
            _dictionaryItemService = dictionaryItemService;
            _loggerFactory = loggerFactory;
            _logger = logger;
            _dynamicRootContentLocator = dynamicRootContentLocator;
            _idKeyMap = idKeyMap;
        }

        /// <summary>
        /// Gets or sets the fields to use to map to the document type.
        /// </summary>
        [Setting("Document Type", Description = "Map document type", DisplayOrder = 10, IsMandatory = true, SupportsPlaceholders = true, View = "Forms.PropertyEditorUi.DocumentMapper")]
        public virtual string Fields { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to publish the created node instantly.
        /// </summary>
        [Setting("Publish", Description = "Publish node instantly", DisplayOrder = 20, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string Publish { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent node under which the node will be created.
        /// </summary>
        [Setting("Where to save", Description = "Choose the location for where to save the created content node", DisplayOrder = 30, View = "Umb.PropertyEditorUi.ContentPicker.Source")]
        public virtual string RootNode { get; set; } = string.Empty;

        /// <inheritdoc />
        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            DocumentMapper? maps = JsonSerializer.Deserialize<DocumentMapper>(Fields, FormsJsonSerializerOptions.Default);
            if (maps == null)
            {
                _logger.LogError("There was a problem saving a node from workflow associated to Form {FormName} with unique id {FormId}. Unable to deserialize the document type mapping.", context.Form.Name, context.Form.Id);
                return WorkflowExecutionStatus.Failed;
            }
            string nodeName = "NodeName";
            if (!string.IsNullOrEmpty(maps.NameStaticValue))
                nodeName = maps.NameStaticValue;
            else if (!string.IsNullOrEmpty(maps.NameField))
                nodeName = context.Record.GetRecordField(new Guid(maps.NameField))?.ValuesAsString(false) ?? string.Empty;
            Dictionary<string, string> mappings = [];
            foreach (DocumentMapping property in maps.Properties)
            {
                if (property.HasValue)
                {
                    string str = property.StaticValue;
                    if (!string.IsNullOrEmpty(property.Field))
                    {
                        RecordField? recordField = context.Record.GetRecordField(new Guid(property.Field));
                        if (recordField is not null)
                            str = recordField.ValuesAsString(false);
                        else
                            continue;
                    }
                    mappings.Add(property.Alias, str);
                }
            }
            IContentType? contentType = _contentTypeService.Get(maps.DocTypeAlias);
            if (contentType == null)
                return WorkflowExecutionStatus.Completed;
            IContent? content = _dynamicRootContentLocator.CreateContent(RootNode, nodeName, contentType.Alias, context.Record.UmbracoPageId).Result;
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
                            IPropertyType? propertyType = contentType.CompositionPropertyTypes.FirstOrDefault(x => string.Equals(x.Alias, property.Alias, StringComparison.InvariantCultureIgnoreCase));
                            if (propertyType is not null)
                            {
                                IDataType? dataType = await _dataTypeService.GetAsync(propertyType.DataTypeKey).ConfigureAwait(false);
                                if (dataType is not null)
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
                                                    configuredTaskAwaitable = GetPublishingKey(dataType.Id, s).ConfigureAwait(false);
                                                    property.SetValue(await configuredTaskAwaitable);
                                                    break;
                                                }
                                                string str = s;
                                                if (str.Equals("true", StringComparison.InvariantCultureIgnoreCase) || str.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    if (bool.TryParse(str, out bool result))
                                                    {
                                                        property.SetValue(result);
                                                        break;
                                                    }
                                                    break;
                                                }
                                                if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result1))
                                                {
                                                    property.SetValue(result1);
                                                    break;
                                                }
                                                break;
                                            }
                                            configuredTaskAwaitable = GetPublishingKey(dataType.Id, s).ConfigureAwait(false);
                                            property.SetValue(await configuredTaskAwaitable);
                                            break;

                                        case ValueStorageType.Date:
                                            if (!string.IsNullOrEmpty(s))
                                            {
                                                if (!DateTime.TryParse(s, out DateTime result))
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
                                                property.SetValue(await GetPublishingKeys(dataType.Id, s).ConfigureAwait(false));
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
                        _logger.LogError(ex, "There was a problem saving a node of DocumentType {DocumentType} from workflow associated to Form {FormName} with unique id {FormId}", maps.DocTypeAlias, context.Form.Name, context.Form.Id);
                        return WorkflowExecutionStatus.Failed;
                    }
                }
            }
            _contentService.Save(content);
            if (Publish == true.ToString())
                _contentService.Publish(content,
                [
                    "*"
                ]);
            return WorkflowExecutionStatus.Completed;
        }

        private async Task<int> GetPublishingKey(int dataTypeId, string value)
        {
            foreach (Prevalue preValue in await new UmbracoPreValuesReadOnly(_dataTypeService, _loggerFactory.CreateLogger<UmbracoPreValuesReadOnly>(), _idKeyMap).GetPreValues(dataTypeId.ToString()).ConfigureAwait(false))
            {
                if (preValue.Value.StartsWith('#'))
                {
                    IDictionaryItem? dictionaryItem = await _dictionaryItemService.GetAsync(preValue.Value[1..]).ConfigureAwait(false);
                    if (dictionaryItem is not null)
                    {
                        IDictionaryTranslation? dictionaryTranslation = dictionaryItem.Translations.FirstOrDefault(x => x.LanguageIsoCode == Thread.CurrentThread.CurrentUICulture.Name);
                        if (dictionaryTranslation is not null && string.Equals(dictionaryTranslation.Value, value, StringComparison.InvariantCultureIgnoreCase))
                            return int.Parse(preValue.Id);
                    }
                    else
                        _logger.LogError("Could not find dictionary item or any translations when mapping a Prevalue that is a dictionary item back to a Prevalue ID for key {Prevalue}", preValue.Value);
                }
                if (string.Equals(preValue.Value, value, StringComparison.InvariantCultureIgnoreCase))
                    return int.Parse(preValue.Id);
            }
            return 0;
        }

        private async Task<string> GetPublishingKeys(int dataTypeId, string value)
        {
            List<Prevalue> source = await new UmbracoPreValuesReadOnly(_dataTypeService, _loggerFactory.CreateLogger<UmbracoPreValuesReadOnly>(), _idKeyMap).GetPreValues(dataTypeId.ToString()).ConfigureAwait(false);
            IEnumerable<string> values;
            if (value.Contains(','))
                values = value.Split(',').Select(p => p.Trim());
            else
                values =
                [
          value
                ];
            return JsonSerializer.Serialize(source.Where(p => values.Contains(p.Value)).Select(q => q.Value), FormsJsonSerializerOptions.Default);
        }

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            if (string.IsNullOrEmpty(Fields))
                exceptionList.Add(new Exception("'Document type' setting with field mapping has not been completed."));
            return exceptionList;
        }
    }
}