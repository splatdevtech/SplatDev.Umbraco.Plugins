
// Type: Umbraco.Forms.Core.Persistence.Repositories.Implement.FolderRepository
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
    internal sealed class FolderRepository :
      EntityRepositoryBase<Guid, FolderEntity>,
      IFolderRepository,
      IReadWriteQueryRepository<Guid, FolderEntity>,
      IReadRepository<Guid, FolderEntity>,
      IRepository,
      IWriteRepository<FolderEntity>,
      IQueryRepository<FolderEntity>
    {
        private readonly IFolderFactory _folderFactory;

        public FolderRepository(
          IScopeAccessor scopeAccessor,
          AppCaches cache,
          ILogger<FolderRepository> logger,
          IFolderFactory folderFactory)
          : base(scopeAccessor, cache, logger)
        {
            this._folderFactory = folderFactory;
        }

        [ExcludeFromCodeCoverage]
        public IEnumerable<FolderEntity> GetAtRoot() => this.GetFolders(x => x.ParentKey == null);

        [ExcludeFromCodeCoverage]
        public IEnumerable<FolderEntity> GetChildren(Guid parentId) => this.GetFolders(x => x.ParentKey == (Guid?)parentId);

        [ExcludeFromCodeCoverage]
        private IEnumerable<FolderEntity> GetFolders(
          Expression<Func<FolderDto, bool>> wherePredicate)
        {
            var folders = Database.Fetch<FolderDto>(NPocoSqlExtensions.Where<FolderDto>(NPocoSqlExtensions.From<FolderDto>(NPocoSqlExtensions.Select<FolderDto>(Sql(), Array.Empty<Expression<Func<FolderDto, object>>>()), null), wherePredicate, null).OrderBy<FolderDto>(x => x.Name)).Select<FolderDto, FolderEntity>(new Func<FolderDto, FolderEntity>(this._folderFactory.BuildEntity));
            return folders;
        }

        [ExcludeFromCodeCoverage]
        public bool ExistsAndIsEmpty(Guid id)
        {
            if (this.Get(id) == null)
                return false;
            if (Database.ExecuteScalar<int>(NPocoSqlExtensions.From<FolderDto>(NPocoSqlExtensions.SelectCount(Sql(), null), null).Where<FolderDto>(x => x.ParentKey == (Guid?)id, null)) > 0)
                return false;
            return Database.ExecuteScalar<int>(NPocoSqlExtensions.From<FormDto>(NPocoSqlExtensions.SelectCount(Sql(), null), null).Where<FormDto>(x => x.FolderKey == (Guid?)id, null)) == 0;
        }

        [ExcludeFromCodeCoverage]
        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return NPocoSqlExtensions.From<FolderDto>(isCount ? NPocoSqlExtensions.SelectCount(sql, null) : NPocoSqlExtensions.Select<FolderDto>(sql, Array.Empty<Expression<Func<FolderDto, object>>>()), null);
        }

        [ExcludeFromCodeCoverage]
        protected override string GetBaseWhereClause() => "UFFolders.id = @id";

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<string> GetDeleteClauses() => new List<string>()
    {
      "DELETE FROM UFUserStartFolders WHERE FolderKey = @key",
      "DELETE FROM UFFolders WHERE [key] = @key"
    };

        [ExcludeFromCodeCoverage]
        protected override FolderEntity? PerformGet(Guid id)
        {
            FolderDto dto = Database.FirstOrDefault<FolderDto>(NPocoSqlExtensions.From<FolderDto>(NPocoSqlExtensions.Select<FolderDto>(SqlContext.Sql(), Array.Empty<Expression<Func<FolderDto, object>>>()), null).Where<FolderDto>(p => p.Key == id, null));
            return dto != null ? this._folderFactory.BuildEntity(dto) : null;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<FolderEntity> PerformGetAll(
          params Guid[]? ids)
        {
            if (ids == null || ids.Length == 0)
                return Database.Fetch<FolderDto>(NPocoSqlExtensions.Select<FolderDto>(Sql(), Array.Empty<Expression<Func<FolderDto, object>>>()).From<FolderDto>(null)).Select<FolderDto, FolderEntity>(new Func<FolderDto, FolderEntity>(this._folderFactory.BuildEntity));
            List<FolderEntity> all = new List<FolderEntity>();
            foreach (IEnumerable<Guid> guids in ids.InGroupsOf<Guid>(2000))
            {
                Sql<ISqlContext> sql = NPocoSqlExtensions.WhereIn<FolderDto>(NPocoSqlExtensions.From<FolderDto>(NPocoSqlExtensions.Select<FolderDto>(Sql(), Array.Empty<Expression<Func<FolderDto, object>>>()), null), x => x.Key, guids);
                all.AddRange(Database.Fetch<FolderDto>(sql).Select<FolderDto, FolderEntity>(new Func<FolderDto, FolderEntity>(this._folderFactory.BuildEntity)));
            }
            return all;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<FolderEntity> PerformGetByQuery(
          IQuery<FolderEntity> query)
        {
            return Database.Fetch<FolderDto>(new SqlTranslator<FolderEntity>(GetBaseQuery(false), query).Translate()).Select<FolderDto, FolderEntity>(new Func<FolderDto, FolderEntity>(this._folderFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistNewItem(FolderEntity entity)
        {
            entity.AddingEntity();
            FolderDto poco = this._folderFactory.BuildDto(entity);
            Database.Insert<FolderDto>(poco);
            entity.Id = poco.Id;
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistUpdatedItem(FolderEntity entity)
        {
            entity.UpdatingEntity();
            Database.Update(this._folderFactory.BuildDto(entity));
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistDeletedItem(FolderEntity entity)
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
