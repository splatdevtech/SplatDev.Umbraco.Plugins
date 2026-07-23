
// Type: Umbraco.Forms.Core.FormDataSourceField
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core
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
    public List<string> AvailablePreValueValueFields { get; set; } = new List<string>();

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
        if (this.IsBoolean)
          return FieldDataType.Bit;
        if (this.IsNumeric)
          return FieldDataType.Integer;
        if (this.IsDateTime)
          return FieldDataType.DateTime;
        return this.IsString && this.MaxLength > 5000 ? FieldDataType.LongString : FieldDataType.String;
      }
    }

    [IgnoreDataMember]
    [JsonIgnore]
    public bool IsBoolean => this.Type == typeof (bool);

    [IgnoreDataMember]
    [JsonIgnore]
    public bool IsNumeric => this.Type == typeof (int) || this.Type == typeof (int) || this.Type == typeof (long) || this.Type == typeof (short) || this.Type == typeof (uint) || this.Type == typeof (ushort) || this.Type == typeof (uint) || this.Type == typeof (ulong) || this.Type == typeof (float) || this.Type == typeof (Decimal) || this.Type == typeof (double);

    [IgnoreDataMember]
    [JsonIgnore]
    public bool IsDateTime => this.Type == typeof (DateTime);

    [IgnoreDataMember]
    [JsonIgnore]
    public bool IsString => !this.IsDateTime && !this.IsNumeric && !this.IsBoolean;

    public static FormDataSourceField Create() => new FormDataSourceField()
    {
      AvailablePreValueValueFields = new List<string>()
    };
  }
}
