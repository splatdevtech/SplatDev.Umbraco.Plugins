
// Type: Umbraco.Forms.Core.Services.Notifications.RecordDeletingNotification
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Services.Notifications
{
  public sealed class RecordDeletingNotification : DeletingNotification<Record>
  {
    public RecordDeletingNotification(Record target, EventMessages messages)
      : base(target, messages)
    {
    }

    public RecordDeletingNotification(IEnumerable<Record> target, EventMessages messages)
      : base(target, messages)
    {
    }
  }
}
