namespace FormBuilder.Core.Configuration
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