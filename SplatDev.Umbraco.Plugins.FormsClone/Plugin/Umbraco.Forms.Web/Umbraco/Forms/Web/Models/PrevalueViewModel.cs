
// Type: Umbraco.Forms.Web.Models.PrevalueViewModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;


#nullable enable
namespace Umbraco.Forms.Web.Models
{
  [Serializable]
  public class PrevalueViewModel
  {
    public string Id { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string? Caption { get; set; }
  }
}
