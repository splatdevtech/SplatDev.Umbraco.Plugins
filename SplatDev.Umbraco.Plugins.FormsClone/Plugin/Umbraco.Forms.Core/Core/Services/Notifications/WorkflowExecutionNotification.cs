
// Type: Umbraco.Forms.Core.Services.Notifications.WorkflowExecutionNotification
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Services.Notifications
{
  public abstract class WorkflowExecutionNotification : ObjectNotification<Workflow>
  {
    protected WorkflowExecutionNotification(Workflow target, Record record, EventMessages messages)
      : base(target, messages)
    {
      this.Record = record;
    }

    public Workflow Workflow => this.Target;

    public Record Record { get; }
  }
}
