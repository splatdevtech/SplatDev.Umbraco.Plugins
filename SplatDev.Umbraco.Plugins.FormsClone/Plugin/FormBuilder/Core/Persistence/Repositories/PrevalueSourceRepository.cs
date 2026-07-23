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
using Umbraco.Cms.Infrastructure.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Persistence.Repositories.Implement;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace FormBuilder.Core.Persistence.Repositories
{
    internal sealed class PrevalueSourceRepository(
      IScopeAccessor scopeAccessor,
      AppCaches cache,
      ILogger<PrevalueSourceRepository> logger,
      IPrevalueSourceFactory prevalueSourceFactory) :
      EntityRepositoryBase<Guid, PrevalueSourceEntity>(scopeAccessor, cache, logger),
      IPrevalueSourceRepository,
      IReadWriteQueryRepository<Guid, PrevalueSourceEntity>,
      IReadRepository<Guid, PrevalueSourceEntity>,
      IRepository,
      IWriteRepository<PrevalueSourceEntity>,
      IQueryRepository<PrevalueSourceEntity>
    {
        private readonly IPrevalueSourceFactory _prevalueSourceFactory = prevalueSourceFactory;

        [ExcludeFromCodeCoverage]
        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return (isCount ? sql.SelectCount(null) : NPocoSqlExtensions.Select(sql, Array.Empty<Expression<Func<PrevalueSourceDto, object>>>()!)).From<PrevalueSourceDto>(null);
        }

        [ExcludeFromCodeCoverage]
        protected override string GetBaseWhereClause() => "UFPrevalueSource.id = @id";

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<string> GetDeleteClauses() =>
        [
          "DELETE FROM FormBuilderPrevalueSource WHERE [key] = @key"
        ];

        [ExcludeFromCodeCoverage]
        protected override PrevalueSourceEntity? PerformGet(Guid id)
        {
            PrevalueSourceDto dto = Database.FirstOrDefault<PrevalueSourceDto>(NPocoSqlExtensions.Select(SqlContext.Sql(), Array.Empty<Expression<Func<PrevalueSourceDto, object>>>()!).From<PrevalueSourceDto>(null).Where<PrevalueSourceDto>(p => p.Key == id, null));
            return dto is not null ? _prevalueSourceFactory.BuildEntity(dto) : null;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<PrevalueSourceEntity> PerformGetAll(
          params Guid[]? ids)
        {
            return ids is null || ids.Length == 0 ? GetDtos().Select(new Func<PrevalueSourceDto, PrevalueSourceEntity>(_prevalueSourceFactory.BuildEntity)) : GetDtos(ids).Select(new Func<PrevalueSourceDto, PrevalueSourceEntity>(_prevalueSourceFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        public IEnumerable<PrevalueSourceEntitySlim> GetManySlim(
          params Guid[]? ids)
        {
            Expression<Func<PrevalueSourceDto, object>>[] expressionArray =
            [
                 x =>  x.Id,
                 x => x.Name,
                 x =>  x.Key,
                 x =>  x.CreateDate
            ];
            return ids is null || ids.Length == 0 ? GetDtos(expressionArray!).Select(new Func<PrevalueSourceDto, PrevalueSourceEntitySlim>(_prevalueSourceFactory.BuildEntitySlim)) : GetDtos(ids, expressionArray!).Select(new Func<PrevalueSourceDto, PrevalueSourceEntitySlim>(_prevalueSourceFactory.BuildEntitySlim));
        }

        public IEnumerable<PrevalueSourceDto> GetDtos(
          params Expression<Func<PrevalueSourceDto, object?>>[] fields)
        {
            return Database.Fetch<PrevalueSourceDto>(NPocoSqlExtensions.Select(Sql(), fields).From<PrevalueSourceDto>(null));
        }

        public IEnumerable<PrevalueSourceDto> GetDtos(
          Guid[] ids,
          params Expression<Func<PrevalueSourceDto, object?>>[] fields)
        {
            return Database.FetchByGroups<PrevalueSourceDto, Guid>(ids, 2000, batch => NPocoSqlExtensions.Select(Sql(), fields).From<PrevalueSourceDto>(null).WhereIn<PrevalueSourceDto>(x => x.Key, batch));
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<PrevalueSourceEntity> PerformGetByQuery(
          IQuery<PrevalueSourceEntity> query)
        {
            return Database.Fetch<PrevalueSourceDto>(new SqlTranslator<PrevalueSourceEntity>(GetBaseQuery(false), query).Translate()).Select(new Func<PrevalueSourceDto, PrevalueSourceEntity>(_prevalueSourceFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistNewItem(PrevalueSourceEntity entity)
        {
            entity.AddingEntity();
            PrevalueSourceDto poco = _prevalueSourceFactory.BuildDto(entity);
            Database.Insert(poco);
            entity.Id = poco.Id;
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistUpdatedItem(PrevalueSourceEntity entity)
        {
            entity.UpdatingEntity();
            Database.Update(_prevalueSourceFactory.BuildDto(entity));
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistDeletedItem(PrevalueSourceEntity entity)
        {
            foreach (string deleteClause in GetDeleteClauses())
                Database.Execute(deleteClause, new
                {
                    key = entity.Key
                });
            entity.DeleteDate = new DateTime?(DateTime.Now);
        }
    }
}