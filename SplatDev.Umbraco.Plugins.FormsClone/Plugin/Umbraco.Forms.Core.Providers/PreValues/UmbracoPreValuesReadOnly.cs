
// Type: Umbraco.Forms.Core.Providers.PreValues.UmbracoPreValuesReadOnly
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Logging;

using System.Globalization;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Providers.PreValues
{
    public class UmbracoPreValuesReadOnly : FieldPreValueSourceType
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly ILogger<UmbracoPreValuesReadOnly> _logger;
        private readonly IIdKeyMap _idKeyMap;

        public UmbracoPreValuesReadOnly(
          IDataTypeService dataTypeService,
          ILogger<UmbracoPreValuesReadOnly> logger,
          IIdKeyMap idKeyMap)
        {
            this.Id = new Guid("EA773CAF-FEF2-491B-B5B7-6A3552B1A0E2");
            this.Name = "Umbraco Data Type Prevalues";
            this.Alias = "umbracoDataTypePreValues";
            this.Description = "Connects to an Umbraco data type and uses its pre-value collection";
            this.Icon = "icon-nodes";
            this._dataTypeService = dataTypeService;
            this._logger = logger;
            this._idKeyMap = idKeyMap;
        }

        [Setting("Data Type", Description = "Data type to use", DisplayOrder = 10, IsMandatory = true, View = "Forms.PropertyEditorUi.DataTypePicker")]
        public virtual string DataTypeId { get; set; } = string.Empty;

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            if (string.IsNullOrEmpty(this.DataTypeId))
            {
                exceptionList.Add(new Exception("'Data type' setting has not been set"));
            }
            else
            {
                int result;
                if (int.TryParse(this.DataTypeId, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                {
                    if (this._dataTypeService.GetDataType(result) == null)
                        exceptionList.Add(new Exception("Data type with id '" + this.DataTypeId + "' not found"));
                }
                else
                    exceptionList.Add(new Exception("'Data type' setting has not been set"));
            }
            return exceptionList;
        }

        public override async Task<List<PreValue>> GetPreValuesAsync(
          Field? field,
          Form? form)
        {
            int dataTypeId;
            if (int.TryParse(this.DataTypeId, NumberStyles.Integer, CultureInfo.InvariantCulture, out dataTypeId))
            {
                Umbraco.Cms.Core.Attempt<Guid> dataTypeKeyAttempt = this._idKeyMap.GetKeyForId(dataTypeId, UmbracoObjectTypes.DataType);
                if (dataTypeKeyAttempt.Success)
                {
                    IDataType dataType = await this._dataTypeService.GetAsync(dataTypeKeyAttempt.Result).ConfigureAwait(false);
                    if (dataType != null)
                        return UmbracoPreValuesReadOnly.GetPrevaluesBasedOnDataType(dataType);
                    this._logger.LogDebug("Data Type with id {DataTypeId} and key '{DataTypeKey}' could not be found", dataTypeId, dataTypeKeyAttempt.Result);
                }
                else
                    this._logger.LogDebug("Data Type with id {DataTypeId} could not be found", dataTypeId);
                dataTypeKeyAttempt = new Umbraco.Cms.Core.Attempt<Guid>();
            }
            else
                this._logger.LogDebug("The DataType ID {DataTypeId} could not be parsed as a real integer", DataTypeId);
            return new List<PreValue>();
        }

        private static List<PreValue> GetPrevaluesBasedOnDataType(IDataType dataType)
        {
            object obj;
            dataType.ConfigurationData.TryGetValue("items", out obj);
            if (obj == null)
                return new List<PreValue>();
            return !(obj is List<string> source) ? new List<PreValue>() : source.Select<string, PreValue>((x, i) => new PreValue()
            {
                Id = x,
                Value = x,
                SortOrder = i
            }).ToList<PreValue>();
        }

        public async Task<List<PreValue>> GetPreValues(string dataTypeId)
        {
            UmbracoPreValuesReadOnly preValuesReadOnly = this;
            preValuesReadOnly.DataTypeId = !string.IsNullOrEmpty(dataTypeId) ? dataTypeId : throw new Exception("'Datatype' setting has not been set");
            return await preValuesReadOnly.GetPreValuesAsync(null, null).ConfigureAwait(false);
        }
    }
}
