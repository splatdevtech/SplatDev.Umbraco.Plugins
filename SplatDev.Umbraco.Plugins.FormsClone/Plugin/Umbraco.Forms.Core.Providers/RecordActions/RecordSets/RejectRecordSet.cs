
// Type: Umbraco.Forms.Core.Providers.RecordActions.Recordsets.RejectRecordSet
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
  public class RejectRecordSet : RecordSetActionType
  {
    private readonly IRecordService _recordService;

    public RejectRecordSet(IRecordService recordService)
    {
      this._recordService = recordService;
      this.Description = "Rejects a set of records";
      this.Icon = "icon-wrong";
      this.Id = new Guid("84cd75a7-d3d9-4551-9c1a-3f478b4ec9ed");
      this.Name = "Reject";
      this.Alias = "reject";
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
          await this._recordService.RejectAsync(record, form).ConfigureAwait(false);
      }
      return RecordActionStatus.Completed;
    }
  }
}
