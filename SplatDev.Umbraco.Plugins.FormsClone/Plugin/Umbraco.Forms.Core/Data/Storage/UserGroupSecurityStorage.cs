
// Type: Umbraco.Forms.Data.Storage.UserGroupSecurityStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Persistence.Dtos;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Data.Storage
{
  internal sealed class UserGroupSecurityStorage : IUserGroupSecurityStorage
  {
    private readonly IScopeProvider _scopeProvider;

    public UserGroupSecurityStorage(IScopeProvider scopeProvider) => this._scopeProvider = scopeProvider;

    public List<UserGroupSecurity> GetAllUserGroupSecurity(
      params int[] groupIds)
    {
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, true))
        return groupIds.Length == 0 ? scope.Database.Fetch<UserGroupSecurity>() : NPocoDatabaseExtensions.FetchByGroups<UserGroupSecurity, int>((IDatabase) scope.Database, (IEnumerable<int>) groupIds, 2000, (Func<IEnumerable<int>, Sql<ISqlContext>>) (batch => NPocoSqlExtensions.WhereIn<UserGroupSecurity>(NPocoSqlExtensions.From<UserGroupSecurity>(NPocoSqlExtensions.Select<UserGroupSecurity>(new Sql<ISqlContext>(scope.SqlContext), Array.Empty<Expression<Func<UserGroupSecurity, object>>>()), (string) null), (Expression<Func<UserGroupSecurity, object>>) (x => (object) x.UserGroupId), (IEnumerable) batch))).ToList<UserGroupSecurity>();
    }

    public UserGroupSecurity? GetUserGroupSecurity(int groupId)
    {
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, true))
        return scope.Database.Query<UserGroupSecurity>().FirstOrDefault((Expression<Func<UserGroupSecurity, bool>>) (x => x.UserGroupId == groupId));
    }

    public UserGroupSecurity InsertUserGroupSecurity(
      UserGroupSecurity userGroupSecurity)
    {
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, false))
      {
        UserGroupSecurityStorage.SetIndentityInsertForUserGroupSecurityTable(scope, true);
        ((IDatabase) scope.Database).Insert<UserGroupSecurity>(userGroupSecurity);
        UserGroupSecurityStorage.SetIndentityInsertForUserGroupSecurityTable(scope, false);
        ((ICoreScope) scope).Complete();
      }
      return userGroupSecurity;
    }

    private static void SetIndentityInsertForUserGroupSecurityTable(IScope scope, bool value)
    {
      if (scope.SqlContext.IsSqlite())
        return;
      scope.Database.Execute("IF (COLUMNPROPERTY(OBJECT_ID('UFUserGroupSecurity'),'UserGroupId','isidentity') = 1) SET IDENTITY_INSERT UFUserGroupSecurity " + (value ? "ON" : "OFF"));
    }

    public bool DeleteUserGroupSecurity(UserGroupSecurity userGroupSecurity)
    {
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, false))
      {
        ((IDatabase) scope.Database).Delete<UserGroupSecurity>((object) userGroupSecurity);
        ((ICoreScope) scope).Complete();
      }
      return true;
    }

    public UserGroupSecurity UpdateUserGroupSecurity(
      UserGroupSecurity userGroupSecurity)
    {
      using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, (IEventDispatcher) null, (IScopedNotificationPublisher) null, null, false, false))
      {
        ((IDatabase) scope.Database).Update<UserGroupSecurity>("SET manageforms = @0, managedatasources = @1, manageprevaluesources = @2, manageworkflows = @3, viewEntries = @4, editEntries = @5, deleteEntries = @6 WHERE userGroupId = @7", (object) userGroupSecurity.ManageForms, (object) userGroupSecurity.ManageDataSources, (object) userGroupSecurity.ManagePreValueSources, (object) userGroupSecurity.ManageWorkflows, (object) userGroupSecurity.ViewEntries, (object) userGroupSecurity.EditEntries, (object) userGroupSecurity.DeleteEntries, (object) userGroupSecurity.UserGroupId);
        ((ICoreScope) scope).Complete();
      }
      return userGroupSecurity;
    }
  }
}
