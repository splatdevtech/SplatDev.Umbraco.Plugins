using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Storage.Interfaces;

using System.Data;
using System.Globalization;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class UserSecurityStorage(IScopeProvider scopeProvider) : IUserSecurityStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public List<UserSecurity> GetAllUserSecurity()
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Fetch<UserSecurity>();
        }

        public UserSecurity? GetUserSecurity(int userId)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Query<UserSecurity>().FirstOrDefault((Expression<Func<UserSecurity, bool>>)(x => x.User == userId.ToString(CultureInfo.InvariantCulture)));
        }

        public UserSecurity InsertUserSecurity(UserSecurity usersecurity)
        {
            try
            {
                using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
                scope.Database.Insert(usersecurity);
                ((ICoreScope)scope).Complete();
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
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserSecurity>(userSecurity);
            ((ICoreScope)scope).Complete();
            return true;
        }

        public UserSecurity UpdateUserSecurity(UserSecurity userSecurity)
        {
            if (!int.TryParse(userSecurity.User, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                return userSecurity;
            using (IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                scope.Database.Update<UserSecurity>("SET manageforms = @0, managedatasources = @1, manageprevaluesources = @2, manageworkflows = @3, viewEntries = @4, editEntries = @5, deleteEntries = @6 WHERE [user] = @7", userSecurity.ManageForms, userSecurity.ManageDataSources, userSecurity.ManagePreValueSources, userSecurity.ManageWorkflows, userSecurity.ViewEntries, userSecurity.EditEntries, userSecurity.DeleteEntries, result);
                ((ICoreScope)scope).Complete();
            }
            return userSecurity;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}