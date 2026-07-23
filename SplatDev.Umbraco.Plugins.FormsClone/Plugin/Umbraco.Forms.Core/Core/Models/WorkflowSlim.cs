
// Type: Umbraco.Forms.Core.Models.WorkflowSlim
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Runtime.Serialization;

namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "workflow", Namespace = "")]
  [Serializable]
  public class WorkflowSlim : BaseSlim
  {
    [DataMember(Name = "form")]
    public Guid Form { get; set; }
  }
}
