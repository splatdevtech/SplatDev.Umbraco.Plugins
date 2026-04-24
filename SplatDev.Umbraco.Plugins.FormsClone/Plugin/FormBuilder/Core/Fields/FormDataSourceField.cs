using FormBuilder.Core.Enums;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FormBuilder.Core.Fields
{
    [DataContract(Name = "formDataSourceField")]
    public class FormDataSourceField
    {
        [DataMember(Name = "key")]
        public string Key { get; set; } = string.Empty;

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "prevalueKeyField")]
        public string PreValueKeyField { get; set; } = string.Empty;

        [DataMember(Name = "prevalueValueField")]
        public string PreValueValueField { get; set; } = string.Empty;

        [DataMember(Name = "prevalueSource")]
        public string PreValueSource { get; set; } = string.Empty;

        [DataMember(Name = "availablePrevalueValueFields")]
        public List<string> AvailablePreValueValueFields { get; set; } = [];

        [DataMember(Name = "isForeignKey")]
        public bool IsForeignKey { get; set; }

        [DataMember(Name = "type")]
        public Type? Type { get; set; }

        [DataMember(Name = "autoIncrement")]
        public bool AutoIncrement { get; set; }

        [DataMember(Name = "maxLength")]
        public int MaxLength { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "isPrimaryKey")]
        public bool IsPrimaryKey { get; set; }

        [DataMember(Name = "allowNulls")]
        public bool AllowNulls { get; set; }

        [DataMember(Name = "isMandatory")]
        public bool IsMandatory { get; set; }

        [DataMember(Name = "isProtected")]
        public bool IsProtected { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        public FieldDataType FieldDataType
        {
            get
            {
                if (IsBoolean)
                    return FieldDataType.Bit;
                if (IsNumeric)
                    return FieldDataType.Integer;
                if (IsDateTime)
                    return FieldDataType.DateTime;
                return IsString && MaxLength > 5000 ? FieldDataType.LongString : FieldDataType.String;
            }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        public bool IsBoolean => Type == typeof(bool);

        [IgnoreDataMember]
        [JsonIgnore]
        public bool IsNumeric => Type == typeof(int) || Type == typeof(int) || Type == typeof(long) || Type == typeof(short) || Type == typeof(uint) || Type == typeof(ushort) || Type == typeof(uint) || Type == typeof(ulong) || Type == typeof(float) || Type == typeof(decimal) || Type == typeof(double);

        [IgnoreDataMember]
        [JsonIgnore]
        public bool IsDateTime => Type == typeof(DateTime);

        [IgnoreDataMember]
        [JsonIgnore]
        public bool IsString => !IsDateTime && !IsNumeric && !IsBoolean;

        public static FormDataSourceField Create() => new()
        {
            AvailablePreValueValueFields = []
        };
    }
}