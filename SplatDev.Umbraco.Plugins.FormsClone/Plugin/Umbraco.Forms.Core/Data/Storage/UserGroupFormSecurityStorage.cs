
// Type: Umbraco.Forms.Data.Storage.UserGroupFormSecurityStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using System.Data;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Persistence.Dtos;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Data.Storage
{
    internal sealed class UserGroupFormSecurityStorage : IUserGroupFormSecurityStorage
    {
        private readonly IScopeProvider _scopeProvider;

        public UserGroupFormSecurityStorage(IScopeProvider scopeProvider) => this._scopeProvider = scopeProvider;

        public List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
          int groupId)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Query<UserGroupFormSecurity>().Where((Expression<Func<UserGroupFormSecurity, bool>>)(x => x.UserGroupId == groupId)).ToList();
        }

        public List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
          params int[] groupIds)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return groupIds.Length == 0 ? scope.Database.Query<UserGroupFormSecurity>().ToList() : NPocoDatabaseExtensions.FetchByGroups<UserGroupFormSecurity, int>(scope.Database, groupIds, 2000, batch => NPocoSqlExtensions.WhereIn<UserGroupFormSecurity>(NPocoSqlExtensions.From<UserGroupFormSecurity>(NPocoSqlExtensions.Select<UserGroupFormSecurity>(new Sql<ISqlContext>(scope.SqlContext), Array.Empty<Expression<Func<UserGroupFormSecurity, object>>>()), null), x => x.UserGroupId, batch)).ToList<UserGroupFormSecurity>();
        }

        public List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
          Guid form,
          params int[] groupIds)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
            {
                if (groupIds.Length != 0)
                {
                    // ISSUE: reference to a compiler-generated field
                    return NPocoDatabaseExtensions.FetchByGroups<UserGroupFormSecurity, int>(scope.Database, groupIds, 2000, batch => NPocoSqlExtensions.WhereIn<UserGroupFormSecurity>(NPocoSqlExtensions.Where<UserGroupFormSecurity>(NPocoSqlExtensions.From<UserGroupFormSecurity>(NPocoSqlExtensions.Select<UserGroupFormSecurity>(new Sql<ISqlContext>(scope.SqlContext), Array.Empty<Expression<Func<UserGroupFormSecurity, object>>>()), null), x => x.Form == form, null), x => x.UserGroupId, batch)).ToList<UserGroupFormSecurity>();
                }
                return scope.Database.Query<UserGroupFormSecurity>().Where((Expression<Func<UserGroupFormSecurity, bool>>)(x => x.Form == form)).ToList();
            }
        }

        public UserGroupFormSecurity? GetUserGroupFormSecurity(
          int groupId,
          Guid form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Query<UserGroupFormSecurity>().FirstOrDefault((Expression<Func<UserGroupFormSecurity, bool>>)(x => x.UserGroupId == groupId && x.Form == form));
        }

        public UserGroupFormSecurity InsertUserGroupFormSecurity(
          UserGroupFormSecurity formSecurity)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                if (this.GetUserGroupFormSecurity(formSecurity.UserGroupId, formSecurity.Form) == null)
                    scope.Database.Insert<UserGroupFormSecurity>(formSecurity);
                else
                    this.UpdateUserGroupFormSecurity(formSecurity);
                scope.Complete();
                return formSecurity;
            }
        }

        public List<UserGroupFormSecurity> GetUserGroupFormSecurityForAllUsers(
          Guid form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                return scope.Database.Fetch<UserGroupFormSecurity>("WHERE form = @0", form);
        }

        public bool DeleteUserGroupFormSecurity(UserGroupFormSecurity formSecurity)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Delete<UserGroupFormSecurity>(formSecurity);
                scope.Complete();
                return true;
            }
        }

        public void DeleteAllUserGroupFormSecurityForForm(Guid form)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                scope.Database.Delete<UserGroupFormSecurity>("WHERE form = @0", form);
        }

        public void DeleteAllUserGroupFormSecurityForUserGroup(int userGroupId)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Delete<UserGroupFormSecurity>("WHERE userGroupId = @0", userGroupId);
                scope.Complete();
            }
        }

        public UserGroupFormSecurity UpdateUserGroupFormSecurity(
          UserGroupFormSecurity formSecurity)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                scope.Database.Update<UserGroupFormSecurity>("SET HasAccess = @0, SecurityType = @1, AllowInEditor = @2 WHERE userGroupId = @3 AND form = @4", formSecurity.HasAccess, formSecurity.SecurityType, formSecurity.AllowInEditor, formSecurity.UserGroupId, formSecurity.Form);
                scope.Complete();
                return formSecurity;
            }
        }
    }
}
