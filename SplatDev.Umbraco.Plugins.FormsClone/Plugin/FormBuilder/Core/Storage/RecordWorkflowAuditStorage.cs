using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Storage.Interfaces;

using NPoco;

using System.Data;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Scoping;

using Umbraco.Cms.Infrastructure.Persistence;

using Umbraco.Extensions;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal class RecordWorkflowAuditStorage(IScopeProvider scopeProvider) : IRecordWorkflowAuditStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public List<RecordWorkflowAudit> GetRecordWorkflowAuditTrail(
          Guid recordId)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            Sql<ISqlContext> sql = scope.SqlContext.Sql().Where<RecordWorkflowAudit>(x => x.RecordUniqueId == recordId, null).OrderByDescending((Expression<Func<RecordWorkflowAudit, object?>>)(x => x.Id));
            List<RecordWorkflowAudit> workflowAuditTrail = scope.Database.Fetch<RecordWorkflowAudit>(sql);
            ((ICoreScope)scope).Complete();
            return workflowAuditTrail;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}