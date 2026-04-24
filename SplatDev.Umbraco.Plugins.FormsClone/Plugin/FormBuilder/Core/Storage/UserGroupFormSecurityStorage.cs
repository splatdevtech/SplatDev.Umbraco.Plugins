using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Storage.Interfaces;

using NPoco;

using System.Data;
using System.Linq.Expressions;

using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class UserGroupFormSecurityStorage(IScopeProvider scopeProvider) : IUserGroupFormSecurityStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
          int groupId)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Query<UserGroupFormSecurity>().Where((Expression<Func<UserGroupFormSecurity, bool>>)(x => x.UserGroupId == groupId)).ToList();
        }

        public List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
          params int[] groupIds)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);

            if (groupIds.Length == 0)
            {
                return scope.Database.Query<UserGroupFormSecurity>().ToList();
            }
            else
            {
                return [.. scope.Database.FetchByGroups<UserGroupFormSecurity, int>(groupIds, 2000, batch => new Sql<ISqlContext>(scope.SqlContext).Select<UserGroupFormSecurity>([]).From<UserGroupFormSecurity>(null).WhereIn<UserGroupFormSecurity>(x => x.UserGroupId, batch))];
            }
        }

        public List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
          Guid form,
          params int[] groupIds)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            if (groupIds.Length != 0)
            {
                return [.. scope.Database.FetchByGroups<UserGroupFormSecurity, int>(groupIds, 2000, batch => new Sql<ISqlContext>(scope.SqlContext).Select<UserGroupFormSecurity>([]).From<UserGroupFormSecurity>(null).Where<UserGroupFormSecurity>(x => x.Form == form, null).WhereIn<UserGroupFormSecurity>(x => x.UserGroupId, batch))];
            }
            return scope.Database.Query<UserGroupFormSecurity>().Where((Expression<Func<UserGroupFormSecurity, bool>>)(x => x.Form == form)).ToList();
        }

        public UserGroupFormSecurity? GetUserGroupFormSecurity(
          int groupId,
          Guid form)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Query<UserGroupFormSecurity>().FirstOrDefault((Expression<Func<UserGroupFormSecurity, bool>>)(x => x.UserGroupId == groupId && x.Form == form));
        }

        public UserGroupFormSecurity InsertUserGroupFormSecurity(
          UserGroupFormSecurity formSecurity)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            if (GetUserGroupFormSecurity(formSecurity.UserGroupId, formSecurity.Form) is null)
                scope.Database.Insert(formSecurity);
            else
                UpdateUserGroupFormSecurity(formSecurity);
            ((ICoreScope)scope).Complete();
            return formSecurity;
        }

        public List<UserGroupFormSecurity> GetUserGroupFormSecurityForAllUsers(
          Guid form)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Fetch<UserGroupFormSecurity>("WHERE form = @0", form);
        }

        public bool DeleteUserGroupFormSecurity(UserGroupFormSecurity formSecurity)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserGroupFormSecurity>(formSecurity);
            ((ICoreScope)scope).Complete();
            return true;
        }

        public void DeleteAllUserGroupFormSecurityForForm(Guid form)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserGroupFormSecurity>("WHERE form = @0", form);
        }

        public void DeleteAllUserGroupFormSecurityForUserGroup(int userGroupId)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserGroupFormSecurity>("WHERE userGroupId = @0", userGroupId);
            ((ICoreScope)scope).Complete();
        }

        public UserGroupFormSecurity UpdateUserGroupFormSecurity(
          UserGroupFormSecurity formSecurity)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Update<UserGroupFormSecurity>("SET HasAccess = @0, SecurityType = @1, AllowInEditor = @2 WHERE userGroupId = @3 AND form = @4", formSecurity.HasAccess, formSecurity.SecurityType, formSecurity.AllowInEditor, formSecurity.UserGroupId, formSecurity.Form);
            ((ICoreScope)scope).Complete();
            return formSecurity;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}