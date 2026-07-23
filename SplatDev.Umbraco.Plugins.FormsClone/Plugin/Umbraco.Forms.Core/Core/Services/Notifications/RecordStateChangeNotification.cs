
// Type: Umbraco.Forms.Core.Services.Notifications.RecordStateChangeNotification
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Services.Notifications
{
  public abstract class RecordStateChangeNotification : ObjectNotification<Record>
  {
    protected RecordStateChangeNotification(Record target, EventMessages messages, Form form)
      : base(target, messages)
    {
      this.Form = form;
    }

    public Record Record => this.Target;

    public Form Form { get; }
  }
}
