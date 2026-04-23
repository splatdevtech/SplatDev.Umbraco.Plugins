
// Type: Umbraco.Forms.Web.Models.Backoffice.DataSourceWizardFieldMapping
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "dataSourceWizardFieldMapping")]
  [Serializable]
  public class DataSourceWizardFieldMapping
  {
    [DataMember(Name = "key")]
    public string Key { get; set; } = string.Empty;

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "include")]
    public bool Include { get; set; }

    [DataMember(Name = "prevalueKeyField")]
    public string PrevalueKeyField { get; set; } = string.Empty;

    [DataMember(Name = "prevalueValueField")]
    public string PrevalueValueField { get; set; } = string.Empty;

    [DataMember(Name = "prevalueSource")]
    public string PrevalueSource { get; set; } = string.Empty;

    [DataMember(Name = "availablePrevalueValueFields")]
    public List<string> AvailablePrevalueValueFields { get; set; } = new List<string>();

    [DataMember(Name = "isForeignKey")]
    public bool IsForeignKey { get; set; }

    [DataMember(Name = "isMandatory")]
    public bool IsMandatory { get; set; }

    [DataMember(Name = "dataType")]
    public FieldDataType DataType { get; set; }

    [DataMember(Name = "defaultValue")]
    public string DefaultValue { get; set; } = string.Empty;

    [DataMember(Name = "fieldTypeId")]
    public Guid FieldTypeId { get; set; }

    [DataMember(Name = "isPrimaryKey")]
    public bool IsPrimaryKey { get; set; }

    [DataMember(Name = "allowNulls")]
    public bool AllowNulls { get; set; }
  }
}
