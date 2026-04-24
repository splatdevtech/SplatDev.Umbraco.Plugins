
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormEntryDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Umbraco.Forms.Core.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormEntryDto
  {
    [JsonPropertyName("values")]
    [JsonConverter(typeof (UmbracoFormsApiPostedValuesConverter))]
    public IDictionary<string, IList<string>> Values { get; set; } = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>();

    [JsonPropertyName("contentId")]
    public string? ContentId { get; set; }

    [JsonPropertyName("culture")]
    public string? Culture { get; set; }

    [JsonPropertyName("additionalData")]
    public IDictionary<string, string?>? AdditionalData { get; set; }
  }
}
