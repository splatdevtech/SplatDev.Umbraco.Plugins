
// Type: Umbraco.Forms.Web.Models.Backoffice.RecordActionExecution
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "recordActionExecution")]
  [Serializable]
  public class RecordActionExecution
  {
    [DataMember(Name = "recordKeys")]
    public IEnumerable<Guid> RecordKeys { get; set; } = Enumerable.Empty<Guid>();
  }
}
