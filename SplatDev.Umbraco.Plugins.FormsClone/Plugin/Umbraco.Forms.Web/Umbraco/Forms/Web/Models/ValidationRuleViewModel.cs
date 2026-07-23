
// Type: Umbraco.Forms.Web.Models.ValidationRuleViewModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Newtonsoft.Json;
using System;


#nullable enable
namespace Umbraco.Forms.Web.Models
{
  [Serializable]
  public class ValidationRuleViewModel
  {
    [JsonProperty("rule")]
    public string Rule { get; set; } = string.Empty;

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;

    [JsonProperty("fieldId")]
    public Guid FieldId { get; set; }
  }
}
