
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormFieldDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Umbraco.Forms.Core.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormFieldDto
  {
    public Guid Id { get; set; }

    public string Caption { get; set; } = string.Empty;

    public string? HelpText { get; set; }

    public string? CssClass { get; set; }

    public string Alias { get; set; } = string.Empty;

    public bool Required { get; set; }

    public string? RequiredErrorMessage { get; set; }

    public string? Pattern { get; set; }

    public string? PatternInvalidErrorMessage { get; set; }

    public FormConditionDto? Condition { get; set; }

    public FormFileUploadOptionsDto? FileUploadOptions { get; set; }

    public IEnumerable<FormFieldPrevalueDto> PreValues { get; set; } = Enumerable.Empty<FormFieldPrevalueDto>();

    [JsonConverter(typeof (UmbracoFormsApiSettingsConverter))]
    public IDictionary<string, string> Settings { get; set; } = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public FormFieldTypeDto Type { get; set; } = new FormFieldTypeDto();
  }
}
