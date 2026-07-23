
// Type: Umbraco.Forms.Core.Persistence.Dtos.FormSecurityBaseDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;
using System;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Dtos
{
  [DataContract(Name = "formSecurityBase")]
  public abstract class FormSecurityBaseDto
  {
    [Ignore]
    [DataMember(Name = "formName")]
    public string FormName { get; set; } = string.Empty;

    [Ignore]
    [DataMember(Name = "formCreated")]
    public DateTime FormCreated { get; set; }

    [Ignore]
    [DataMember(Name = "fields")]
    public string Fields { get; set; } = string.Empty;

    [DataMember(Name = "hasAccess")]
    public bool HasAccess { get; set; }

    [Ignore]
    public FormSecurityType SecurityType { get; set; }

    [DataMember(Name = "allowInEditor")]
    public bool AllowInEditor { get; set; }

    [Column(Name = "SecurityType")]
    public int SecurityTypeInt { get; set; }
  }
}
