
// Type: Umbraco.Forms.Core.Services.Notifications.PrevalueSourceSavedNotification
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;


#nullable enable
namespace Umbraco.Forms.Core.Services.Notifications
{
  public sealed class PrevalueSourceSavedNotification : SavedNotification<FieldPreValueSource>
  {
    public PrevalueSourceSavedNotification(FieldPreValueSource target, EventMessages messages)
      : base(target, messages)
    {
    }

    public PrevalueSourceSavedNotification(
      IEnumerable<FieldPreValueSource> target,
      EventMessages messages)
      : base(target, messages)
    {
    }
  }
}
