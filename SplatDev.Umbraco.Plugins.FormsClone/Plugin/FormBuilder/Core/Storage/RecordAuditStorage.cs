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

    internal sealed class RecordAuditStorage(IScopeProvider scopeProvider) : IRecordAuditStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public List<RecordAudit> GetRecordAuditTrail(int recordId)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            Sql<ISqlContext> sql = scope.SqlContext.Sql().Where<RecordAudit>(x => x.Record == recordId, null).OrderByDescending((Expression<Func<RecordAudit, object?>>)(x => x.Id));
            List<RecordAudit> recordAuditTrail = scope.Database.Fetch<RecordAudit>(sql);
            ((ICoreScope)scope).Complete();
            return recordAuditTrail;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}