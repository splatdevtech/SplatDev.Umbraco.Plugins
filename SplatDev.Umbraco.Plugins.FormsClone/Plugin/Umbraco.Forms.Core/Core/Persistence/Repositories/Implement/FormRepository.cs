
// Type: Umbraco.Forms.Core.Persistence.Repositories.Implement.FormRepository
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

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
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Persistence.Factories;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Repositories.Implement
{
    internal sealed class FormRepository :
      EntityRepositoryBase<Guid, FormEntity>,
      IFormRepository,
      IReadWriteQueryRepository<Guid, FormEntity>,
      IReadRepository<Guid, FormEntity>,
      IRepository,
      IWriteRepository<FormEntity>,
      IQueryRepository<FormEntity>
    {
        private readonly IFormFactory _formFactory;

        public FormRepository(
          IScopeAccessor scopeAccessor,
          AppCaches cache,
          ILogger<FormRepository> logger,
          IFormFactory formFactory)
          : base(scopeAccessor, cache, logger)
        {
            this._formFactory = formFactory;
        }

        public IEnumerable<FormEntitySlim> GetManySlim() => this.GetFormEntitiesSlim();

        [ExcludeFromCodeCoverage]
        public IEnumerable<FormEntity> GetAtRoot() => this.GetFormEntities(x => x.FolderKey == null);

        [ExcludeFromCodeCoverage]
        public IEnumerable<FormEntitySlim> GetAtRootSlim() => this.GetFormEntitiesSlim(x => x.FolderKey == null);

        [ExcludeFromCodeCoverage]
        public FormEntitySlim? GetSlim(Guid key) => this.GetFormEntitiesSlim(x => x.Key == key).SingleOrDefault<FormEntitySlim>();

        [ExcludeFromCodeCoverage]
        public IEnumerable<FormEntity> GetFromFolder(Guid parentFolderId) => this.GetFormEntities(x => x.FolderKey == (Guid?)parentFolderId);

        [ExcludeFromCodeCoverage]
        public IEnumerable<FormEntitySlim> GetFromFolderSlim(
          Guid parentFolderId)
        {
            return this.GetFormEntitiesSlim(x => x.FolderKey == (Guid?)parentFolderId);
        }

        private IEnumerable<FormEntity> GetFormEntities(
          Expression<Func<FormDto, bool>>? wherePredicate = null)
        {
            return this.FetchFormDtos(wherePredicate).Select<FormDto, FormEntity>(new Func<FormDto, FormEntity>(this._formFactory.BuildEntity)).ToList<FormEntity>();
        }

        private IEnumerable<FormEntitySlim> GetFormEntitiesSlim(
          Expression<Func<FormDto, bool>>? wherePredicate = null)
        {
            Expression<Func<FormDto, object>>[] expressionArray = new Expression<Func<FormDto, object>>[5]
            {
         x =>  x.Id,
         x => x.Name,
         x =>  x.Key,
         x =>  x.CreateDate,
         x =>  x.FolderKey
            };
            var entities = this.FetchFormDtos(wherePredicate, expressionArray).Select<FormDto, FormEntitySlim>(new Func<FormDto, FormEntitySlim>(this._formFactory.BuildEntitySlim)).ToList<FormEntitySlim>();
            return entities;
        }

        private IEnumerable<FormDto> FetchFormDtos(
          Expression<Func<FormDto, bool>>? wherePredicate = null,
          params Expression<Func<FormDto, object?>>[] fields)
        {
            Sql<ISqlContext> sql = NPocoSqlExtensions.From<FormDto>(NPocoSqlExtensions.Select<FormDto>(Sql(), fields), null);
            if (wherePredicate != null)
                sql = NPocoSqlExtensions.Where<FormDto>(sql, wherePredicate, null);
            return Database.Fetch<FormDto>(sql.OrderBy<FormDto>(x => x.Name));
        }

        [ExcludeFromCodeCoverage]
        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return NPocoSqlExtensions.From<FormDto>(isCount ? NPocoSqlExtensions.SelectCount(sql, null) : NPocoSqlExtensions.Select<FormDto>(sql, Array.Empty<Expression<Func<FormDto, object>>>()), null);
        }

        [ExcludeFromCodeCoverage]
        protected override string GetBaseWhereClause() => "UFForms.id = @id";

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<string> GetDeleteClauses() => new List<string>()
    {
      "DELETE FROM UFUserFormSecurity WHERE Form = @key",
      "DELETE FROM UFUserGroupFormSecurity WHERE Form = @key",
      "DELETE FROM UFForms WHERE [key] = @key",
      "DELETE FROM umbracoRelation WHERE parentid = (SELECT id FROM umbracoNode WHERE uniqueid = @key) OR childId = (SELECT id FROM umbracoNode WHERE uniqueid = @key)",
      "DELETE FROM umbracoNode WHERE uniqueid = @key"
    };

        [ExcludeFromCodeCoverage]
        protected override FormEntity? PerformGet(Guid id)
        {
            FormDto dto = Database.FirstOrDefault<FormDto>(NPocoSqlExtensions.From<FormDto>(NPocoSqlExtensions.Select<FormDto>(SqlContext.Sql(), Array.Empty<Expression<Func<FormDto, object>>>()), null).Where<FormDto>(p => p.Key == id, null));
            return dto != null ? this._formFactory.BuildEntity(dto) : null;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<FormEntity> PerformGetAll(params Guid[]? ids) => ids == null || ids.Length == 0 ? Database.Fetch<FormDto>(NPocoSqlExtensions.Select<FormDto>(Sql(), Array.Empty<Expression<Func<FormDto, object>>>()).From<FormDto>(null)).Select<FormDto, FormEntity>(new Func<FormDto, FormEntity>(this._formFactory.BuildEntity)) : NPocoDatabaseExtensions.FetchByGroups<FormDto, Guid>(Database, ids, 2000, batch => NPocoSqlExtensions.WhereIn<FormDto>(NPocoSqlExtensions.From<FormDto>(NPocoSqlExtensions.Select<FormDto>(Sql(), Array.Empty<Expression<Func<FormDto, object>>>()), null), x => x.Key, batch)).Select<FormDto, FormEntity>(new Func<FormDto, FormEntity>(this._formFactory.BuildEntity));

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<FormEntity> PerformGetByQuery(
          IQuery<FormEntity> query)
        {
            return Database.Fetch<FormDto>(new SqlTranslator<FormEntity>(GetBaseQuery(false), query).Translate()).Select<FormDto, FormEntity>(new Func<FormDto, FormEntity>(this._formFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistNewItem(FormEntity entity)
        {
            NodeDto poco1 = this._formFactory.BuildNodeDto(entity);
            Database.Insert<NodeDto>(poco1);
            entity.NodeId = poco1.NodeId;
            entity.AddingEntity();
            FormDto poco2 = this._formFactory.BuildDto(entity);
            Database.Insert<FormDto>(poco2);
            entity.Id = poco2.Id;
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistUpdatedItem(FormEntity entity)
        {
            this.EnsureNodeId(entity);
            entity.UpdatingEntity();
            Database.Update(this._formFactory.BuildDto(entity));
            NodeDto relatedNodeId = this.GetRelatedNodeId(entity);
            if (relatedNodeId != null && relatedNodeId.Text != entity.Name)
            {
                relatedNodeId.Text = entity.Name;
                Database.Update(relatedNodeId);
            }
            entity.ResetDirtyProperties();
        }

        private void EnsureNodeId(FormEntity entity)
        {
            int? nullable = Database.FirstOrDefault<int?>(NPocoSqlExtensions.From<FormDto>(NPocoSqlExtensions.Select<FormDto>(Sql(), new Expression<Func<FormDto, object>>[1]
            {
         x =>  x.NodeId
            }), null).Where<FormDto>(p => p.Key == entity.Key, null));
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
