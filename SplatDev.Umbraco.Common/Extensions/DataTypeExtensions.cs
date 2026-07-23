#if !NET10_0_OR_GREATER
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
#endif

namespace SplatDev.Umbraco.Common.Extensions
{
#if !NET10_0_OR_GREATER
    public static partial class DataTypeExtensions
    {
        public static Dictionary<int, string>? GetByDataTypeName(IDataTypeService dataTypeService, string dataTypeName, int keyId = 0) => DataTypeListContent(dataTypeService.GetDataType(dataTypeName), keyId);

        public static Dictionary<int, string>? GetByDataTypeId(IDataTypeService dataTypeService, int dataTypeId, int keyId = 0) => DataTypeListContent(dataTypeService.GetDataType(dataTypeId), keyId);

        private static Dictionary<int, string>? DataTypeListContent(IDataType? dataTypeList, int keyId)
        {
            Dictionary<int, string> dataTypeValues = [];

            if (dataTypeList != null)
            {
                ValueListConfiguration? kvpList = dataTypeList.Configuration as ValueListConfiguration;

                if (kvpList is not null && kvpList.Items is not null)
                {
                    foreach (var kvp in kvpList.Items)
                    {
                        if (kvp.Value is not null)
                            dataTypeValues.Add(kvp.Id, kvp.Value);
                    }
                }

                if (keyId > 0)
                {
                    return dataTypeValues.Where(kvp => kvp.Key == keyId).ToDictionary(p => p.Key, p => p.Value);
                }

                return dataTypeValues;
            }

            return [];
        }

        public static IEnumerable<string> GetDropdownPreValues(this IDataTypeService service, int dataTypeId)
        {
            var dataType = service.GetDataType(dataTypeId);

            if (dataType == null) return [];

            if (dataType.Configuration is not ValueListConfiguration preValues) return [];

            return preValues!.Items!.Select(x => x.Value)!;
        }
    }
#endif
}
