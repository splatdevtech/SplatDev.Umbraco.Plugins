using NPoco;
using NPoco.Linq;

using SplatDev.Umbraco.NPoco.Notifications;

using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Core.Services;

using Umbraco.Cms.Infrastructure.Persistence;

namespace SplatDev.Umbraco.NPoco.Repositories
{
    public interface IRepository<T> : IDisposable where T : IBaseEntity
    {
        int? UserId { get; set; }

        int? Count();

        void Delete(int id);

        void Delete(T entity);

        void DeleteBulk(IList<int> ids);

        void DeleteBulk(IList<T> entities);

        bool Exists(int id);

        IList<T> Fetch(string query);

        T? Get(int? id);

        T? Get(string column, object? value);

        IEnumerable<T> GetAll();

        IEnumerable<T> GetMany(params int[]? ids);

        IEnumerable<T> GetPagedResultsByQuery(
            IQuery<T> query,
            long pageIndex,
            int pageSize,
            out long totalRecords,
            IQuery<T>? filter,
            Ordering? ordering);

        IEnumerable<T> GetPagedResultsByQuery(
            IQuery<T> query,
            long pageIndex,
            int pageSize,
            out long totalRecords,
            IQuery<T>? filter,
            Ordering? ordering,
            Action<Sql<ISqlContext>>? sqlCustomization = null);
        T Insert(T entity);

        void InsertBulk(List<T> entities);

        void Update(T entity);

        void UpdateBulk(List<T> entities);
        IEnumerable<T> GetMany(int id);

        #region Events
        delegate void OnActionCompleted(object sender, ActionCompletedEvent e);
        event OnActionCompleted? ActionCompleted;
        #endregion
    }
}
