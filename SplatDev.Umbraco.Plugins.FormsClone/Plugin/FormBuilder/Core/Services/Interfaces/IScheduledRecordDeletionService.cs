using FormBuilder.Core.Services.Results;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IScheduledRecordDeletionService
    {
        ScheduledRecordDeletionResult PerformScheduledRecordDeletion(
          DateTime now);
    }
}