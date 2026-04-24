
// Type: Umbraco.Forms.Web.Models.Backoffice.Theme
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "theme")]
  [Serializable]
  public class Theme
  {
    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;
  }
}
