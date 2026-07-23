
// Type: Umbraco.Forms.Core.Services.IRecordReaderService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public interface IRecordReaderService
  {
    PagedResult<Record> GetApprovedRecordsFromPage(
      int pageId,
      int pageNumber,
      int pageSize);

    PagedResult<Record> GetApprovedRecordsFromFormOnPage(
      int pageId,
      Guid formId,
      int pageNumber,
      int pageSize);

    PagedResult<Record> GetRecordsFromPage(
      int pageId,
      int pageNumber,
      int pageSize);

    PagedResult<Record> GetRecordsFromFormOnPage(
      int pageId,
      Guid formId,
      int pageNumber,
      int pageSize);

    PagedResult<Record> GetRecordsFromForm(
      Guid formId,
      int pageNumber,
      int pageSize);

    PagedResult<Record> GetApprovedRecordsFromForm(
      Guid formId,
      int pageNumber,
      int pageSize);

    PagedResult<Record> GetRecordsFromFormForMember(
      Guid formId,
      string memberKey,
      int pageNumber,
      int pageSize);
  }
}
