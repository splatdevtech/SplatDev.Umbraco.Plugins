namespace FormBuilder.Core.Services.Results
{
    public class ScheduledRecordDeletionResult
    {
        public bool HasDeletedRecords => TotalRecordCount > 0;

        public int TotalRecordCount { get; set; }

        public int TotalFormCount { get; set; }
    }
}