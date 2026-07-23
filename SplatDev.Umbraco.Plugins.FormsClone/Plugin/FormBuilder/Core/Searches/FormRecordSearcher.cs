using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.Extensions.Options;

using NPoco;

using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;

namespace FormBuilder.Core.Searches
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class FormRecordSearcher(
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IFieldTypeStorage fieldTypeStorage,
      IScopeProvider scopeProvider,
      IMemberService memberService,
      ILocalizedTextService localizedTextService,
      IFormService formService,
      IContentService contentService,
      IEventMessagesFactory eventMessagesFactory,
      IOptions<PackageOptionSettings> packageOptionSettings) : IFormRecordSearcher
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly IScopeProvider _scopeProvider = scopeProvider;
        private readonly IMemberService _memberService = memberService;
        private readonly ILocalizedTextService _localizedTextService = localizedTextService;
        private readonly IFormService _formService = formService;
        private readonly IContentService _contentService = contentService;
        private readonly IEventMessagesFactory _eventMessagesFactory = eventMessagesFactory;
        private readonly PackageOptionSettings _packageOptionSettings = packageOptionSettings.Value;

        internal static readonly string[] sourceArray =
        [
          "created",
          "updated",
          "state"
        ];

        public EntrySearchResultCollection QueryDataBase(
          Guid formId,
          RecordFilter filter)
        {
            return QueryDataBaseForSubmissions(formId, filter);
        }

        public EntrySearchResultMetadata QueryDataBaseForMetadata(
          Guid formId,
          RecordFilter filter)
        {
            return QueryDataBaseForSubmissionsMetadata(formId, filter);
        }

        public int? GetPageNumberForRecord(Guid formId, RecordFilter filter)
        {
            if (filter.RecordId == Guid.Empty)
                return new int?();
            if (!ValidSortProvided(filter))
                return new int?();
            EnsureFilterDefaults(filter);
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            string str = scope.SqlContext.IsSqlite() ? "CAST((RowNumber / @0) as int) + ((RowNumber / @0) > CAST((RowNumber / @0) as int))" : "CEILING(RowNumber / @0)";
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(364, 3);
            interpolatedStringHandler.AppendLiteral("\n                    SELECT ");
            interpolatedStringHandler.AppendFormatted(str);
            interpolatedStringHandler.AppendLiteral(" + 1 PageNumber\n                    FROM (\n                        SELECT ROW_NUMBER() OVER (ORDER BY ");
            interpolatedStringHandler.AppendFormatted(filter.SortBy);
            interpolatedStringHandler.AppendLiteral(" ");
            interpolatedStringHandler.AppendFormatted(filter.SortOrder == RecordSorting.Descending ? "DESC" : "ASC");
            interpolatedStringHandler.AppendLiteral(") RowNumber, UniqueId\n                        FROM FormBuilderRecords\n                        WHERE Form = @1\n                        AND Created >= @2 AND Created <= @3\n                    ) RowNumbers\n                    WHERE UniqueId = @4");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            Sql<ISqlContext> sql = scope.SqlContext.Sql(stringAndClear,
            [
               filter.Take,
               formId,
               filter.StartDate,
               filter.EndDate,
               filter.RecordId
            ]);
            return new int?(scope.Database.ExecuteScalar<int>(sql));
        }

        private EntrySearchResultCollection QueryDataBaseForSubmissions(
          Guid formId,
          RecordFilter filter,
          bool enforceSensitiveData = false)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            IUser? currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            bool flag1 = currentUser is not null && currentUser.HasAccessToSensitiveData();
            Form? form = _formService.Get(formId);
            EnsureFilterDefaults(filter);
            Sql sql = BuildSqlQuery(scope, formId, filter);
            EntrySearchResultCollection resultCollection = new();
            List<Record> records;
            if (filter.Skip == 0 && filter.Take == int.MaxValue)
            {
                records = scope.Database.Fetch<Record>(sql);
                resultCollection.TotalNumberOfResults = records.Count;
                resultCollection.TotalNumberOfPages = 1L;
            }
            else
            {
                long page1 = filter.Skip / filter.Take + 1;
                long take = filter.Take;
                Page<Record> page2 = scope.Database.Page<Record>(page1, take, sql);
                records = page2.Items;
                resultCollection.TotalNumberOfResults = page2.TotalItems;
                resultCollection.TotalNumberOfPages = page2.TotalPages;
            }
            IList<RecordWorkflowAuditSummary> workflowSummaries = GetWorkflowSummaries(scope, records);
            List<EntrySearchResult> results = [];
            if (form is not null)
            {
                List<EntrySearchResultSchema> searchResultSchemaList = [];
                foreach (Field allField in form.AllFields)
                {
                    FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(allField);
                    if (fieldTypeByField == null)
                    {
                        EntrySearchResultSchema searchResultSchema = new()
                        {
                            Alias = allField.Alias,
                            Name = allField.Caption,
                            Id = allField.Id.ToString(),
                            View = "text"
                        };
                        searchResultSchemaList.Add(searchResultSchema);
                    }
                    else if (fieldTypeByField.StoresData)
                    {
                        EntrySearchResultSchema searchResultSchema = new()
                        {
                            Alias = allField.Alias,
                            Name = allField.Caption,
                            Id = allField.Id.ToString(),
                            View = fieldTypeByField.RenderView,
                            EditView = fieldTypeByField.EditView
                        };
                        if (allField.ContainsSensitiveData)
                            searchResultSchema.ContainsSensitiveData = true;
                        searchResultSchemaList.Add(searchResultSchema);
                    }
                }
                searchResultSchemaList.Add(new EntrySearchResultSchema()
                {
                    Id = "member",
                    Alias = "member",
                    View = "member",
                    Name = "Member",
                    ContainsSensitiveData = true
                });
                searchResultSchemaList.Add(new EntrySearchResultSchema()
                {
                    Id = "state",
                    Alias = "state",
                    View = "text",
                    Name = "State"
                });
                searchResultSchemaList.Add(new EntrySearchResultSchema()
                {
                    Id = "created",
                    Alias = "created",
                    View = "datetime",
                    Name = "Created"
                });
                searchResultSchemaList.Add(new EntrySearchResultSchema()
                {
                    Id = "updated",
                    Alias = "updated",
                    View = "datetime",
                    Name = "Updated"
                });
                searchResultSchemaList.Add(new EntrySearchResultSchema()
                {
                    Id = "recordId",
                    Alias = "recordId",
                    View = "number",
                    Name = "Entry Id",
                    ShowOnListingScreen = false
                });
                searchResultSchemaList.Add(new EntrySearchResultSchema()
                {
                    Id = "umbracoPageId",
                    Alias = "umbracoPageId",
                    View = "number",
                    Name = "Page Id",
                    ShowOnListingScreen = false
                });
                searchResultSchemaList.Add(new EntrySearchResultSchema()
                {
                    Id = "umbracoPageName",
                    Alias = "umbracoPageName",
                    View = "text",
                    Name = "Page Name",
                    ShowOnListingScreen = false
                });
                searchResultSchemaList.Add(new EntrySearchResultSchema()
                {
                    Id = "workflowAuditSummary",
                    Alias = "workflowAuditSummary",
                    View = "workflowSummary",
                    Name = "Workflows"
                });
                resultCollection.Schema = searchResultSchemaList;
            }
            int? indexOfPageName = new int?();
            foreach (Record record in records)
            {
                EntrySearchResult entrySearchResult = new()
                {
                    Id = record.Id,
                    Form = record.Form.ToString(),
                    State = record.StateAsString,
                    Culture = record.Culture
                };
                int num = -filter.LocalTimeOffset - Convert.ToInt32(DateTimeOffset.Now.Offset.TotalMinutes);
                entrySearchResult.Created = record.Created.AddMinutes(num);
                entrySearchResult.Updated = record.Updated.AddMinutes(num);
                entrySearchResult.UniqueId = record.UniqueId;
                entrySearchResult.UmbracoPage = new UmbracoPageDetail()
                {
                    Id = record.UmbracoPageId
                };
                ApplyWorkflowSummaryToEntry(entrySearchResult, record, workflowSummaries);
                if (!string.IsNullOrEmpty(record.MemberKey))
                {
                    IMember? member = !Guid.TryParse(record.MemberKey, out Guid result1) ? !int.TryParse(record.MemberKey, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result2) ? _memberService.GetByProviderKey(record.MemberKey) : _memberService.GetById(result2) : _memberService.GetByKey(result1);
                    if (member is not null)
                        entrySearchResult.Member = new EntrySearchResult.MemberData()
                        {
                            Name = member.Name ?? string.Empty,
                            Email = member.Email,
                            Unique = member.Key
                        };
                }
                if (form is not null && !string.IsNullOrEmpty(record.RecordData))
                {
                    Dictionary<string, string> formfields = record.DeserializeRecordData();
                    List<EntrySearchResult.FieldData> fieldDataList = [];
                    string str = _localizedTextService.Localize("content", "isSensitiveValue", CultureInfo.CurrentCulture);
                    foreach (EntrySearchResultSchema schemaItem in resultCollection.Schema.Take(resultCollection.Schema.Count() - 8))
                    {
                        if (formfields.ContainsKey(schemaItem.Id))
                        {
                            if (!flag1 && schemaItem.ContainsSensitiveData || enforceSensitiveData && schemaItem.ContainsSensitiveData)
                                fieldDataList.Add(new EntrySearchResult.FieldData()
                                {
                                    FieldId = schemaItem.Id,
                                    Value = str
                                });
                            else
                                fieldDataList.Add(new EntrySearchResult.FieldData()
                                {
                                    FieldId = schemaItem.Id,
                                    Value = ParseValue(formfields, record.Culture, schemaItem)
                                });
                        }
                        else
                            fieldDataList.Add(new EntrySearchResult.FieldData()
                            {
                                FieldId = schemaItem.Id,
                                Value = string.Empty
                            });
                    }
                    fieldDataList.Add(new EntrySearchResult.FieldData()
                    {
                        FieldId = "member",
                        Value = entrySearchResult.Member
                    });
                    fieldDataList.Add(new EntrySearchResult.FieldData()
                    {
                        FieldId = "state",
                        Value = entrySearchResult.State
                    });
                    fieldDataList.Add(new EntrySearchResult.FieldData()
                    {
                        FieldId = "created",
                        Value = entrySearchResult.Created
                    });
                    fieldDataList.Add(new EntrySearchResult.FieldData()
                    {
                        FieldId = "updated",
                        Value = entrySearchResult.Updated
                    });
                    fieldDataList.Add(new EntrySearchResult.FieldData()
                    {
                        FieldId = "recordId",
                        Value = entrySearchResult.Id
                    });
                    fieldDataList.Add(new EntrySearchResult.FieldData()
                    {
                        FieldId = "umbracoPageId",
                        Value = entrySearchResult.UmbracoPage.Id
                    });
                    fieldDataList.Add(new EntrySearchResult.FieldData()
                    {
                        FieldId = "umbracoPageName",
                        Value = entrySearchResult.UmbracoPage.Name
                    });
                    fieldDataList.Add(new EntrySearchResult.FieldData()
                    {
                        FieldId = "workflowAuditSummary",
                        Value = entrySearchResult.NumberOfWorkflowsCompleted.ToString() + "/" + entrySearchResult.NumberOfWorkflowsExecuted.ToString()
                    });
                    if (!indexOfPageName.HasValue)
                        indexOfPageName = new int?(fieldDataList.Count - 2);
                    entrySearchResult.Fields = fieldDataList;
                }
                EventMessages messages = _eventMessagesFactory.Get();
                EntrySearchResultFetchingNotification fetchingNotification = new(entrySearchResult, messages);
                ((ICoreScope)scope).Notifications.Publish(fetchingNotification);
                results.Add(entrySearchResult);
            }
            PopulateUmbracoPageNames(results, indexOfPageName);
            HideMemberIfAllRecordsAreAnonymous(results, resultCollection.Schema);
            resultCollection.Results = results;
            ((ICoreScope)scope).Complete();
            return resultCollection;
        }

        private static IList<RecordWorkflowAuditSummary> GetWorkflowSummaries(
          IScope scope,
          List<Record> records)
        {
            if (records.Count == 0)
                return [];
            List<RecordWorkflowAudit> auditEntries = [];
            List<Guid> list1 = [.. records.Select(x => x.UniqueId)];
            foreach (IEnumerable<Guid> source in list1.InGroupsOf(1000))
            {
                string sql = "WHERE RecordUniqueId IN (" + string.Join(", ", source.Select((x, i) => "@" + i.ToString())) + ")";
                List<RecordWorkflowAudit> collection = scope.Database.Fetch<RecordWorkflowAudit>(sql, [.. source.Cast<object>()]);
                auditEntries.AddRange(collection);
            }
            return [.. list1.Select(x =>
            {
                List<RecordWorkflowAudit> list2 = [.. auditEntries.Where(y => y.RecordUniqueId == x)];
                return new RecordWorkflowAuditSummary()
                {
                    RecordUniqueId = x,
                    NumberOfWorkflowsExecuted = list2.Select(y => y.WorkflowKey).Distinct().Count(),
                    NumberOfWorkflowsCompleted = list2.Where(y => y.ExecutionStatus == 3).Select(y => y.WorkflowKey).Distinct().Count()
                };
            })];
        }

        private static void ApplyWorkflowSummaryToEntry(
          EntrySearchResult entry,
          Record record,
          IEnumerable<RecordWorkflowAuditSummary> workflowSummaries)
        {
            RecordWorkflowAuditSummary? workflowAuditSummary = workflowSummaries.SingleOrDefault(x => x.RecordUniqueId == record.UniqueId);
            if (workflowAuditSummary == null)
                return;
            entry.NumberOfWorkflowsCompleted = workflowAuditSummary.NumberOfWorkflowsCompleted;
            entry.NumberOfWorkflowsExecuted = workflowAuditSummary.NumberOfWorkflowsExecuted;
        }

        private object ParseValue(
          Dictionary<string, string> formfields,
          string serializedWithCulture,
          EntrySearchResultSchema schemaItem)
        {
            string formfield = formfields[schemaItem.Id];
            if (schemaItem.View == "date")
            {
                string datesForBackOffice = _packageOptionSettings.CultureToUseWhenParsingDatesForBackOffice;
                CultureInfo provider = string.IsNullOrEmpty(serializedWithCulture) ? string.IsNullOrEmpty(datesForBackOffice) ? Thread.CurrentThread.CurrentCulture : new CultureInfo(datesForBackOffice) : new CultureInfo(serializedWithCulture);
                if (DateTime.TryParse(formfield, provider, DateTimeStyles.None, out DateTime result))
                    return result;
            }
            return formfield;
        }

        private static Sql<ISqlContext> BuildSqlQuery(
          IScope ambientScope,
          Guid formId,
          RecordFilter model,
          string select = "")
        {
            EnsureFilterDefaults(model);
            Sql<ISqlContext> sql1 = ambientScope.SqlContext.Sql();
            if (!string.IsNullOrEmpty(select))
                sql1 = sql1.Select(select);
            Sql<ISqlContext> sql2 = sql1.From<Record>(null);
            sql2.Where("Created >= @0 AND Created <= @1", model.StartDate, model.EndDate.AddDays(1.0));
            Sql<ISqlContext> sql3 = sql2.Where("Form = @0", formId.ToString().ToUpperInvariant());
            if (!string.IsNullOrWhiteSpace(model.MemberKey))
                sql3 = sql3.Where("MemberKey = @0", model.MemberKey);
            if (!string.IsNullOrWhiteSpace(model.Filter))
                sql3 = sql3.Where("RecordData LIKE @0", "%" + model.Filter + "%");
            if (model.States.Count != 0)
                sql3 = sql3.Where("State in (@states)", new
                {
                    states = model.States.Select((Func<FormState, string>)(state => state.ToString())).ToList()
                });
            if (!string.IsNullOrEmpty(model.SortBy) && ValidSortProvided(model))
            {
                if (model.SortOrder == RecordSorting.Descending)
                    sql3 = sql3.OrderByDescending(
                    [
                        model.SortBy
                    ]);
                else
                    sql3 = sql3.OrderBy(model.SortBy);
            }
            return sql3;
        }

        private static void EnsureFilterDefaults(RecordFilter model)
        {
            if (model.Skip == 0)
                model.Skip = 0;
            if (model.Take == 0)
                model.Take = 20;
            if (model.StartDate == new DateTime())
                model.StartDate = DateTime.Now.Subtract(TimeSpan.FromDays(2000));
            if (model.EndDate == new DateTime())
                model.EndDate = DateTime.Now;
            RecordFilter recordFilter = model;
            if (recordFilter.States is not null)
                return;
            recordFilter.States =
              [
                FormState.Approved,
                FormState.Rejected,
                FormState.Submitted
              ];
        }

        private static bool ValidSortProvided(RecordFilter model) => sourceArray.Contains(model.SortBy.ToLowerInvariant());

        private void PopulateUmbracoPageNames(
          IEnumerable<EntrySearchResult> results,
          int? indexOfPageName)
        {
            List<IContent> list = [.. _contentService.GetByIds(results.Where(x => x.UmbracoPage is not null).Select(x => x.UmbracoPage!.Id).Distinct().ToList())];
            foreach (EntrySearchResult result1 in results)
            {
                EntrySearchResult result = result1;
                if (result.UmbracoPage is not null)
                {
                    IContent? content = list.FirstOrDefault(x => x.Id == result.UmbracoPage.Id);
                    if (content is not null)
                    {
                        result.UmbracoPage.Name = content.Name ?? string.Empty;
                        result.UmbracoPage.Unique = content.Key;
                        if (indexOfPageName.HasValue)
                            ((List<EntrySearchResult.FieldData>)result.Fields)[indexOfPageName.Value].Value = content.Name ?? string.Empty;
                    }
                }
            }
        }

        private static void HideMemberIfAllRecordsAreAnonymous(
          List<EntrySearchResult> results,
          IEnumerable<EntrySearchResultSchema> schema)
        {
            if (results.Any(x => x.Member is not null))
                return;
            schema.Single(x => x.Name == "Member").ShowOnListingScreen = false;
        }

        private EntrySearchResultMetadata QueryDataBaseForSubmissionsMetadata(
          Guid formId,
          RecordFilter model)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            Sql sql = BuildSqlQuery(scope, formId, model, "count(*) As Count,max(created) As LastSubmittedDate");
            Dictionary<string, object> dictionary = scope.Database.SingleOrDefault<Dictionary<string, object>>(sql);
            EntrySearchResultMetadata searchResultMetadata = new()
            {
                Count = Convert.ToInt32(dictionary["Count"]),
                LastSubmittedDate = ParseLastSubmittedDate(dictionary["LastSubmittedDate"])
            };
            ((ICoreScope)scope).Complete();
            return searchResultMetadata;
        }

        private static string ParseLastSubmittedDate(object? value)
        {
            if (value is not null && value is DateTime nullable)
            {
                DateTime local = nullable;
                return local.ToString("D");
            }
            return string.IsNullOrEmpty(value?.ToString()) ? string.Empty : value?.ToString()?[..value.ToString()!.IndexOf('.')] ?? string.Empty;
        }

        private class RecordWorkflowAuditSummary
        {
            public Guid RecordUniqueId { get; set; }

            public int NumberOfWorkflowsExecuted { get; set; }

            public int NumberOfWorkflowsCompleted { get; set; }
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}