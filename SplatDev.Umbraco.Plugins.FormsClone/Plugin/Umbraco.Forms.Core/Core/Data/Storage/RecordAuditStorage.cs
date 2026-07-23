
// Type: Umbraco.Forms.Core.Data.Storage.RecordAuditStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using System.Data;

using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Persistence.Dtos;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Data.Storage
{
    internal sealed class RecordAuditStorage : IRecordAuditStorage
    {
        private readonly IScopeProvider _scopeProvider;

        public RecordAuditStorage(IScopeProvider scopeProvider) => this._scopeProvider = scopeProvider;

        public List<RecordAudit> GetRecordAuditTrail(int recordId)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                Sql<ISqlContext> sql = NPocoSqlExtensions.OrderByDescending<RecordAudit>(NPocoSqlExtensions.Where<RecordAudit>(scope.SqlContext.Sql(), x => x.Record == recordId, null), x => x.Id);
                List<RecordAudit> recordAuditTrail = scope.Database.Fetch<RecordAudit>(sql);
                scope.Complete();
                return recordAuditTrail;
            }
        }
    }
}
