
// Type: Umbraco.Forms.Core.Configuration.PackageOptionSettings
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389


#nullable enable
namespace Umbraco.Forms.Core.Configuration
{
  public class PackageOptionSettings
  {
    public string IgnoreWorkFlowsOnEdit { get; set; } = "True";

    public bool AllowEditableFormSubmissions { get; set; }

    public bool AppendQueryStringOnRedirectAfterFormSubmission { get; set; }

    public string CultureToUseWhenParsingDatesForBackOffice { get; set; } = string.Empty;

    public string TriggerConditionsCheckOn { get; set; } = "change";

    public ScheduledRecordDeletionSettings ScheduledRecordDeletion { get; set; } = new ScheduledRecordDeletionSettings();

    public bool DisableRecordIndexing { get; set; }

    public bool EnableFormsApi { get; set; }

    public bool EnableRecordingOfIpWithFormSubmission { get; set; }

    public bool DisableClientSideValidationDependencyCheck { get; set; }

    public bool DisableRelationTracking { get; set; }

    public TrackRenderedFormsStorageMethodOption TrackRenderedFormsStorageMethod { get; set; } = TrackRenderedFormsStorageMethodOption.HttpContextItems;

    public bool EnableMultiPageFormSettings { get; set; } = true;

    public bool EnableAdvancedValidationRules { get; set; }
  }
}
