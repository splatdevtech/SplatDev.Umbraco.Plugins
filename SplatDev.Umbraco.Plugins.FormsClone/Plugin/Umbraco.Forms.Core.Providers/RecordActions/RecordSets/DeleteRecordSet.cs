
// Type: Umbraco.Forms.Core.Providers.RecordActions.RecordSets.DeleteRecordSet
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
namespace Umbraco.Forms.Core.Providers.RecordActions.RecordSets
{
  public class DeleteRecordSet : RecordSetActionType
  {
    private readonly IRecordService _recordService;

    public DeleteRecordSet(IRecordService recordService)
    {
      this._recordService = recordService;
      this.Description = "Deletes a set of records";
      this.Icon = "icon-trash";
      this.Id = new Guid("CB126B70-9011-11DF-A4EE-0800200C9A66");
      this.Name = "Delete";
      this.Alias = "delete";
    }

    public override bool NeedsConfirm => true;

    public override string ConfirmMessage => "@formRecordSetActions_deleteConfirm";

    public override List<Exception> ValidateSettings() => new List<Exception>();

    public override async Task<RecordActionStatus> ExecuteAsync(
      List<Record> records,
      Form form)
    {
      foreach (Record record in records)
        await this._recordService.DeleteAsync(record, form).ConfigureAwait(false);
      return RecordActionStatus.Completed;
    }
  }
}
