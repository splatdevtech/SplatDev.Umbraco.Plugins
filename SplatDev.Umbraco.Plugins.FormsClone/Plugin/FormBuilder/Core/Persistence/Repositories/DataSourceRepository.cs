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
    internal sealed class DataSourceRepository(
      IScopeAccessor scopeAccessor,
      AppCaches cache,
      ILogger<DataSourceRepository> logger,
      IDataSourceFactory dataSourceFactory) :
      EntityRepositoryBase<Guid, DataSourceEntity>(scopeAccessor, cache, logger),
      IDataSourceRepository,
      IReadWriteQueryRepository<Guid, DataSourceEntity>,
      IReadRepository<Guid, DataSourceEntity>,
      IRepository,
      IWriteRepository<DataSourceEntity>,
      IQueryRepository<DataSourceEntity>
    {
        private readonly IDataSourceFactory _dataSourceFactory = dataSourceFactory;

        [ExcludeFromCodeCoverage]
        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return (isCount ? sql.SelectCount(null) : NPocoSqlExtensions.Select(sql, Array.Empty<Expression<Func<DataSourceDto, object>>>()!)).From<DataSourceDto>(null);
        }

        [ExcludeFromCodeCoverage]
        protected override string GetBaseWhereClause() => "UFDataSource.id = @id";

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<string> GetDeleteClauses() =>
        [
          "DELETE FROM FormBuilderDataSource WHERE [key] = @key"
        ];

        [ExcludeFromCodeCoverage]
        protected override DataSourceEntity? PerformGet(Guid id)
        {
            DataSourceDto dto = Database.FirstOrDefault<DataSourceDto>(NPocoSqlExtensions.Select(SqlContext.Sql(), Array.Empty<Expression<Func<DataSourceDto, object>>>()!).From<DataSourceDto>(null).Where<DataSourceDto>(p => p.Key == id, null));
            return dto is not null ? _dataSourceFactory.BuildEntity(dto) : null;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<DataSourceEntity> PerformGetAll(
          params Guid[]? ids)
        {
            return ids is null || ids.Length == 0 ? GetDtos().Select(new Func<DataSourceDto, DataSourceEntity>(_dataSourceFactory.BuildEntity)) : GetDtos(ids).Select(new Func<DataSourceDto, DataSourceEntity>(_dataSourceFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        public IEnumerable<DataSourceEntitySlim> GetManySlim(
          params Guid[]? ids)
        {
            Expression<Func<DataSourceDto, object>>[] expressionArray =
            [
                 x =>  x.Id,
                 x => x.Name,
                 x =>  x.Key,
                 x =>  x.CreateDate
            ];
            return ids is null || ids.Length == 0 ? GetDtos(expressionArray!).Select(new Func<DataSourceDto, DataSourceEntitySlim>(_dataSourceFactory.BuildEntitySlim)) : GetDtos(ids, expressionArray!).Select(new Func<DataSourceDto, DataSourceEntitySlim>(_dataSourceFactory.BuildEntitySlim));
        }

        public IEnumerable<DataSourceDto> GetDtos(
          params Expression<Func<DataSourceDto, object?>>[] fields)
        {
            return Database.Fetch<DataSourceDto>(NPocoSqlExtensions.Select(Sql(), fields).From<DataSourceDto>(null));
        }

        public IEnumerable<DataSourceDto> GetDtos(
          Guid[] ids,
          params Expression<Func<DataSourceDto, object?>>[] fields)
        {
            return Database.FetchByGroups<DataSourceDto, Guid>(ids, 2000, batch => NPocoSqlExtensions.Select(Sql(), fields).From<DataSourceDto>(null).WhereIn<DataSourceDto>(x => x.Key, batch));
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<DataSourceEntity> PerformGetByQuery(
          IQuery<DataSourceEntity> query)
        {
            return Database.Fetch<DataSourceDto>(new SqlTranslator<DataSourceEntity>(GetBaseQuery(false), query).Translate()).Select(new Func<DataSourceDto, DataSourceEntity>(_dataSourceFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistNewItem(DataSourceEntity entity)
        {
            entity.AddingEntity();
            Database.Insert(_dataSourceFactory.BuildDto(entity));
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistUpdatedItem(DataSourceEntity entity)
        {
            entity.UpdatingEntity();
            Database.Update(_dataSourceFactory.BuildDto(entity));
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistDeletedItem(DataSourceEntity entity)
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