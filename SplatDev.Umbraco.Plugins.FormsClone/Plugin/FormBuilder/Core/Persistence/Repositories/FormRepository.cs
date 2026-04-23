using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;
using FormBuilder.Core.Persistence.Interfaces;

using Microsoft.Extensions.Logging;

using NPoco;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Cms.Infrastructure.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence.Repositories.Implement;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace FormBuilder.Core.Persistence.Repositories
{
    internal sealed class FormRepository(
      IScopeAccessor scopeAccessor,
      AppCaches cache,
      ILogger<FormRepository> logger,
      IFormFactory formFactory) :
      EntityRepositoryBase<Guid, FormEntity>(scopeAccessor, cache, logger),
      IFormRepository,
      IReadWriteQueryRepository<Guid, FormEntity>,
      IReadRepository<Guid, FormEntity>,
      IRepository,
      IWriteRepository<FormEntity>,
      IQueryRepository<FormEntity>
    {
        private readonly IFormFactory _formFactory = formFactory;

        public IEnumerable<FormEntitySlim> GetManySlim() => GetFormEntitiesSlim();

        [ExcludeFromCodeCoverage]
        public IEnumerable<FormEntity> GetAtRoot() => GetFormEntities(x => x.FolderKey == new Guid?());

        [ExcludeFromCodeCoverage]
        public IEnumerable<FormEntitySlim> GetAtRootSlim() => GetFormEntitiesSlim(x => x.FolderKey == new Guid?());

        [ExcludeFromCodeCoverage]
        public FormEntitySlim? GetSlim(Guid key) => GetFormEntitiesSlim(x => x.Key == key).SingleOrDefault();

        [ExcludeFromCodeCoverage]
        public IEnumerable<FormEntity> GetFromFolder(Guid parentFolderId) => GetFormEntities(x => x.FolderKey == (Guid?)parentFolderId);

        [ExcludeFromCodeCoverage]
        public IEnumerable<FormEntitySlim> GetFromFolderSlim(
          Guid parentFolderId)
        {
            return GetFormEntitiesSlim(x => x.FolderKey == (Guid?)parentFolderId);
        }

        private IEnumerable<FormEntity> GetFormEntities(
          Expression<Func<FormDto, bool>>? wherePredicate = null) => [.. FetchFormDtos(wherePredicate).Select(new Func<FormDto, FormEntity>(_formFactory.BuildEntity))];

        private IEnumerable<FormEntitySlim> GetFormEntitiesSlim(
          Expression<Func<FormDto, bool>>? wherePredicate = null)
        {
            Expression<Func<FormDto, object>>[] expressionArray =
            [
                 (x =>  x.Id),
                 (x =>  x.Name),
                 (x =>  x.Key),
                 (x =>  x.CreateDate),
                 (x =>  x.FolderKey!)
            ];
            return [.. FetchFormDtos(wherePredicate, expressionArray!).Select(new Func<FormDto, FormEntitySlim>(_formFactory.BuildEntitySlim))];
        }

        private List<FormDto> FetchFormDtos(
          Expression<Func<FormDto, bool>>? wherePredicate = null,
          params Expression<Func<FormDto, object?>>[] fields)
        {
            Sql<ISqlContext> sql = NPocoSqlExtensions.Select(Sql(), fields).From<FormDto>(null);
            if (wherePredicate is not null)
                sql = sql.Where(wherePredicate, null);
            return Database.Fetch<FormDto>(sql.OrderBy<FormDto>(x => x.Name));
        }

        [ExcludeFromCodeCoverage]
        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return (isCount ? sql.SelectCount(null) : sql.Select<FormDto>([])).From<FormDto>(null);
        }

        [ExcludeFromCodeCoverage]
        protected override string GetBaseWhereClause() => "UFForms.id = @id";

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<string> GetDeleteClauses() =>
        [
            "DELETE FROM FormBuilderUserFormSecurity WHERE Form = @key",
            "DELETE FROM FormBuilderUserGroupFormSecurity WHERE Form = @key",
            "DELETE FROM FormBuilderForms WHERE [key] = @key",
            "DELETE FROM umbracoRelation WHERE parentid = (SELECT id FROM umbracoNode WHERE uniqueid = @key) OR childId = (SELECT id FROM umbracoNode WHERE uniqueid = @key)",
            "DELETE FROM umbracoNode WHERE uniqueid = @key"
        ];

        [ExcludeFromCodeCoverage]
        protected override FormEntity? PerformGet(Guid id)
        {
            FormDto dto = Database.FirstOrDefault<FormDto>(SqlContext.Sql().Select<FormDto>([]).From<FormDto>(null).Where<FormDto>(p => p.Key == id, null));
            return dto is not null ? _formFactory.BuildEntity(dto) : null;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<FormEntity> PerformGetAll(params Guid[]? ids) => ids is null || ids.Length == 0 ? Database.Fetch<FormDto>(Sql().Select<FormDto>([]).From<FormDto>(null)).Select(new Func<FormDto, FormEntity>(_formFactory.BuildEntity)) : Database.FetchByGroups<FormDto, Guid>(ids, 2000, batch => NPocoSqlExtensions.Select(Sql(), Array.Empty<Expression<Func<FormDto, object>>>()!).From<FormDto>(null).WhereIn(((Expression<Func<FormDto, object>>)(x => x.Key))!, batch)).Select(new Func<FormDto, FormEntity>(_formFactory.BuildEntity));

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<FormEntity> PerformGetByQuery(
          IQuery<FormEntity> query)
        {
            return Database.Fetch<FormDto>(new SqlTranslator<FormEntity>(GetBaseQuery(false), query).Translate()).Select(new Func<FormDto, FormEntity>(_formFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistNewItem(FormEntity entity)
        {
            NodeDto poco1 = _formFactory.BuildNodeDto(entity);
            Database.Insert(poco1);
            entity.NodeId = poco1.NodeId;
            entity.AddingEntity();
            FormDto poco2 = _formFactory.BuildDto(entity);
            Database.Insert(poco2);
            entity.Id = poco2.Id;
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistUpdatedItem(FormEntity entity)
        {
            EnsureNodeId(entity);
            entity.UpdatingEntity();
            Database.Update(_formFactory.BuildDto(entity));
            NodeDto? relatedNodeId = GetRelatedNodeId(entity);
            if (relatedNodeId is not null && relatedNodeId.Text != entity.Name)
            {
                relatedNodeId.Text = entity.Name;
                Database.Update(relatedNodeId);
            }
            entity.ResetDirtyProperties();
        }

        private void EnsureNodeId(FormEntity entity)
        {
            int? nullable = Database.FirstOrDefault<int?>(NPocoSqlExtensions.Select(Sql(),
            [
                 ((Expression<Func<FormDto, object>>) (x =>  x.NodeId))!
            ]).From<FormDto>(null).Where<FormDto>(p => p.Key == entity.Key, null));
            if (!nullable.HasValue || nullable.Value == entity.NodeId)
                return;
            entity.NodeId = nullable.Value;
        }

        private NodeDto? GetRelatedNodeId(FormEntity entity)
        {
            if (entity == null) return null;

            var sql = SqlContext.Sql()
                .SelectAll()
                .From<NodeDto>()
                .Where<NodeDto>(n => n.NodeId == entity.NodeId);

            return Database.FirstOrDefault<NodeDto>(sql);
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistDeletedItem(FormEntity entity)
        {
            foreach (string deleteClause in GetDeleteClauses())
                Database.Execute(deleteClause, new
                {
                    key = entity.Key
                });
            entity.DeleteDate = new DateTime?(DateTime.Now);
            Database.Execute("DELETE FROM umbracoNode WHERE id = @id", new
            {
                id = entity.NodeId
            });
        }
    }
}