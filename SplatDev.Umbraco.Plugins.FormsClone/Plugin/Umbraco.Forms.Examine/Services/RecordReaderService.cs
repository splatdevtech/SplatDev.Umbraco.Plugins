
// Type: Umbraco.Forms.Examine.Services.RecordReaderService
// Assembly: Umbraco.Forms.Examine, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF5A33E-94A1-42C9-B681-695454D27A51

using Examine;
using Examine.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Umbraco.Cms.Core.Models;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Examine.Indexes;


#nullable enable
namespace Umbraco.Forms.Examine.Services
{
  internal sealed class RecordReaderService : IRecordReaderService
  {
    private static readonly string s_approvedFormState = RecordValueSetBuilder.SanitizeXmlString(Enum.GetName(typeof (FormState), (object) FormState.Approved));
    private readonly ISearcher _formsIndexSearcher;

    public RecordReaderService(IExamineManager examineManager)
    {
      IIndex index;
      if (!examineManager.TryGetIndex("UmbracoFormsRecordsIndex", out index))
        throw new InvalidOperationException("Could not get Examine index: UmbracoFormsRecordsIndex");
      this._formsIndexSearcher = index.Searcher;
    }

    public PagedResult<Record> GetApprovedRecordsFromPage(
      int pageId,
      int pageNumber,
      int pageSize)
    {
      return this.GetRecordsFromBooleanOperation(this._formsIndexSearcher.CreateQuery().Field("State", RecordReaderService.s_approvedFormState).And().Field("UmbracoPageId", pageId.ToString()), pageNumber, pageSize);
    }

    public PagedResult<Record> GetApprovedRecordsFromFormOnPage(
      int pageId,
      Guid formId,
      int pageNumber,
      int pageSize)
    {
      return this.GetRecordsFromBooleanOperation(this._formsIndexSearcher.CreateQuery().Field("State", RecordReaderService.s_approvedFormState).And().Field("Form", formId.ToString()).And().Field("UmbracoPageId", pageId.ToString()), pageNumber, pageSize);
    }

    public PagedResult<Record> GetApprovedRecordsFromForm(
      Guid formId,
      int pageNumber,
      int pageSize)
    {
      return this.GetRecordsFromBooleanOperation(this._formsIndexSearcher.CreateQuery().Field("State", RecordReaderService.s_approvedFormState).And().Field("Form", formId.ToString()), pageNumber, pageSize);
    }

    public PagedResult<Record> GetRecordsFromPage(
      int pageId,
      int pageNumber,
      int pageSize)
    {
      return this.GetRecordsFromBooleanOperation(this._formsIndexSearcher.CreateQuery().Field("UmbracoPageId", pageId.ToString()), pageNumber, pageSize);
    }

    public PagedResult<Record> GetRecordsFromFormOnPage(
      int pageId,
      Guid formId,
      int pageNumber,
      int pageSize)
    {
      return this.GetRecordsFromBooleanOperation(this._formsIndexSearcher.CreateQuery().Field("Form", formId.ToString()).And().Field("UmbracoPageId", pageId.ToString()), pageNumber, pageSize);
    }

    public PagedResult<Record> GetRecordsFromForm(
      Guid formId,
      int pageNumber,
      int pageSize)
    {
      return this.GetRecordsFromBooleanOperation(this._formsIndexSearcher.CreateQuery().Field("Form", formId.ToString()), pageNumber, pageSize);
    }

    public PagedResult<Record> GetRecordsFromFormForMember(
      Guid formId,
      string memberKey,
      int pageNumber,
      int pageSize)
    {
      return this.GetRecordsFromBooleanOperation(this._formsIndexSearcher.CreateQuery().Field("Form", formId.ToString()).And().Field("MemberKey", memberKey), pageNumber, pageSize);
    }

    private PagedResult<Record> GetRecordsFromBooleanOperation(
      IBooleanOperation booleanOperation,
      int pageNumber,
      int pageSize)
    {
      int num = pageNumber - 1;
      ISearchResults source = booleanOperation.Execute(QueryOptions.SkipTake(Convert.ToInt32(pageSize * num), new int?(pageSize)));
      return new PagedResult<Record>(source.TotalItemCount, (long) pageNumber, (long) pageSize)
      {
        Items = (IEnumerable<Record>) source.Select<ISearchResult, Record>(new Func<ISearchResult, Record>(this.MapToRecord)).ToArray<Record>()
      };
    }

    private Record MapToRecord(ISearchResult item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      Record record = new Record()
      {
        Created = DateTime.Parse(item["Created"]),
        Updated = DateTime.Parse(item["Updated"]),
        Form = Guid.Parse(item["Form"]),
        Id = int.Parse(item.Id),
        State = (FormState) Enum.Parse(typeof (FormState), item["State"]),
        CurrentPage = Guid.Parse(item["CurrentPage"]),
        IP = item["Ip"],
        UniqueId = Guid.Parse(item["UniqueId"]),
        MemberKey = item["MemberKey"],
        RecordFields = JsonSerializer.Deserialize<Dictionary<Guid, RecordField>>(item["RecordFields"], FormsJsonSerializerOptions.Default) ?? new Dictionary<Guid, RecordField>(),
        UmbracoPageId = int.Parse(item["UmbracoPageId"])
      };
      record.RecordData = record.GenerateRecordDataAsJson();
      record.Culture = Thread.CurrentThread.CurrentCulture.Name;
      return record;
    }
  }
}
