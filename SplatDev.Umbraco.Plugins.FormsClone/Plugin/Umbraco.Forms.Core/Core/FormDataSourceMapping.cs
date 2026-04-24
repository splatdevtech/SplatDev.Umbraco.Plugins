
// Type: Umbraco.Forms.Core.FormDataSourceMapping
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core
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
