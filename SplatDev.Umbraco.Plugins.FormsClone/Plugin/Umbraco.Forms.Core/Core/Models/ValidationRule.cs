
// Type: Umbraco.Forms.Core.Models.ValidationRule
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "validationRule")]
  [Serializable]
  public class ValidationRule
  {
    [DataMember(Name = "rule")]
    public string Rule { get; set; } = string.Empty;

    [DataMember(Name = "errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;

    [DataMember(Name = "fieldId")]
    public Guid FieldId { get; set; }
  }
}
