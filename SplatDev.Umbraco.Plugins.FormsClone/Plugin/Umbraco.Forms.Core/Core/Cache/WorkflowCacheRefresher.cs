
// Type: Umbraco.Forms.Core.Cache.WorkflowCacheRefresher
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Cache
{
  internal sealed class WorkflowCacheRefresher : 
    PayloadCacheRefresherBase<WorkflowCacheRefresherNotification, JsonPayloads.WorkflowPayload>
  {
    private readonly IRuntimeState _runtimeState;

    public WorkflowCacheRefresher(
      AppCaches appCache,
      IJsonSerializer serializer,
      IEventAggregator eventAggregator,
      ICacheRefresherNotificationFactory factory,
      IRuntimeState runtimeState)
      : base(appCache, serializer, eventAggregator, factory)
    {
      this._runtimeState = runtimeState;
    }

    public override Guid RefresherUniqueId => CacheKeys.WorkflowDbCacheRefresherGuid;

    public override string Name => "Umbraco Forms Workflow Cache Refresher";
  }
}
