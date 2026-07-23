
// Type: Umbraco.Forms.Web.Models.Backoffice.UpdateRecordResult
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "updateRecordResult")]
  [Serializable]
  public class UpdateRecordResult
  {
    [DataMember(Name = "success")]
    public bool Success { get; set; }

    [DataMember(Name = "fieldMessages")]
    public List<UpdateRecordResult.FieldMessage> FieldMessages { get; set; } = new List<UpdateRecordResult.FieldMessage>();

    [DataContract(Name = "fieldMessage")]
    public class FieldMessage
    {
      [DataMember(Name = "fieldId")]
      public Guid FieldId { get; set; }

      [DataMember(Name = "messages")]
      public List<string> Messages { get; set; } = new List<string>();
    }
  }
}
