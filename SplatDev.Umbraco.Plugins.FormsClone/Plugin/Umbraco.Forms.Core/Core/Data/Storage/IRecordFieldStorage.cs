
// Type: Umbraco.Forms.Core.Data.Storage.IRecordFieldStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Data.Storage
{
  public interface IRecordFieldStorage
  {
    Dictionary<Guid, RecordField> GetAllRecordFields(
      Record record,
      Form form);

    Dictionary<Guid, RecordField> GetAllRecordFields(
      IEnumerable<Record> records,
      Form form);

    RecordField? GetRecordField(Guid key);

    IEnumerable<RecordField> InsertRecordFields(
      IEnumerable<RecordField> recordfields);

    RecordField InsertRecordField(RecordField recordfield);

    bool DeleteRecordField(RecordField recordField);

    RecordField UpdateRecordField(RecordField recordField);

    IEnumerable<RecordField> UpdateRecordFields(
      IEnumerable<RecordField> recordFields);
  }
}
