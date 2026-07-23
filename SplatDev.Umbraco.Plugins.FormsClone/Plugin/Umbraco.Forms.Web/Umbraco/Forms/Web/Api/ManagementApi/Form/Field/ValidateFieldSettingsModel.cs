
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.Field.ValidateFieldSettingsModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System.Collections.Generic;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form.Field
{
  public class ValidateFieldSettingsModel
  {
    public string Caption { get; set; } = string.Empty;

    public string Alias { get; set; } = string.Empty;

    public IDictionary<string, string> Settings { get; set; } = (IDictionary<string, string>) new Dictionary<string, string>();

    public IEnumerable<AllowedUploadType>? AllowedUploadTypes { get; set; }
  }
}
