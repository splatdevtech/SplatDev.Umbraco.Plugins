
// Type: Umbraco.Forms.Core.Cache.NotificationHandlers.MemberSavedDistributedCacheNotificationHandler
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.Notifications;


#nullable enable
namespace Umbraco.Forms.Core.Cache.NotificationHandlers
{
  internal sealed class MemberSavedDistributedCacheNotificationHandler : 
    SavedDistributedCacheNotificationHandlerBase<IMember, MemberSavedNotification>
  {
    private readonly AppCaches _appCaches;

    public MemberSavedDistributedCacheNotificationHandler(AppCaches appCaches) => this._appCaches = appCaches;

    protected override void Handle(IEnumerable<IMember> entities)
    {
      foreach (IEntity entity in entities)
        this._appCaches.RuntimeCache.ClearByKey(Umbraco.Forms.Core.Cache.CacheKeys.GetMemberCacheKey(entity.Key));
    }
  }
}
