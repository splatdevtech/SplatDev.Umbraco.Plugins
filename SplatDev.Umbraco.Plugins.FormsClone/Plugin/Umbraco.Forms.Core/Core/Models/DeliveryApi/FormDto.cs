
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormDto : IPostSubmissionDetail
  {
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Indicator { get; set; } = string.Empty;

    public string? CssClass { get; set; }

    public string? NextLabel { get; set; }

    public string? PreviousLabel { get; set; }

    public string? SubmitLabel { get; set; }

    public bool DisableDefaultStylesheet { get; set; }

    public FormFieldIndication FieldIndicationType { get; set; }

    public bool HideFieldValidation { get; set; }

    public string? MessageOnSubmit { get; set; }

    public bool MessageOnSubmitIsHtml { get; set; }

    public bool ShowValidationSummary { get; set; }

    public Guid? GotoPageOnSubmit { get; set; }

    public IApiContentRoute? GotoPageOnSubmitRoute { get; set; }

    public IEnumerable<FormPageDto> Pages { get; set; } = (IEnumerable<FormPageDto>) new List<FormPageDto>();

    public IEnumerable<FormValidationRuleDto> ValidationRules { get; set; } = (IEnumerable<FormValidationRuleDto>) new List<FormValidationRuleDto>();
  }
}
