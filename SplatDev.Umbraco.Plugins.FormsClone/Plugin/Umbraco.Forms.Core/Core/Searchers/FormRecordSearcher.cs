
// Type: Umbraco.Forms.Core.Searchers.FormRecordSearcher
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;
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
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Searchers
{
    public sealed class FormRecordSearcher : IFormRecordSearcher
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IScopeProvider _scopeProvider;
        private readonly IMemberService _memberService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IFormService _formService;

        private readonly IContentService _contentService;
        private readonly IEventMessagesFactory _eventMessagesFactory;
        private readonly ILogger<FormRecordSearcher> _logger;
        private readonly PackageOptionSettings _packageOptionSettings;

        public FormRecordSearcher(
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          IFieldTypeStorage fieldTypeStorage,
          IScopeProvider scopeProvider,
          IMemberService memberService,
          ILocalizedTextService localizedTextService,
          IFormService formService,

          IContentService contentService,
          IEventMessagesFactory eventMessagesFactory,
          IOptions<PackageOptionSettings> packageOptionSettings,
          ILogger<FormRecordSearcher> logger)
        {
            this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            this._fieldTypeStorage = fieldTypeStorage;
            this._scopeProvider = scopeProvider;
            this._memberService = memberService;
            this._localizedTextService = localizedTextService;
            this._formService = formService;

            this._contentService = contentService;
            this._eventMessagesFactory = eventMessagesFactory;
            this._logger = logger;
            this._packageOptionSettings = packageOptionSettings.Value;
        }

        public EntrySearchResultCollection QueryDataBase(
          Guid formId,
          RecordFilter filter)
        {
            return this.QueryDataBaseForSubmissions(formId, filter);
        }

        public EntrySearchResultMetadata QueryDataBaseForMetadata(
          Guid formId,
          RecordFilter filter)
        {
            return this.QueryDataBaseForSubmissionsMetadata(formId, filter);
        }

        public int? GetPageNumberForRecord(Guid formId, RecordFilter filter)
        {
            if (filter.RecordId == Guid.Empty)
                return null;
            if (!FormRecordSearcher.ValidSortProvided(filter))
                return null;
            FormRecordSearcher.EnsureFilterDefaults(filter);
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
            {
                string str = scope.SqlContext.IsSqlite() ? "CAST((RowNumber / @0) as int) + ((RowNumber / @0) > CAST((RowNumber / @0) as int))" : "CEILING(RowNumber / @0)";
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(364, 3);
                interpolatedStringHandler.AppendLiteral("\n                    SELECT ");
                interpolatedStringHandler.AppendFormatted(str);
                interpolatedStringHandler.AppendLiteral(" + 1 PageNumber\n                    FROM (\n                        SELECT ROW_NUMBER() OVER (ORDER BY ");
                interpolatedStringHandler.AppendFormatted(filter.SortBy);
                interpolatedStringHandler.AppendLiteral(" ");
                interpolatedStringHandler.AppendFormatted(filter.SortOrder == RecordSorting.Descending ? "DESC" : "ASC");
                interpolatedStringHandler.AppendLiteral(") RowNumber, UniqueId\n                        FROM UFRecords\n                        WHERE Form = @1\n                        AND Created >= @2 AND Created <= @3\n                    ) RowNumbers\n                    WHERE UniqueId = @4");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                Sql<ISqlContext> sql = scope.SqlContext.Sql(stringAndClear, new object[5]
                {
           filter.Take,
           formId,
           filter.StartDate,
           filter.EndDate,
           filter.RecordId
                });
                return new int?(scope.Database.ExecuteScalar<int>(sql));
            }
        }

        private EntrySearchResultCollection QueryDataBaseForSubmissions(
          Guid formId,
          RecordFilter filter,
          bool enforceSensitiveData = false)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                IUser currentUser = this._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
                bool flag1 = currentUser != null && currentUser.HasAccessToSensitiveData();
                Form form = this._formService.Get(formId);
                FormRecordSearcher.EnsureFilterDefaults(filter);
                Sql sql = this.BuildSqlQuery(scope, formId, filter);
                EntrySearchResultCollection resultCollection = new EntrySearchResultCollection();
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
                IList<FormRecordSearcher.RecordWorkflowAuditSummary> workflowSummaries = this.GetWorkflowSummaries(scope, records);
                List<EntrySearchResult> results = new List<EntrySearchResult>();
                if (form != null)
                {
                    List<EntrySearchResultSchema> searchResultSchemaList = new List<EntrySearchResultSchema>();
                    foreach (Field allField in form.AllFields)
                    {
                        FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(allField);
                        if (fieldTypeByField == null)
                        {
                            EntrySearchResultSchema searchResultSchema = new EntrySearchResultSchema()
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
                            EntrySearchResultSchema searchResultSchema = new EntrySearchResultSchema()
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
                int? indexOfPageName = null;
                foreach (Record record in records)
                {
                    EntrySearchResult entrySearchResult = new EntrySearchResult()
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
                    FormRecordSearcher.ApplyWorkflowSummaryToEntry(entrySearchResult, record, workflowSummaries);
                    if (!string.IsNullOrEmpty(record.MemberKey))
                    {
                        Guid result1;
                        int result2;
                        IMember member = !Guid.TryParse(record.MemberKey, out result1) ? (!int.TryParse(record.MemberKey, NumberStyles.Integer, CultureInfo.InvariantCulture, out result2) ? this._memberService.GetByProviderKey(record.MemberKey) : this._memberService.GetById(result2)) : this._memberService.GetByKey(result1);
                        if (member != null)
                            entrySearchResult.Member = new EntrySearchResult.MemberData()
                            {
                                Name = member.Name ?? string.Empty,
                                Email = member.Email,
                                Unique = member.Key
                            };
                    }
                    if (form != null && !string.IsNullOrEmpty(record.RecordData))
                    {
                        Dictionary<string, string> formfields = record.DeserializeRecordData(_logger);
                        List<EntrySearchResult.FieldData> fieldDataList = new List<EntrySearchResult.FieldData>();
                        string str = this._localizedTextService.Localize("content", "isSensitiveValue", CultureInfo.CurrentCulture);
                        foreach (EntrySearchResultSchema schemaItem in resultCollection.Schema.Take<EntrySearchResultSchema>(resultCollection.Schema.Count<EntrySearchResultSchema>() - 8))
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
                                        Value = this.ParseValue(formfields, record.Culture, schemaItem)
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
                    EventMessages messages = this._eventMessagesFactory.Get();
                    EntrySearchResultFetchingNotification fetchingNotification = new EntrySearchResultFetchingNotification(entrySearchResult, messages);
                    scope.Notifications.Publish(fetchingNotification);
                    results.Add(entrySearchResult);
                }
                this.PopulateUmbracoPageNames(results, indexOfPageName);
                FormRecordSearcher.HideMemberIfAllRecordsAreAnonymous(results, resultCollection.Schema);
                resultCollection.Results = results;
                scope.Complete();
                return resultCollection;
            }
        }

        private IList<FormRecordSearcher.RecordWorkflowAuditSummary> GetWorkflowSummaries(
          IScope scope,
          List<Record> records)
        {
            if (!records.Any<Record>())
                return new List<FormRecordSearcher.RecordWorkflowAuditSummary>();
            List<RecordWorkflowAudit> auditEntries = new List<RecordWorkflowAudit>();
            List<Guid> list1 = records.Select<Record, Guid>(x => x.UniqueId).ToList<Guid>();
            foreach (IEnumerable<Guid> source in list1.InGroupsOf<Guid>(1000))
            {
                string sql = "WHERE RecordUniqueId IN (" + string.Join(", ", source.Select<Guid, string>((x, i) => "@" + i.ToString())) + ")";
                List<RecordWorkflowAudit> collection = scope.Database.Fetch<RecordWorkflowAudit>(sql, source.Cast<object>().ToArray<object>());
                auditEntries.AddRange(collection);
            }
            return list1.Select<Guid, FormRecordSearcher.RecordWorkflowAuditSummary>(x =>
            {
                List<RecordWorkflowAudit> list2 = auditEntries.Where<RecordWorkflowAudit>(y => y.RecordUniqueId == x).ToList<RecordWorkflowAudit>();
                return new FormRecordSearcher.RecordWorkflowAuditSummary()
                {
                    RecordUniqueId = x,
                    NumberOfWorkflowsExecuted = list2.Select<RecordWorkflowAudit, Guid>(y => y.WorkflowKey).Distinct<Guid>().Count<Guid>(),
                    NumberOfWorkflowsCompleted = list2.Where<RecordWorkflowAudit>(y => y.ExecutionStatus == 3).Select<RecordWorkflowAudit, Guid>(y => y.WorkflowKey).Distinct<Guid>().Count<Guid>()
                };
            }).ToList<FormRecordSearcher.RecordWorkflowAuditSummary>();
        }

        private static void ApplyWorkflowSummaryToEntry(
          EntrySearchResult entry,
          Record record,
          IEnumerable<FormRecordSearcher.RecordWorkflowAuditSummary> workflowSummaries)
        {
            FormRecordSearcher.RecordWorkflowAuditSummary workflowAuditSummary = workflowSummaries.SingleOrDefault<FormRecordSearcher.RecordWorkflowAuditSummary>(x => x.RecordUniqueId == record.UniqueId);
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
                string datesForBackOffice = this._packageOptionSettings.CultureToUseWhenParsingDatesForBackOffice;
                CultureInfo provider = string.IsNullOrEmpty(serializedWithCulture) ? (string.IsNullOrEmpty(datesForBackOffice) ? Thread.CurrentThread.CurrentCulture : new CultureInfo(datesForBackOffice)) : new CultureInfo(serializedWithCulture);
                DateTime result;
                if (DateTime.TryParse(formfield, provider, DateTimeStyles.None, out result))
                    return result;
            }
            return formfield;
        }

        private Sql BuildSqlQuery(
          IScope ambientScope,
          Guid formId,
          RecordFilter model,
          string select = "")
        {
            FormRecordSearcher.EnsureFilterDefaults(model);
            Sql<ISqlContext> sql1 = ambientScope.SqlContext.Sql();
            if (!string.IsNullOrEmpty(select))
                sql1 = sql1.Select(select);
            Sql<ISqlContext> sql2 = NPocoSqlExtensions.From<Record>(sql1, null);
            sql2.Where("Created >= @0 AND Created <= @1", model.StartDate, model.EndDate.AddDays(1.0));
            Sql<ISqlContext> sql3 = sql2.Where("Form = @0", formId.ToString().ToUpperInvariant());
            if (!string.IsNullOrWhiteSpace(model.MemberKey))
                sql3 = sql3.Where("MemberKey = @0", model.MemberKey);
            if (!string.IsNullOrWhiteSpace(model.Filter))
                sql3 = sql3.Where("RecordData LIKE @0", "%" + model.Filter + "%");
            if (model.States.Any<FormState>())
                sql3 = sql3.Where("State in (@states)", new
                {
                    states = model.States.Select<FormState, string>((Func<FormState, string>)(state => state.ToString())).ToList<string>()
                });
            if (!string.IsNullOrEmpty(model.SortBy) && FormRecordSearcher.ValidSortProvided(model))
            {
                if (model.SortOrder == RecordSorting.Descending)
                    sql3 = NPocoSqlExtensions.OrderByDescending(sql3, new string[1]
                    {
            model.SortBy
                    });
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
            if (recordFilter.States != null)
                return;
            recordFilter.States = new List<FormState>()
      {
        FormState.Approved,
        FormState.Rejected,
        FormState.Submitted
      };
        }

        private static bool ValidSortProvided(RecordFilter model) => (new string[3]
        {
      "created",
      "updated",
      "state"
        }).Contains<string>(model.SortBy.ToLowerInvariant());

        private void PopulateUmbracoPageNames(
          IEnumerable<EntrySearchResult> results,
          int? indexOfPageName)
        {
            List<IContent> list = this._contentService.GetByIds(results.Where<EntrySearchResult>(x => x.UmbracoPage != null).Select<EntrySearchResult, int>(x => x.UmbracoPage.Id).Distinct<int>().ToList<int>()).ToList<IContent>();
            foreach (EntrySearchResult result1 in results)
            {
                EntrySearchResult result = result1;
                if (result.UmbracoPage != null)
                {
                    IContent content = list.FirstOrDefault<IContent>(x => x.Id == result.UmbracoPage.Id);
                    if (content != null)
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
            if (results.Any<EntrySearchResult>(x => x.Member != null))
                return;
            schema.Single<EntrySearchResultSchema>(x => x.Name == "Member").ShowOnListingScreen = false;
        }

        private EntrySearchResultMetadata QueryDataBaseForSubmissionsMetadata(
          Guid formId,
          RecordFilter model)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                Sql sql = this.BuildSqlQuery(scope, formId, model, "count(*) As Count,max(created) As LastSubmittedDate");
                Dictionary<string, object> dictionary = scope.Database.SingleOrDefault<Dictionary<string, object>>(sql);
                EntrySearchResultMetadata searchResultMetadata = new EntrySearchResultMetadata();
                searchResultMetadata.Count = Convert.ToInt32(dictionary["Count"]);
                searchResultMetadata.LastSubmittedDate = this.ParseLastSubmittedDate(dictionary["LastSubmittedDate"]);
                scope.Complete();
                return searchResultMetadata;
            }
        }

        private string ParseLastSubmittedDate(object? value)
        {
            value ??= string.Empty;
            return string.IsNullOrEmpty(value?.ToString()) ? string.Empty : value.ToString().Substring(0, value.ToString().IndexOf(".")) ?? string.Empty;
        }

        private class RecordWorkflowAuditSummary
        {
            public Guid RecordUniqueId { get; set; }

            public int NumberOfWorkflowsExecuted { get; set; }

            public int NumberOfWorkflowsCompleted { get; set; }
        }
    }
}
