using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Results;
using FormBuilder.Core.Storage.Interfaces;

namespace FormBuilder.Core.Services
{
    internal sealed class ScheduledRecordDeletionService(IFormService formService, IRecordStorage recordStorage) : IScheduledRecordDeletionService
    {
        private readonly IFormService _formService = formService;
        private readonly IRecordStorage _recordStorage = recordStorage;

        public ScheduledRecordDeletionResult PerformScheduledRecordDeletion(
          DateTime now)
        {
            ScheduledRecordDeletionResult recordDeletionResult = new();
            foreach (Form form in _formService.Get())
            {
                int num1 = DeleteRecordsForForm(form, FormState.Submitted, now, form.DaysToRetainSubmittedRecordsFor);
                int num2 = DeleteRecordsForForm(form, FormState.Approved, now, form.DaysToRetainApprovedRecordsFor);
                int num3 = DeleteRecordsForForm(form, FormState.Rejected, now, form.DaysToRetainRejectedRecordsFor);
                if (num1 + num2 + num3 > 0)
                {
                    recordDeletionResult.TotalRecordCount += num1 + num2 + num3;
                    ++recordDeletionResult.TotalFormCount;
                }
            }
            return recordDeletionResult;
        }

        private int DeleteRecordsForForm(
          Form form,
          FormState formState,
          DateTime now,
          int daysToRetainRecordsFor)
        {
            if (daysToRetainRecordsFor <= 0)
                return 0;
            DateTime deleteRecordsCreatedEarlierThan = now.AddDays(-daysToRetainRecordsFor);
            return _recordStorage.DeleteFormRecords(form, formState, deleteRecordsCreatedEarlierThan);
        }
    }
}