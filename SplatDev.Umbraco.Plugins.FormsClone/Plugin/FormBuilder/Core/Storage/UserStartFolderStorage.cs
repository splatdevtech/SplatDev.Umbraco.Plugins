using FormBuilder.Core.Persistence;
using FormBuilder.Core.Storage.Interfaces;

using System.Data;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class UserStartFolderStorage(IScopeProvider scopeProvider) : IUserStartFolderStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public IEnumerable<Guid> GetStartFolderKeys(int userId)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Query<UserStartFolder>().Where((Expression<Func<UserStartFolder, bool>>)(x => x.UserId == userId)).ToList().Select(x => x.FolderKey);
        }

        public void UpdateStartFolders(int userId, IEnumerable<Guid> folderKeys)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserStartFolder>("WHERE UserId = @0", userId);
            foreach (Guid folderKey in folderKeys)
            {
                UserStartFolder poco = new()
                {
                    UserId = userId,
                    FolderKey = folderKey
                };
                scope.Database.Insert(poco);
            }
              ((ICoreScope)scope).Complete();
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}