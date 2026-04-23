using FormBuilder.Core.Persistence;
using FormBuilder.Core.Storage.Interfaces;

using System.Data;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class UserGroupStartFolderStorage(IScopeProvider scopeProvider) : IUserGroupStartFolderStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public IEnumerable<Guid> GetStartFolderKeys(int userGroupId)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Query<UserGroupStartFolder>().Where((Expression<Func<UserGroupStartFolder, bool>>)(x => x.UserGroupId == userGroupId)).ToList().Select(x => x.FolderKey);
        }

        public void UpdateStartFolders(int userGroupId, IEnumerable<Guid> folderKeys)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserGroupStartFolder>("WHERE UserGroupId = @0", userGroupId);
            foreach (Guid folderKey in folderKeys)
            {
                UserGroupStartFolder poco = new()
                {
                    UserGroupId = userGroupId,
                    FolderKey = folderKey
                };
                scope.Database.Insert(poco);
            }
              ((ICoreScope)scope).Complete();
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}