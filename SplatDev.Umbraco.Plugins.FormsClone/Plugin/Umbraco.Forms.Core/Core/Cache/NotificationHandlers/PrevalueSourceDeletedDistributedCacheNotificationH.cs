
// Type: Umbraco.Forms.Core.Cache.NotificationHandlers.PrevalueSourceDeletedDistributedCacheNotificationHandler
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Cms.Core.Cache;
using Umbraco.Forms.Core.Services.Notifications;


#nullable enable
namespace Umbraco.Forms.Core.Cache.NotificationHandlers
{
  internal sealed class PrevalueSourceDeletedDistributedCacheNotificationHandler : 
    DeletedDistributedCacheNotificationHandlerBase<FieldPreValueSource, PrevalueSourceDeletedNotification>
  {
    private readonly IFormsDistributedCache _formsDbDistributedCache;

    public PrevalueSourceDeletedDistributedCacheNotificationHandler(
      IFormsDistributedCache formsDbDistributedCache)
    {
      this._formsDbDistributedCache = formsDbDistributedCache;
    }

    protected override void Handle(IEnumerable<FieldPreValueSource> entities) => this._formsDbDistributedCache.RemovePrevalueSources(entities);
  }
}
