
// Type: Umbraco.Forms.Core.Services.ScheduledRecordDeletionService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  internal sealed class ScheduledRecordDeletionService : IScheduledRecordDeletionService
  {
    private readonly IFormService _formService;
    private readonly IRecordStorage _recordStorage;

    public ScheduledRecordDeletionService(IFormService formService, IRecordStorage recordStorage)
    {
      this._formService = formService;
      this._recordStorage = recordStorage;
    }

    public ScheduledRecordDeletionResult PerformScheduledRecordDeletion(
      DateTime now)
    {
      ScheduledRecordDeletionResult recordDeletionResult = new ScheduledRecordDeletionResult();
      foreach (Form form in this._formService.Get())
      {
        int num1 = this.DeleteRecordsForForm(form, FormState.Submitted, now, form.DaysToRetainSubmittedRecordsFor);
        int num2 = this.DeleteRecordsForForm(form, FormState.Approved, now, form.DaysToRetainApprovedRecordsFor);
        int num3 = this.DeleteRecordsForForm(form, FormState.Rejected, now, form.DaysToRetainRejectedRecordsFor);
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
      DateTime deleteRecordsCreatedEarlierThan = now.AddDays((double) -daysToRetainRecordsFor);
      return this._recordStorage.DeleteFormRecords(form, formState, deleteRecordsCreatedEarlierThan);
    }
  }
}
