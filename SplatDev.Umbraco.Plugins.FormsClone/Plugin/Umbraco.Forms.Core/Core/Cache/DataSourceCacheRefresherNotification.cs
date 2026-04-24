
// Type: Umbraco.Forms.Core.Cache.DataSourceCacheRefresherNotification
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Sync;


#nullable enable
namespace Umbraco.Forms.Core.Cache
{
  public class DataSourceCacheRefresherNotification : CacheRefresherNotification
  {
    public DataSourceCacheRefresherNotification(object messageObject, MessageType messageType)
      : base(messageObject, messageType)
    {
    }
  }
}
