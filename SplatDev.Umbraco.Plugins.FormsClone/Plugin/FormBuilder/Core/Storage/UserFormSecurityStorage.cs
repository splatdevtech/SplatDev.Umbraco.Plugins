using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Storage.Interfaces;

using System.Data;
using System.Globalization;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class UserFormSecurityStorage(IScopeProvider scopeProvider) : IUserFormSecurityStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public List<UserFormSecurity> GetAllUserFormSecurity(string userId)
        {
            if (!int.TryParse(userId, NumberStyles.Integer, CultureInfo.InvariantCulture, out int _))
                return [];
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Query<UserFormSecurity>().Where((Expression<Func<UserFormSecurity, bool>>)(x => x.User == userId)).ToList();
        }

        public UserFormSecurity? GetUserFormSecurity(string userId, Guid form)
        {
            if (!int.TryParse(userId, NumberStyles.Integer, CultureInfo.InvariantCulture, out int _))
                return null;
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Query<UserFormSecurity>().FirstOrDefault((Expression<Func<UserFormSecurity, bool>>)(x => x.User == userId && x.Form == form));
        }

        public UserFormSecurity InsertUserFormSecurity(UserFormSecurity formSecurity)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            if (GetUserFormSecurity(formSecurity.User, formSecurity.Form) == null)
                _ = scope.Database.Insert(formSecurity);
            else
                UpdateUserFormSecurity(formSecurity);
            ((ICoreScope)scope).Complete();
            return formSecurity;
        }

        public List<UserFormSecurity> GetUserFormSecurityForAllUsers(Guid form)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Fetch<UserFormSecurity>("WHERE form = @0", form);
        }

        public bool DeleteUserFormSecurity(UserFormSecurity formSecurity)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserFormSecurity>(formSecurity);
            ((ICoreScope)scope).Complete();
            return true;
        }

        public void DeleteAllUserFormSecurityForForm(Guid form)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserFormSecurity>("WHERE form = @0", form);
            ((ICoreScope)scope).Complete();
        }

        public void DeleteAllUserFormSecurityForUser(int userId)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserFormSecurity>("WHERE [user] = @0", userId.ToString());
            ((ICoreScope)scope).Complete();
        }

        public UserFormSecurity UpdateUserFormSecurity(UserFormSecurity formSecurity)
        {
            if (!int.TryParse(formSecurity.User, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                return formSecurity;
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Update<UserFormSecurity>("SET HasAccess = @0, SecurityType = @1, AllowInEditor = @2 WHERE [user] = @3 AND form = @4", formSecurity.HasAccess, formSecurity.SecurityType, formSecurity.AllowInEditor, result, formSecurity.Form);
            ((ICoreScope)scope).Complete();
            return formSecurity;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}