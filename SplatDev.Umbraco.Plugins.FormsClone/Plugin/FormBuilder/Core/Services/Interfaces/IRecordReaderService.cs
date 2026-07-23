using FormBuilder.Core.Persistence.Fields;

using Umbraco.Cms.Core.Models;

namespace FormBuilder.Core.Services.Interfaces
{
#pragma warning disable CS0618 // Type or member is obsolete

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

#pragma warning restore CS0618 // Type or member is obsolete
}