using FormBuilder.Core.Attributes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;

using Microsoft.Extensions.Logging;

using System.Globalization;

using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Core.Providers.Prevalues
{
    /// <summary>
    /// Defines a     /// </summary>
    public class UmbracoPreValuesReadOnly : FieldPrevalueSourceType
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly ILogger<UmbracoPreValuesReadOnly> _logger;
        private readonly IIdKeyMap _idKeyMap;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public UmbracoPreValuesReadOnly(
          IDataTypeService dataTypeService,
          ILogger<UmbracoPreValuesReadOnly> logger,
          IIdKeyMap idKeyMap)
        {
            Id = new Guid("EA773CAF-FEF2-491B-B5B7-6A3552B1A0E2");
            Name = "Umbraco Data Type Prevalues";
            Alias = "umbracoDataTypePreValues";
            Description = "Connects to an Umbraco data type and uses its pre-value collection";
            Icon = "icon-nodes";
            _dataTypeService = dataTypeService;
            _logger = logger;
            _idKeyMap = idKeyMap;
        }

        /// <summary>Gets or sets the datatype containing the prevalues.</summary>
        [Setting("Data Type", Description = "Data type to use", DisplayOrder = 10, IsMandatory = true, View = "Forms.PropertyEditorUi.DataTypePicker")]
        public virtual string DataTypeId { get; set; } = string.Empty;

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            if (string.IsNullOrEmpty(DataTypeId))
            {
                exceptionList.Add(new Exception("'Data type' setting has not been set"));
            }
            else
            {
                if (int.TryParse(DataTypeId, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    if (_dataTypeService.GetDataType(result) == null)
                        exceptionList.Add(new Exception("Data type with id '" + DataTypeId + "' not found"));
#pragma warning restore CS0618 // Type or member is obsolete
                }
                else
                    exceptionList.Add(new Exception("'Data type' setting has not been set"));
            }
            return exceptionList;
        }

        /// <inheritdoc />
        public override async Task<List<Prevalue>> GetPreValuesAsync(
          Field? field,
          Form? form)
        {
            if (int.TryParse(DataTypeId, NumberStyles.Integer, CultureInfo.InvariantCulture, out int dataTypeId))
            {
                Umbraco.Cms.Core.Attempt<Guid> dataTypeKeyAttempt = _idKeyMap.GetKeyForId(dataTypeId, UmbracoObjectTypes.DataType);
                if (dataTypeKeyAttempt.Success)
                {
                    IDataType? dataType = await _dataTypeService.GetAsync(dataTypeKeyAttempt.Result).ConfigureAwait(false);
                    if (dataType is not null)
                        return GetPrevaluesBasedOnDataType(dataType);
                    _logger.LogDebug("Data Type with id {DataTypeId} and key '{DataTypeKey}' could not be found", dataTypeId, dataTypeKeyAttempt.Result);
                }
                else
                    _logger.LogDebug("Data Type with id {DataTypeId} could not be found", dataTypeId);
            }
            else
                _logger.LogDebug("The DataType ID {DataTypeId} could not be parsed as a real integer", DataTypeId);
            return [];
        }

        private static List<Prevalue> GetPrevaluesBasedOnDataType(IDataType dataType)
        {
            dataType.ConfigurationData.TryGetValue("items", out object? obj);
            if (obj == null)
                return [];
            return obj is not List<string> source ? [] : source.Select((x, i) => new Prevalue()
            {
                Id = x,
                Value = x,
                SortOrder = i
            }).ToList();
        }

        /// <summary>Gets the prevalues for the provided datatype Id.</summary>
        public async Task<List<Prevalue>> GetPreValues(string dataTypeId)
        {
            UmbracoPreValuesReadOnly preValuesReadOnly = this;
            preValuesReadOnly.DataTypeId = !string.IsNullOrEmpty(dataTypeId) ? dataTypeId : throw new Exception("'Datatype' setting has not been set");
            return await preValuesReadOnly.GetPreValuesAsync(null, null).ConfigureAwait(false);
        }
    }
}