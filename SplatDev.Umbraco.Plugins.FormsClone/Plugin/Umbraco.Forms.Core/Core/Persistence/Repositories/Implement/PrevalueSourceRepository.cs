
// Type: Umbraco.Forms.Core.Persistence.Repositories.Implement.PrevalueSourceRepository
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
    internal sealed class PrevalueSourceRepository :
      EntityRepositoryBase<Guid, PrevalueSourceEntity>,
      IPrevalueSourceRepository,
      IReadWriteQueryRepository<Guid, PrevalueSourceEntity>,
      IReadRepository<Guid, PrevalueSourceEntity>,
      IRepository,
      IWriteRepository<PrevalueSourceEntity>,
      IQueryRepository<PrevalueSourceEntity>
    {
        private readonly IPrevalueSourceFactory _prevalueSourceFactory;

        public PrevalueSourceRepository(
          IScopeAccessor scopeAccessor,
          AppCaches cache,
          ILogger<PrevalueSourceRepository> logger,
          IPrevalueSourceFactory prevalueSourceFactory)
          : base(scopeAccessor, cache, logger)
        {
            this._prevalueSourceFactory = prevalueSourceFactory;
        }

        [ExcludeFromCodeCoverage]
        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return NPocoSqlExtensions.From<PrevalueSourceDto>(isCount ? NPocoSqlExtensions.SelectCount(sql, null) : NPocoSqlExtensions.Select<PrevalueSourceDto>(sql, Array.Empty<Expression<Func<PrevalueSourceDto, object>>>()), null);
        }

        [ExcludeFromCodeCoverage]
        protected override string GetBaseWhereClause() => "UFPrevalueSource.id = @id";

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<string> GetDeleteClauses() => new List<string>()
    {
      "DELETE FROM UFPrevalueSource WHERE [key] = @key"
    };

        [ExcludeFromCodeCoverage]
        protected override PrevalueSourceEntity? PerformGet(Guid id)
        {
            PrevalueSourceDto dto = Database.FirstOrDefault<PrevalueSourceDto>(NPocoSqlExtensions.From<PrevalueSourceDto>(NPocoSqlExtensions.Select<PrevalueSourceDto>(SqlContext.Sql(), Array.Empty<Expression<Func<PrevalueSourceDto, object>>>()), null).Where<PrevalueSourceDto>(p => p.Key == id, null));
            return dto != null ? this._prevalueSourceFactory.BuildEntity(dto) : null;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<PrevalueSourceEntity> PerformGetAll(
          params Guid[]? ids)
        {
            return ids == null || ids.Length == 0 ? this.GetDtos().Select<PrevalueSourceDto, PrevalueSourceEntity>(new Func<PrevalueSourceDto, PrevalueSourceEntity>(this._prevalueSourceFactory.BuildEntity)) : this.GetDtos(ids).Select<PrevalueSourceDto, PrevalueSourceEntity>(new Func<PrevalueSourceDto, PrevalueSourceEntity>(this._prevalueSourceFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        public IEnumerable<PrevalueSourceEntitySlim> GetManySlim(
          params Guid[]? ids)
        {
            Expression<Func<PrevalueSourceDto, object>>[] expressionArray = new Expression<Func<PrevalueSourceDto, object>>[4]
            {
         x =>  x.Id,
         x => x.Name,
         x =>  x.Key,
         x =>  x.CreateDate
            };
            return ids == null || ids.Length == 0 ? this.GetDtos(expressionArray).Select<PrevalueSourceDto, PrevalueSourceEntitySlim>(new Func<PrevalueSourceDto, PrevalueSourceEntitySlim>(this._prevalueSourceFactory.BuildEntitySlim)) : this.GetDtos(ids, expressionArray).Select<PrevalueSourceDto, PrevalueSourceEntitySlim>(new Func<PrevalueSourceDto, PrevalueSourceEntitySlim>(this._prevalueSourceFactory.BuildEntitySlim));
        }

        public IEnumerable<PrevalueSourceDto> GetDtos(
          params Expression<Func<PrevalueSourceDto, object?>>[] fields)
        {
            return Database.Fetch<PrevalueSourceDto>(NPocoSqlExtensions.Select<PrevalueSourceDto>(Sql(), fields).From<PrevalueSourceDto>(null));
        }

        public IEnumerable<PrevalueSourceDto> GetDtos(
          Guid[] ids,
          params Expression<Func<PrevalueSourceDto, object?>>[] fields)
        {
            return NPocoDatabaseExtensions.FetchByGroups<PrevalueSourceDto, Guid>(Database, ids, 2000, batch => NPocoSqlExtensions.WhereIn<PrevalueSourceDto>(NPocoSqlExtensions.From<PrevalueSourceDto>(NPocoSqlExtensions.Select<PrevalueSourceDto>(Sql(), fields), null), x => x.Key, batch));
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<PrevalueSourceEntity> PerformGetByQuery(
          IQuery<PrevalueSourceEntity> query)
        {
            return Database.Fetch<PrevalueSourceDto>(new SqlTranslator<PrevalueSourceEntity>(GetBaseQuery(false), query).Translate()).Select<PrevalueSourceDto, PrevalueSourceEntity>(new Func<PrevalueSourceDto, PrevalueSourceEntity>(this._prevalueSourceFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistNewItem(PrevalueSourceEntity entity)
        {
            entity.AddingEntity();
            PrevalueSourceDto poco = this._prevalueSourceFactory.BuildDto(entity);
            Database.Insert<PrevalueSourceDto>(poco);
            entity.Id = poco.Id;
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistUpdatedItem(PrevalueSourceEntity entity)
        {
            entity.UpdatingEntity();
            Database.Update(this._prevalueSourceFactory.BuildDto(entity));
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
