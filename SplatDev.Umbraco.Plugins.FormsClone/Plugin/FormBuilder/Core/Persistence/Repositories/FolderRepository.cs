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
    internal sealed class FolderRepository(
      IScopeAccessor scopeAccessor,
      AppCaches cache,
      ILogger<FolderRepository> logger,
      IFolderFactory folderFactory) :
      EntityRepositoryBase<Guid, FolderEntity>(scopeAccessor, cache, logger),
      IFolderRepository,
      IReadWriteQueryRepository<Guid, FolderEntity>,
      IReadRepository<Guid, FolderEntity>,
      IRepository,
      IWriteRepository<FolderEntity>,
      IQueryRepository<FolderEntity>
    {
        private readonly IFolderFactory _folderFactory = folderFactory;

        [ExcludeFromCodeCoverage]
        public IEnumerable<FolderEntity> GetAtRoot() => GetFolders(x => x.ParentKey == new Guid?());

        [ExcludeFromCodeCoverage]
        public IEnumerable<FolderEntity> GetChildren(Guid parentId) => GetFolders(x => x.ParentKey == (Guid?)parentId);

        [ExcludeFromCodeCoverage]
        private IEnumerable<FolderEntity> GetFolders(
          Expression<Func<FolderDto, bool>> wherePredicate)
        {
            return Database.Fetch<FolderDto>(Sql().Select<FolderDto>([])
.From<FolderDto>(null).Where(wherePredicate, null).OrderBy<FolderDto>(x => x.Name)).Select(new Func<FolderDto, FolderEntity>(_folderFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        public bool ExistsAndIsEmpty(Guid id)
        {
            if (Get(id) == null)
                return false;
            if (Database.ExecuteScalar<int>(Sql().SelectCount(null).From<FolderDto>(null).Where<FolderDto>(x => x.ParentKey == (Guid?)id, null)) > 0)
                return false;
            return Database.ExecuteScalar<int>(Sql().SelectCount(null).From<FormDto>(null).Where<FormDto>(x => x.FolderKey == (Guid?)id, null)) == 0;
        }

        [ExcludeFromCodeCoverage]
        protected override Sql<ISqlContext> GetBaseQuery(bool isCount)
        {
            Sql<ISqlContext> sql = Sql();
            return (isCount ? sql.SelectCount(null) : sql.Select<FolderDto>([])).From<FolderDto>(null);
        }

        [ExcludeFromCodeCoverage]
        protected override string GetBaseWhereClause() => "UFFolders.id = @id";

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<string> GetDeleteClauses() =>
        [
          "DELETE FROM FormBuilderUserStartFolders WHERE FolderKey = @key",
          "DELETE FROM FormBuilderFolders WHERE [key] = @key"
        ];

        [ExcludeFromCodeCoverage]
        protected override FolderEntity? PerformGet(Guid id)
        {
            FolderDto dto = Database.FirstOrDefault<FolderDto>(SqlContext.Sql().Select<FolderDto>([]).From<FolderDto>(null).Where<FolderDto>(p => p.Key == id, null));
            return dto is not null ? _folderFactory.BuildEntity(dto) : null;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<FolderEntity> PerformGetAll(
          params Guid[]? ids)
        {
            if (ids == null || ids.Length == 0)
                return Database.Fetch<FolderDto>(Sql().Select<FolderDto>([]).From<FolderDto>(null)).Select(new Func<FolderDto, FolderEntity>(_folderFactory.BuildEntity));
            List<FolderEntity> all = [];
            foreach (IEnumerable<Guid> guids in ids.InGroupsOf(2000))
            {
                Sql<ISqlContext> sql = Sql().Select<FolderDto>([]).From<FolderDto>(null).WhereIn<FolderDto>(x => x.Key, guids);
                all.AddRange(Database.Fetch<FolderDto>(sql).Select(new Func<FolderDto, FolderEntity>(_folderFactory.BuildEntity)));
            }
            return all;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<FolderEntity> PerformGetByQuery(
          IQuery<FolderEntity> query)
        {
            return Database.Fetch<FolderDto>(new SqlTranslator<FolderEntity>(GetBaseQuery(false), query).Translate()).Select(new Func<FolderDto, FolderEntity>(_folderFactory.BuildEntity));
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistNewItem(FolderEntity entity)
        {
            entity.AddingEntity();
            FolderDto poco = _folderFactory.BuildDto(entity);
            Database.Insert(poco);
            entity.Id = poco.Id;
            entity.ResetDirtyProperties();
        }

        [ExcludeFromCodeCoverage]
        protected override void PersistUpdatedItem(FolderEntity entity)
        {
            entity.UpdatingEntity();
            Database.Update(_folderFactory.BuildDto(entity));
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