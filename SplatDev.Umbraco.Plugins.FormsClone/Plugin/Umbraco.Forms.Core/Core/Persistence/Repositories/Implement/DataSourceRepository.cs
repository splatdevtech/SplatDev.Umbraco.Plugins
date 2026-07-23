
// Type: Umbraco.Forms.Core.Persistence.Repositories.Implement.DataSourceRepository
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
    internal sealed class DataSourceRepository :
      EntityRepositoryBase<Guid, DataSourceEntity>,
      IDataSourceRepository,
      IReadWriteQueryRepository<Guid, DataSourceEntity>,
      IReadRepository<Guid, DataSourceEntity>,
      IRepository,
      IWriteRepository<DataSourceEntity>,
      IQueryRepository<DataSourceEntity>
    {
        private readonly IDataSourceFactory _dataSourceFactory;

        public DataSourceRepository(
          IScopeAccessor scopeAccessor,
          AppCaches cache,
          ILogger<DataSourceRepository> logger,
          IDataSourceFactory dataSourceFactory)
          : base(scopeAccessor, cache, logger)
        {
            this._dataSourceFactory = dataSourceFactory;
        }

        [ExcludeFromCodeCoverage]
        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return NPocoSqlExtensions.From<DataSourceDto>(isCount ? NPocoSqlExtensions.SelectCount(sql, null) : NPocoSqlExtensions.Select<DataSourceDto>(sql, Array.Empty<Expression<Func<DataSourceDto, object>>>()), null);
        }

        [ExcludeFromCodeCoverage]
        protected override string GetBaseWhereClause() => "UFDataSource.id = @id";

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<string> GetDeleteClauses() => new List<string>()
    {
      "DELETE FROM UFDataSource WHERE [key] = @key"
    };

        [ExcludeFromCodeCoverage]
        protected override DataSourceEntity? PerformGet(Guid id)
        {
            DataSourceDto dto = (Database).FirstOrDefault<DataSourceDto>(NPocoSqlExtensions.From<DataSourceDto>(NPocoSqlExtensions.Select<DataSourceDto>(SqlContext.Sql(), Array.Empty<Expression<Func<DataSourceDto, object>>>()), null).Where<DataSourceDto>(p => p.Key == id, null));
            return dto != null ? this._dataSourceFactory.BuildEntity(dto) : null;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<DataSourceEntity> PerformGetAll(
          params Guid[]? ids)
        {
            return ids == null || ids.Length == 0 ? this.GetDtos().Select<DataSourceDto, DataSourceEntity>(new Func<DataSourceDto, DataSourceEntity>(this._dataSourceFactory.BuildEntity)) : this.GetDtos(ids).Select<DataSourceDto, DataSourceEntity>(new Func<DataSourceDto, DataSourceEntity>(this._dataSourceFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        public IEnumerable<DataSourceEntitySlim> GetManySlim(
          params Guid[]? ids)
        {
            Expression<Func<DataSourceDto, object>>[] expressionArray = new Expression<Func<DataSourceDto, object>>[4]
            {
         x =>  x.Id,
         x => x.Name,
         x =>  x.Key,
         x =>  x.CreateDate
            };
            return ids == null || ids.Length == 0 ? this.GetDtos(expressionArray).Select<DataSourceDto, DataSourceEntitySlim>(new Func<DataSourceDto, DataSourceEntitySlim>(this._dataSourceFactory.BuildEntitySlim)) : this.GetDtos(ids, expressionArray).Select<DataSourceDto, DataSourceEntitySlim>(new Func<DataSourceDto, DataSourceEntitySlim>(this._dataSourceFactory.BuildEntitySlim));
        }

        public IEnumerable<DataSourceDto> GetDtos(
          params Expression<Func<DataSourceDto, object?>>[] fields)
        {
            return (Database).Fetch<DataSourceDto>(NPocoSqlExtensions.Select<DataSourceDto>(Sql(), fields).From<DataSourceDto>(null));
        }

        public IEnumerable<DataSourceDto> GetDtos(
          Guid[] ids,
          params Expression<Func<DataSourceDto, object?>>[] fields)
        {
            return NPocoDatabaseExtensions.FetchByGroups<DataSourceDto, Guid>(Database, ids, 2000, batch => NPocoSqlExtensions.WhereIn<DataSourceDto>(NPocoSqlExtensions.From<DataSourceDto>(NPocoSqlExtensions.Select<DataSourceDto>(Sql(), fields), null), x => x.Key, batch));
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<DataSourceEntity> PerformGetByQuery(
          IQuery<DataSourceEntity> query)
        {
            return (Database).Fetch<DataSourceDto>(new SqlTranslator<DataSourceEntity>(GetBaseQuery(false), query).Translate()).Select<DataSourceDto, DataSourceEntity>(new Func<DataSourceDto, DataSourceEntity>(this._dataSourceFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistNewItem(DataSourceEntity entity)
        {
            entity.AddingEntity();
            Database.Insert<DataSourceDto>(this._dataSourceFactory.BuildDto(entity));
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistUpdatedItem(DataSourceEntity entity)
        {
            entity.UpdatingEntity();
            Database.Update(this._dataSourceFactory.BuildDto(entity));
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistDeletedItem(DataSourceEntity entity)
        {
            foreach (string deleteClause in GetDeleteClauses())
                (Database).Execute(deleteClause, new
                {
                    key = entity.Key
                });
            entity.DeleteDate = new DateTime?(DateTime.Now);
        }
    }
}
