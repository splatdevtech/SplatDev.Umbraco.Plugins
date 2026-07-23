
// Type: Umbraco.Forms.Core.Searchers.IFormRecordSearcher
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Searchers
{
  public interface IFormRecordSearcher
  {
    EntrySearchResultCollection QueryDataBase(
      Guid formId,
      RecordFilter filter);

    EntrySearchResultMetadata QueryDataBaseForMetadata(
      Guid formId,
      RecordFilter fllter);

    int? GetPageNumberForRecord(Guid formId, RecordFilter filter);
  }
}
