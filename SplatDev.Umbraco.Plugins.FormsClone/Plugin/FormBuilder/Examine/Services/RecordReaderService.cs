using Examine;
using Examine.Search;

using FormBuilder.Core.Enums;
using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Examine.ValueSetBuilders;

using System.Text.Json;

using Umbraco.Cms.Core.Models;

namespace FormBuilder.Examine.Services
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class RecordReaderService : IRecordReaderService
    {
        private static readonly string s_approvedFormState = RecordValueSetBuilder.SanitizeXmlString(Enum.GetName(FormState.Approved)!);
        private readonly ISearcher _formsIndexSearcher;

        public RecordReaderService(IExamineManager examineManager)
        {
            if (!examineManager.TryGetIndex("FormBuildersRecordsIndex", out IIndex index))
                throw new InvalidOperationException("Could not get Examine index: FormBuildersRecordsIndex");
            _formsIndexSearcher = index.Searcher;
        }

        public PagedResult<Record> GetApprovedRecordsFromPage(
          int pageId,
          int pageNumber,
          int pageSize)
        {
            return GetRecordsFromBooleanOperation(_formsIndexSearcher.CreateQuery().Field("State", s_approvedFormState).And().Field("UmbracoPageId", pageId.ToString()), pageNumber, pageSize);
        }

        public PagedResult<Record> GetApprovedRecordsFromFormOnPage(
          int pageId,
          Guid formId,
          int pageNumber,
          int pageSize)
        {
            return GetRecordsFromBooleanOperation(_formsIndexSearcher.CreateQuery().Field("State", s_approvedFormState).And().Field("Form", formId.ToString()).And().Field("UmbracoPageId", pageId.ToString()), pageNumber, pageSize);
        }

        public PagedResult<Record> GetApprovedRecordsFromForm(
          Guid formId,
          int pageNumber,
          int pageSize)
        {
            return GetRecordsFromBooleanOperation(_formsIndexSearcher.CreateQuery().Field("State", s_approvedFormState).And().Field("Form", formId.ToString()), pageNumber, pageSize);
        }

        public PagedResult<Record> GetRecordsFromPage(
          int pageId,
          int pageNumber,
          int pageSize)
        {
            return GetRecordsFromBooleanOperation(_formsIndexSearcher.CreateQuery().Field("UmbracoPageId", pageId.ToString()), pageNumber, pageSize);
        }

        public PagedResult<Record> GetRecordsFromFormOnPage(
          int pageId,
          Guid formId,
          int pageNumber,
          int pageSize)
        {
            return GetRecordsFromBooleanOperation(_formsIndexSearcher.CreateQuery().Field("Form", formId.ToString()).And().Field("UmbracoPageId", pageId.ToString()), pageNumber, pageSize);
        }

        public PagedResult<Record> GetRecordsFromForm(
          Guid formId,
          int pageNumber,
          int pageSize)
        {
            return GetRecordsFromBooleanOperation(_formsIndexSearcher.CreateQuery().Field("Form", formId.ToString()), pageNumber, pageSize);
        }

        public PagedResult<Record> GetRecordsFromFormForMember(
          Guid formId,
          string memberKey,
          int pageNumber,
          int pageSize)
        {
            return GetRecordsFromBooleanOperation(_formsIndexSearcher.CreateQuery().Field("Form", formId.ToString()).And().Field("MemberKey", memberKey), pageNumber, pageSize);
        }

        private PagedResult<Record> GetRecordsFromBooleanOperation(
          IBooleanOperation booleanOperation,
          int pageNumber,
          int pageSize)
        {
            int num = pageNumber - 1;
            ISearchResults source = booleanOperation.Execute(QueryOptions.SkipTake(Convert.ToInt32(pageSize * num), new int?(pageSize)));
            return new PagedResult<Record>(source.TotalItemCount, pageNumber, pageSize)
            {
                Items = [.. source.Select(new Func<ISearchResult, Record>(MapToRecord))]
            };
        }

        private Record MapToRecord(ISearchResult item)
        {
            ArgumentNullException.ThrowIfNull(item);
            Record record = new()
            {
                Created = DateTime.Parse(item["Created"]),
                Updated = DateTime.Parse(item["Updated"]),
                Form = Guid.Parse(item["Form"]),
                Id = int.Parse(item.Id),
                State = Enum.Parse<FormState>(item["State"]),
                CurrentPage = Guid.Parse(item["CurrentPage"]),
                IP = item["Ip"],
                UniqueId = Guid.Parse(item["UniqueId"]),
                MemberKey = item["MemberKey"],
                RecordFields = JsonSerializer.Deserialize<Dictionary<Guid, RecordField>>(item["RecordFields"], FormsJsonSerializerOptions.Default) ?? [],
                UmbracoPageId = int.Parse(item["UmbracoPageId"])
            };
            record.RecordData = record.GenerateRecordDataAsJson();
            record.Culture = Thread.CurrentThread.CurrentCulture.Name;
            return record;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}