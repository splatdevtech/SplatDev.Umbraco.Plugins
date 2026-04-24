
// Type: Umbraco.Forms.Data.Storage.UserSecurityStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Data;
using System.Globalization;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Scoping;
using Umbraco.Forms.Core.Persistence.Dtos;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Data.Storage
{
    internal sealed class UserSecurityStorage : IUserSecurityStorage
    {
        private readonly IScopeProvider _scopeProvider;

        public UserSecurityStorage(IScopeProvider scopeProvider) => this._scopeProvider = scopeProvider;

        public List<UserSecurity> GetAllUserSecurity()
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Fetch<UserSecurity>();
        }

        public UserSecurity? GetUserSecurity(int userId)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Query<UserSecurity>().FirstOrDefault((Expression<Func<UserSecurity, bool>>)(x => x.User == userId.ToString(CultureInfo.InvariantCulture)));
        }

        public UserSecurity InsertUserSecurity(UserSecurity usersecurity)
        {
            try
            {
                using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                {
                    scope.Database.Insert<UserSecurity>(usersecurity);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("UNIQUE constraint failed") || ex.Message.StartsWith("Violation of PRIMARY KEY constraint"))
                    return usersecurity;
                throw;
            }
            return usersecurity;
        }

        public bool DeleteUserSecurity(UserSecurity userSecurity)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Delete<UserSecurity>(userSecurity);
                scope.Complete();
            }
            return true;
        }

        public UserSecurity UpdateUserSecurity(UserSecurity userSecurity)
        {
            int result;
            if (!int.TryParse(userSecurity.User, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return userSecurity;
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Update<UserSecurity>("SET manageforms = @0, managedatasources = @1, manageprevaluesources = @2, manageworkflows = @3, viewEntries = @4, editEntries = @5, deleteEntries = @6 WHERE [user] = @7", userSecurity.ManageForms, userSecurity.ManageDataSources, userSecurity.ManagePreValueSources, userSecurity.ManageWorkflows, userSecurity.ViewEntries, userSecurity.EditEntries, userSecurity.DeleteEntries, result);
                scope.Complete();
            }
            return userSecurity;
        }
    }
}
