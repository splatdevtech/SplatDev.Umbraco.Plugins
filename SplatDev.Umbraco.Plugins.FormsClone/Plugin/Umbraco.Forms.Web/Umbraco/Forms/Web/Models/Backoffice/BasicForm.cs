
// Type: Umbraco.Forms.Web.Models.Backoffice.BasicForm
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "form")]
  [Serializable]
  public class BasicForm
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "fields")]
    public string Fields { get; set; } = string.Empty;

    [DataMember(Name = "summary")]
    public string Summary { get; set; } = string.Empty;
  }
}
