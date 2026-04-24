
// Type: Umbraco.Forms.Data.Storage.UserFormSecurityStorage
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
    internal sealed class UserFormSecurityStorage : IUserFormSecurityStorage
    {
        private readonly IScopeProvider _scopeProvider;

        public UserFormSecurityStorage(IScopeProvider scopeProvider) => this._scopeProvider = scopeProvider;

        public List<UserFormSecurity> GetAllUserFormSecurity(string userId)
        {
            if (!int.TryParse(userId, NumberStyles.Integer, CultureInfo.InvariantCulture, out int _))
                return new List<UserFormSecurity>();
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Query<UserFormSecurity>().Where((Expression<Func<UserFormSecurity, bool>>)(x => x.User == userId)).ToList();
        }

        public UserFormSecurity? GetUserFormSecurity(string userId, Guid form)
        {
            if (!int.TryParse(userId, NumberStyles.Integer, CultureInfo.InvariantCulture, out int _))
                return null;
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Query<UserFormSecurity>().FirstOrDefault((Expression<Func<UserFormSecurity, bool>>)(x => x.User == userId && x.Form == form));
        }

        public UserFormSecurity InsertUserFormSecurity(UserFormSecurity formSecurity)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                if (this.GetUserFormSecurity(formSecurity.User, formSecurity.Form) == null)
                    scope.Database.Insert<UserFormSecurity>(formSecurity);
                else
                    this.UpdateUserFormSecurity(formSecurity);
                scope.Complete();
                return formSecurity;
            }
        }

        public List<UserFormSecurity> GetUserFormSecurityForAllUsers(Guid form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Fetch<UserFormSecurity>("WHERE form = @0", form);
        }

        public bool DeleteUserFormSecurity(UserFormSecurity formSecurity)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Delete<UserFormSecurity>(formSecurity);
                scope.Complete();
                return true;
            }
        }

        public void DeleteAllUserFormSecurityForForm(Guid form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Delete<UserFormSecurity>("WHERE form = @0", form);
                scope.Complete();
            }
        }

        public void DeleteAllUserFormSecurityForUser(int userId)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Delete<UserFormSecurity>("WHERE [user] = @0", userId.ToString());
                scope.Complete();
            }
        }

        public UserFormSecurity UpdateUserFormSecurity(UserFormSecurity formSecurity)
        {
            int result;
            if (!int.TryParse(formSecurity.User, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return formSecurity;
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Update<UserFormSecurity>("SET HasAccess = @0, SecurityType = @1, AllowInEditor = @2 WHERE [user] = @3 AND form = @4", formSecurity.HasAccess, formSecurity.SecurityType, formSecurity.AllowInEditor, result, formSecurity.Form);
                scope.Complete();
                return formSecurity;
            }
        }
    }
}
