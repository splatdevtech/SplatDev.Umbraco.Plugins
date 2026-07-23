using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;
using FormBuilder.Core.Persistence.Interfaces;

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

namespace FormBuilder.Core.Persistence.Repositories
{
    internal sealed class WorkflowRepository(
      IScopeAccessor scopeAccessor,
      AppCaches cache,
      ILogger<WorkflowRepository> logger,
      IWorkflowFactory workflowFactory) :
      EntityRepositoryBase<Guid, WorkflowEntity>(scopeAccessor, cache, logger),
      IWorkflowRepository,
      IReadWriteQueryRepository<Guid, WorkflowEntity>,
      IReadRepository<Guid, WorkflowEntity>,
      IRepository,
      IWriteRepository<WorkflowEntity>,
      IQueryRepository<WorkflowEntity>
    {
        private readonly IWorkflowFactory _workflowFactory = workflowFactory;

        private static readonly Expression<Func<WorkflowDto, object?>>[] _slimFields =
        [
           x =>  x.Id,
           x =>  x.Key,
           x => x.Name,
           x =>  x.CreateDate,
           x =>  x.FormId
        ];

        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return (isCount ? sql.SelectCount(null) : sql.Select<FormDto>([])).From<WorkflowDto>(null);
        }

        protected override string GetBaseWhereClause() => "UFWorkflows.id = @id";

        protected override IEnumerable<string> GetDeleteClauses() => ["DELETE FROM FormBuilderWorkflows WHERE [key] = @key"];

        protected override WorkflowEntity? PerformGet(Guid id)
        {
            WorkflowDto dto = Database.FirstOrDefault<WorkflowDto>(SqlContext.Sql().Select<WorkflowDto>([]).From<WorkflowDto>(null).Where<WorkflowDto>(p => p.Key == id, null));
            return dto is not null ? _workflowFactory.BuildEntity(dto) : null;
        }

        protected override IEnumerable<WorkflowEntity> PerformGetAll(
          params Guid[]? ids)
        {
            if (ids is null || ids.Length == 0)
                return Database.Fetch<WorkflowDto>(NPocoSqlExtensions.Select(Sql(), Array.Empty<Expression<Func<WorkflowDto, object>>>()!).From<WorkflowDto>(null)).Select(new Func<WorkflowDto, WorkflowEntity>(_workflowFactory.BuildEntity));
            List<WorkflowEntity> all = [];
            foreach (IEnumerable<Guid> guids in ids.InGroupsOf(2000))
            {
                var sql = Sql().Select<WorkflowDto>([]).From<WorkflowDto>(null).WhereIn<WorkflowDto>(x => x.Key, guids);
                all.AddRange(Database.Fetch<WorkflowDto>(sql).Select(new Func<WorkflowDto, WorkflowEntity>(_workflowFactory.BuildEntity)));
            }
            return all;
        }

        protected override IEnumerable<WorkflowEntity> PerformGetByQuery(
          IQuery<WorkflowEntity> query)
        {
            return Database.Fetch<WorkflowDto>(new SqlTranslator<WorkflowEntity>(GetBaseQuery(false), query).Translate()).Select(new Func<WorkflowDto, WorkflowEntity>(_workflowFactory.BuildEntity));
        }

        protected override void PersistNewItem(WorkflowEntity entity)
        {
            entity.AddingEntity();
            WorkflowDto poco = _workflowFactory.BuildDto(entity);
            Database.Insert(poco);
            entity.Id = poco.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(WorkflowEntity entity)
        {
            entity.UpdatingEntity();
            Database.Update(_workflowFactory.BuildDto(entity));
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

        public IEnumerable<WorkflowEntity> GetFor(Form form) => Database.Fetch<WorkflowDto>(NPocoSqlExtensions.Select(Sql(), Array.Empty<Expression<Func<WorkflowDto, object>>>()!).From<WorkflowDto>(null).Where<WorkflowDto>(p => p.FormId == form.Id, null)).Select(new Func<WorkflowDto, WorkflowEntity>(_workflowFactory.BuildEntity));

        public IEnumerable<WorkflowEntitySlim> GetForSlim(Guid formId) => Database.Fetch<WorkflowDto>(NPocoSqlExtensions.Select(Sql(), _slimFields).From<WorkflowDto>(null).Where<WorkflowDto>(p => p.FormId == formId, null)).Select(new Func<WorkflowDto, WorkflowEntitySlim>(_workflowFactory.BuildEntitySlim));

        public IEnumerable<WorkflowEntitySlim> GetForSlim(Form form) => GetForSlim(form.Id);
    }
}