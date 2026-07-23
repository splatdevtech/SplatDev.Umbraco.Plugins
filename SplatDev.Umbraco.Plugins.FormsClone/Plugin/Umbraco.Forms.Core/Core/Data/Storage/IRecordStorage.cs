
// Type: Umbraco.Forms.Core.Data.Storage.IRecordStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Data.Storage
{
  public interface IRecordStorage
  {
    List<Record> GetAllRecords(Form form, bool includeFields = true);

    List<Record> GetRecords(IEnumerable<Guid> keys, Form form, bool includeFields = true);

    Record? GetRecord(int id, Form form);

    Record? GetRecordByUniqueId(Guid uniqueId, Form form);

    Record InsertRecord(Record record, Form form);

    void DeleteFormRecords(Form form);

    int DeleteFormRecords(Form form, FormState formState, DateTime deleteRecordsCreatedEarlierThan);

    void DeleteRecord(Record record, Form form);

    Record UpdateRecord(Record record, Form form);

    Record UpdateRecord(Record record, Form form, int? userId);

    int GetRecordCount(Form form) => this.GetAllRecords(form, false).Count;
  }
}
