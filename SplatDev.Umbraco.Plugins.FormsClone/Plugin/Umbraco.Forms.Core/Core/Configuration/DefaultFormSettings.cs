
// Type: Umbraco.Forms.Core.Configuration.DefaultFormSettings
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Configuration
{
  public class DefaultFormSettings
  {
    public bool ManualApproval { get; set; }

    public bool DisableStylesheet { get; set; }

    public FormFieldIndication MarkFieldsIndicator { get; set; }

    public string Indicator { get; set; } = "*";

    public string RequiredErrorMessage { get; set; } = "Please provide a value for {0}";

    public string InvalidErrorMessage { get; set; } = "Please provide a valid value for {0}";

    public bool ShowValidationSummary { get; set; }

    public bool HideFieldValidationLabels { get; set; }

    public string NextPageButtonLabel { get; set; } = "Next";

    public string PreviousPageButtonLabel { get; set; } = "Previous";

    public string SubmitButtonLabel { get; set; } = "Submit";

    public string MessageOnSubmit { get; set; } = "Thank you";

    public bool MessageOnSubmitIsHtml { get; set; }

    public bool StoreRecordsLocally { get; set; } = true;

    public string AutocompleteAttribute { get; set; } = string.Empty;

    public int DaysToRetainSubmittedRecordsFor { get; set; }

    public int DaysToRetainApprovedRecordsFor { get; set; }

    public int DaysToRetainRejectedRecordsFor { get; set; }

    public MultiPageNavigationOption ShowPagingOnMultiPageForms { get; set; }

    public string PagingDetailsFormat { get; set; } = "Page {0} of {1}";

    public string PageCaptionFormat { get; set; } = "Page {0}";

    public bool ShowSummaryPageOnMultiPageForms { get; set; }

    public string SummaryLabel { get; set; } = "Summary of Entry";
  }
}
