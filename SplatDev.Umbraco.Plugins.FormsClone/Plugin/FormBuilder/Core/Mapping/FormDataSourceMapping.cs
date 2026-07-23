using FormBuilder.Core.Enums;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Mapping
{
    [DataContract(Name = "formDataSourceMapping")]
    public class FormDataSourceMapping
    {
        [DataMember(Name = "formId")]
        public Guid FormId { get; set; }

        [DataMember(Name = "dataFieldKey")]
        public string DataFieldKey { get; set; } = string.Empty;

        [DataMember(Name = "prevalueKeyField")]
        public string PrevalueKeyfield { get; set; } = string.Empty;

        [DataMember(Name = "prevalueValueField")]
        public string PrevalueValueField { get; set; } = string.Empty;

        [DataMember(Name = "prevalueTable")]
        public string PrevalueTable { get; set; } = string.Empty;

        [DataMember(Name = "dataType")]
        public FieldDataType DataType { get; set; }

        [DataMember(Name = "defaultValue")]
        public string DefaultValue { get; set; } = string.Empty;
    }
}