
// Type: Umbraco.Forms.Data.Storage.UserGroupStartFolderStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Data;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Scoping;
using Umbraco.Forms.Core.Persistence.Dtos;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Data.Storage
{
    internal sealed class UserGroupStartFolderStorage : IUserGroupStartFolderStorage
    {
        private readonly IScopeProvider _scopeProvider;

        public UserGroupStartFolderStorage(IScopeProvider scopeProvider) => this._scopeProvider = scopeProvider;

        public IEnumerable<Guid> GetStartFolderKeys(int userGroupId)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Query<UserGroupStartFolder>().Where((Expression<Func<UserGroupStartFolder, bool>>)(x => x.UserGroupId == userGroupId)).ToList().Select<UserGroupStartFolder, Guid>(x => x.FolderKey);
        }

        public void UpdateStartFolders(int userGroupId, IEnumerable<Guid> folderKeys)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Delete<UserGroupStartFolder>("WHERE UserGroupId = @0", userGroupId);
                foreach (Guid folderKey in folderKeys)
                {
                    UserGroupStartFolder poco = new UserGroupStartFolder()
                    {
                        UserGroupId = userGroupId,
                        FolderKey = folderKey
                    };
                    scope.Database.Insert<UserGroupStartFolder>(poco);
                }
                scope.Complete();
            }
        }
    }
}
