
// Type: Umbraco.Forms.Core.Cache.FormCacheRefresher
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Cache
{
  internal sealed class FormCacheRefresher : 
    PayloadCacheRefresherBase<FormCacheRefresherNotification, JsonPayloads.FormPayload>
  {
    public FormCacheRefresher(
      AppCaches appCache,
      IJsonSerializer serializer,
      IEventAggregator eventAggregator,
      ICacheRefresherNotificationFactory factory)
      : base(appCache, serializer, eventAggregator, factory)
    {
    }

    public override Guid RefresherUniqueId => CacheKeys.FormsDbCacheRefresherGuid;

    public override string Name => "Umbraco Forms Cache Refresher";

    public override void Refresh(JsonPayloads.FormPayload[] payload)
    {
      this.AppCaches.RuntimeCache.Clear("Forms.FormStorage.All");
      base.Refresh(payload);
    }
  }
}
