using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    [DataContract(Name = "entryExportFilterModel")]
    public class RecordExportFilter : RecordFilter
    {
        [DataMember(Name = "exportType")]
        public string ExportType { get; set; } = string.Empty;
    }
}