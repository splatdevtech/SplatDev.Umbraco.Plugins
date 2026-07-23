
// Type: Umbraco.Forms.Core.Providers.RecordActions.Recordsets.ApproveRecordSet
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.RecordActions.Recordsets
{
  public class ApproveRecordSet : RecordSetActionType
  {
    private readonly IRecordService _recordService;

    public ApproveRecordSet(IRecordService recordService)
    {
      this._recordService = recordService;
      this.Description = "Approves a set of records";
      this.Icon = "icon-check";
      this.Id = new Guid("CB126B79-9011-11DF-A4EE-0800200C9A66");
      this.Name = "Approve";
      this.Alias = "approve";
      this.IsAvailableForApprovedRecords = false;
    }

    public override List<Exception> ValidateSettings() => new List<Exception>();

    public override async Task<RecordActionStatus> ExecuteAsync(
      List<Record> records,
      Form form)
    {
      foreach (Record record in records)
      {
        if (form.Id == record.Form)
          await this._recordService.ApproveAsync(record, form).ConfigureAwait(false);
      }
      return RecordActionStatus.Completed;
    }
  }
}
