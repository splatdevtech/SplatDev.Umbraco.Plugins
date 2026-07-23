
// Type: Umbraco.Forms.Core.Models.FormEntity
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "form", Namespace = "")]
  [Serializable]
  public class FormEntity : EntityBase
  {
    [Required]
    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "createdBy")]
    public int? CreatedBy { get; set; }

    [DataMember(Name = "updatedBy")]
    public int? UpdatedBy { get; set; }

    [DataMember(Name = "pages")]
    public List<Page> Pages { get; set; } = new List<Page>();

    [DataMember(Name = "validationRules")]
    public List<ValidationRule> ValidationRules { get; set; } = new List<ValidationRule>();

    [DataMember(Name = "fieldIndicationType")]
    public FormFieldIndication FieldIndicationType { get; set; }

    [DataMember(Name = "indicator")]
    public string Indicator { get; set; } = string.Empty;

    [DataMember(Name = "showValidationSummary")]
    public bool ShowValidationSummary { get; set; }

    [DataMember(Name = "hideFieldValidation")]
    public bool HideFieldValidation { get; set; }

    [DataMember(Name = "requiredErrorMessage")]
    public string RequiredErrorMessage { get; set; } = string.Empty;

    [DataMember(Name = "invalidErrorMessage")]
    public string InvalidErrorMessage { get; set; } = string.Empty;

    [DataMember(Name = "messageOnSubmit")]
    public string? MessageOnSubmit { get; set; }

    [DataMember(Name = "messageOnSubmitIsHtml")]
    public bool MessageOnSubmitIsHtml { get; set; }

    [DataMember(Name = "goToPageOnSubmit")]
    [JsonConverter(typeof (JsonIntToStringConverter))]
    public string? GoToPageOnSubmit { get; set; }

    [DataMember(Name = "xPathOnSubmit")]
    public string? XPathOnSubmit { get; set; }

    [DataMember(Name = "manualApproval")]
    public bool ManualApproval { get; set; }

    [DataMember(Name = "storeRecordsLocally")]
    public bool StoreRecordsLocally { get; set; }

    [DataMember(Name = "autocompleteAttribute")]
    public string? AutocompleteAttribute { get; set; }

    [DataMember(Name = "displayDefaultFields")]
    public bool DisplayDefaultFields { get; set; } = true;

    [DataMember(Name = "selectedDisplayFields")]
    public List<RecordFieldDisplay> SelectedDisplayFields { get; set; } = new List<RecordFieldDisplay>();

    [DataMember(Name = "daysToRetainSubmittedRecordsFor")]
    public int DaysToRetainSubmittedRecordsFor { get; set; }

    [DataMember(Name = "daysToRetainApprovedRecordsFor")]
    public int DaysToRetainApprovedRecordsFor { get; set; }

    [DataMember(Name = "daysToRetainRejectedRecordsFor")]
    public int DaysToRetainRejectedRecordsFor { get; set; }

    [DataMember(Name = "cssClass")]
    public string? CssClass { get; set; }

    [DataMember(Name = "disableDefaultStylesheet")]
    public bool DisableDefaultStylesheet { get; set; }

    [DataMember(Name = "workflows")]
    public List<WorkflowEntity> Workflows { get; set; } = new List<WorkflowEntity>();

    [DataMember(Name = "datasource")]
    [JsonPropertyName("datasource")]
    public FormDataSourceDefinition? DataSource { get; set; }

    [DataMember(Name = "submitLabel")]
    public string? SubmitLabel { get; set; }

    [DataMember(Name = "nextLabel")]
    public string? NextLabel { get; set; }

    [DataMember(Name = "prevLabel")]
    public string? PrevLabel { get; set; }

    [DataMember(Name = "folderId")]
    public Guid? FolderId { get; set; }

    [DataMember(Name = "nodeId")]
    public int NodeId { get; set; }

    public IEnumerable<Field> AllFields() => this.Pages.SelectMany<Page, FieldSet>((Func<Page, IEnumerable<FieldSet>>) (x => (IEnumerable<FieldSet>) x.FieldSets)).SelectMany<FieldSet, FieldsetContainer>((Func<FieldSet, IEnumerable<FieldsetContainer>>) (x => (IEnumerable<FieldsetContainer>) x.Containers)).SelectMany<FieldsetContainer, Field>((Func<FieldsetContainer, IEnumerable<Field>>) (x => (IEnumerable<Field>) x.Fields));

    [DataMember(Name = "showPagingOnMultiPageForms")]
    public MultiPageNavigationOption ShowPagingOnMultiPageForms { get; set; }

    [DataMember(Name = "pagingDetailsFormat")]
    public string PagingDetailsFormat { get; set; } = "Page {0} of {1}";

    [DataMember(Name = "pageCaptionFormat")]
    public string PageCaptionFormat { get; set; } = "Page {0}";

    [DataMember(Name = "showSummaryPageOnMultiPageForms")]
    public bool ShowSummaryPageOnMultiPageForms { get; set; }

    [DataMember(Name = "summaryLabel")]
    public string? SummaryLabel { get; set; }
  }
}
