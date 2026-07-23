
// Type: Umbraco.Forms.Data.Storage.UserStartFolderStorage
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
    internal sealed class UserStartFolderStorage : IUserStartFolderStorage
    {
        private readonly IScopeProvider _scopeProvider;

        public UserStartFolderStorage(IScopeProvider scopeProvider) => this._scopeProvider = scopeProvider;

        public IEnumerable<Guid> GetStartFolderKeys(int userId)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Query<UserStartFolder>().Where((Expression<Func<UserStartFolder, bool>>)(x => x.UserId == userId)).ToList().Select<UserStartFolder, Guid>(x => x.FolderKey);
        }

        public void UpdateStartFolders(int userId, IEnumerable<Guid> folderKeys)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Delete<UserStartFolder>("WHERE UserId = @0", userId);
                foreach (Guid folderKey in folderKeys)
                {
                    UserStartFolder poco = new UserStartFolder()
                    {
                        UserId = userId,
                        FolderKey = folderKey
                    };
                    scope.Database.Insert<UserStartFolder>(poco);
                }
                scope.Complete();
            }
        }
    }
}
