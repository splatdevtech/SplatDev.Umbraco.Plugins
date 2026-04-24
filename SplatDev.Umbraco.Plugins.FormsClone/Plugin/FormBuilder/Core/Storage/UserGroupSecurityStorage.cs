using FormBuilder.Core.Extensions;
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

    internal sealed class UserGroupSecurityStorage(IScopeProvider scopeProvider) : IUserGroupSecurityStorage
    {
        private readonly IScopeProvider _scopeProvider = scopeProvider;

        public List<UserGroupSecurity> GetAllUserGroupSecurity(
          params int[] groupIds)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return groupIds.Length == 0 ? scope.Database.Fetch<UserGroupSecurity>() : [.. scope.Database.FetchByGroups<UserGroupSecurity, int>(groupIds, 2000, batch => new Sql<ISqlContext>(scope.SqlContext).Select<UserGroupSecurity>([]).From<UserGroupSecurity>(null).WhereIn<UserGroupSecurity>(x => x.UserGroupId, batch))];
        }

        public UserGroupSecurity? GetUserGroupSecurity(int groupId)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            return scope.Database.Query<UserGroupSecurity>().FirstOrDefault((Expression<Func<UserGroupSecurity, bool>>)(x => x.UserGroupId == groupId));
        }

        public UserGroupSecurity InsertUserGroupSecurity(
          UserGroupSecurity userGroupSecurity)
        {
            using (IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                SetIndentityInsertForUserGroupSecurityTable(scope, true);
                scope.Database.Insert(userGroupSecurity);
                SetIndentityInsertForUserGroupSecurityTable(scope, false);
                ((ICoreScope)scope).Complete();
            }
            return userGroupSecurity;
        }

        private static void SetIndentityInsertForUserGroupSecurityTable(IScope scope, bool value)
        {
            if (scope.SqlContext.IsSqlite())
                return;
            scope.Database.Execute("IF (COLUMNPROPERTY(OBJECT_ID('UFUserGroupSecurity'),'UserGroupId','isidentity') = 1) SET IDENTITY_INSERT FormBuilderUserGroupSecurity " + (value ? "ON" : "OFF"));
        }

        public bool DeleteUserGroupSecurity(UserGroupSecurity userGroupSecurity)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            scope.Database.Delete<UserGroupSecurity>(userGroupSecurity);
            ((ICoreScope)scope).Complete();
            return true;
        }

        public UserGroupSecurity UpdateUserGroupSecurity(
          UserGroupSecurity userGroupSecurity)
        {
            using (IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                _ = scope.Database.Update<UserGroupSecurity>("SET manageforms = @0, managedatasources = @1, manageprevaluesources = @2, manageworkflows = @3, viewEntries = @4, editEntries = @5, deleteEntries = @6 WHERE userGroupId = @7", userGroupSecurity.ManageForms, userGroupSecurity.ManageDataSources, userGroupSecurity.ManagePreValueSources, userGroupSecurity.ManageWorkflows, userGroupSecurity.ViewEntries, userGroupSecurity.EditEntries, userGroupSecurity.DeleteEntries, userGroupSecurity.UserGroupId);
                ((ICoreScope)scope).Complete();
            }
            return userGroupSecurity;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}