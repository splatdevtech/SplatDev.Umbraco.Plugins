
// Type: Umbraco.Forms.Core.Persistence.Repositories.Implement.WorkflowRepository
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;

using NPoco;

using System.Linq.Expressions;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence.Repositories.Implement;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Persistence.Factories;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Repositories.Implement
{
    internal sealed class WorkflowRepository :
      EntityRepositoryBase<Guid, WorkflowEntity>,
      IWorkflowRepository,
      IReadWriteQueryRepository<Guid, WorkflowEntity>,
      IReadRepository<Guid, WorkflowEntity>,
      IRepository,
      IWriteRepository<WorkflowEntity>,
      IQueryRepository<WorkflowEntity>
    {
        private readonly IWorkflowFactory _workflowFactory;
        private static readonly Expression<Func<WorkflowDto, object?>>[] _slimFields = new Expression<Func<WorkflowDto, object>>[5]
        {
       x =>  x.Id,
       x =>  x.Key,
       x => x.Name,
       x =>  x.CreateDate,
       x =>  x.FormId
        };

        public WorkflowRepository(
          IScopeAccessor scopeAccessor,
          AppCaches cache,
          ILogger<WorkflowRepository> logger,
          IWorkflowFactory workflowFactory)
          : base(scopeAccessor, cache, logger)
        {
            this._workflowFactory = workflowFactory;
        }

        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return NPocoSqlExtensions.From<WorkflowDto>(isCount ? NPocoSqlExtensions.SelectCount(sql, null) : NPocoSqlExtensions.Select<FormDto>(sql, Array.Empty<Expression<Func<FormDto, object>>>()), null);
        }

        protected override string GetBaseWhereClause() => "UFWorkflows.id = @id";

        protected override IEnumerable<string> GetDeleteClauses() => ["DELETE FROM UFWorkflows WHERE [key] = @key"];

        protected override WorkflowEntity? PerformGet(Guid id)
        {
            WorkflowDto dto = Database.FirstOrDefault<WorkflowDto>(NPocoSqlExtensions.From<WorkflowDto>(NPocoSqlExtensions.Select<WorkflowDto>(SqlContext.Sql(), Array.Empty<Expression<Func<WorkflowDto, object>>>()), null).Where<WorkflowDto>(p => p.Key == id, null));
            return dto != null ? this._workflowFactory.BuildEntity(dto) : null;
        }

        protected override IEnumerable<WorkflowEntity> PerformGetAll(
          params Guid[]? ids)
        {
            if (ids == null || ids.Length == 0)
                return Database.Fetch<WorkflowDto>(NPocoSqlExtensions.Select<WorkflowDto>(Sql(), Array.Empty<Expression<Func<WorkflowDto, object>>>()).From<WorkflowDto>(null)).Select<WorkflowDto, WorkflowEntity>(new Func<WorkflowDto, WorkflowEntity>(this._workflowFactory.BuildEntity));
            List<WorkflowEntity> all = new List<WorkflowEntity>();
            foreach (IEnumerable<Guid> guids in ids.InGroupsOf<Guid>(2000))
            {
                Sql<ISqlContext> sql = NPocoSqlExtensions.WhereIn<WorkflowDto>(NPocoSqlExtensions.From<WorkflowDto>(NPocoSqlExtensions.Select<WorkflowDto>(Sql(), Array.Empty<Expression<Func<WorkflowDto, object>>>()), null), x => x.Key, guids);
                all.AddRange(Database.Fetch<WorkflowDto>(sql).Select<WorkflowDto, WorkflowEntity>(new Func<WorkflowDto, WorkflowEntity>(this._workflowFactory.BuildEntity)));
            }
            return all;
        }

        protected override IEnumerable<WorkflowEntity> PerformGetByQuery(
          IQuery<WorkflowEntity> query)
        {
            return Database.Fetch<WorkflowDto>(new SqlTranslator<WorkflowEntity>(GetBaseQuery(false), query).Translate()).Select<WorkflowDto, WorkflowEntity>(new Func<WorkflowDto, WorkflowEntity>(this._workflowFactory.BuildEntity));
        }

        protected override void PersistNewItem(WorkflowEntity entity)
        {
            entity.AddingEntity();
            WorkflowDto poco = this._workflowFactory.BuildDto(entity);
            Database.Insert<WorkflowDto>(poco);
            entity.Id = poco.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(WorkflowEntity entity)
        {
            entity.UpdatingEntity();
            Database.Update(this._workflowFactory.BuildDto(entity));
            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(WorkflowEntity entity)
        {
            foreach (string deleteClause in GetDeleteClauses())
                Database.Execute(deleteClause, new
                {
                    key = entity.Key
                });
            entity.DeleteDate = new DateTime?(DateTime.Now);
        }

        public IEnumerable<WorkflowEntity> GetFor(Form form) => Database.Fetch<WorkflowDto>(NPocoSqlExtensions.From<WorkflowDto>(NPocoSqlExtensions.Select<WorkflowDto>(Sql(), Array.Empty<Expression<Func<WorkflowDto, object>>>()), null).Where<WorkflowDto>(p => p.FormId == form.Id, null)).Select<WorkflowDto, WorkflowEntity>(new Func<WorkflowDto, WorkflowEntity>(this._workflowFactory.BuildEntity));

        public IEnumerable<WorkflowEntitySlim> GetForSlim(Guid formId) => Database.Fetch<WorkflowDto>(NPocoSqlExtensions.From<WorkflowDto>(NPocoSqlExtensions.Select<WorkflowDto>(Sql(), WorkflowRepository._slimFields), null).Where<WorkflowDto>(p => p.FormId == formId, null)).Select<WorkflowDto, WorkflowEntitySlim>(new Func<WorkflowDto, WorkflowEntitySlim>(this._workflowFactory.BuildEntitySlim));

        public IEnumerable<WorkflowEntitySlim> GetForSlim(Form form) => this.GetForSlim(form.Id);
    }
}
