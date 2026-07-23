
// Type: Umbraco.Forms.Core.Data.Storage.IRecordFieldValueStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Data.Storage
{
  public interface IRecordFieldValueStorage
  {
    List<object> GetRecordFieldValues(RecordField recordFieldInForm);

    List<object> InsertRecordFieldValues(RecordField recordFieldInForm);

    bool DeleteRecordFieldValues(RecordField recordFieldInForm);

    bool DeleteRecordFieldValues(IEnumerable<RecordField> recordFieldInForms);

    void DeleteAllRecordFieldValues(Record record);

    void DeleteAllRecordFieldValues(IList<int> recordIds);

    void DeleteAllRecordAuditValues(Record record);

    void DeleteAllRecordAuditValues(IList<int> recordIds);
  }
}
