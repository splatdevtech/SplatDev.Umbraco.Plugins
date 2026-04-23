
// Type: Umbraco.Forms.Core.NotificationHandlers.FormDeletingNotificationHandler
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Core.Services.Notifications;


#nullable enable
namespace Umbraco.Forms.Core.NotificationHandlers
{
  internal sealed class FormDeletingNotificationHandler : 
    INotificationHandler<FormDeletingNotification>,
    INotificationHandler
  {
    private readonly IWorkflowService _workflowService;

    public FormDeletingNotificationHandler(IWorkflowService workflowService) => this._workflowService = workflowService;

    public void Handle(FormDeletingNotification notification) => this.Handle(notification.DeletedEntities);

    public void Handle(
      IEnumerable<FormDeletingNotification> notifications)
    {
      this.Handle(notifications.SelectMany<FormDeletingNotification, Form>((Func<FormDeletingNotification, IEnumerable<Form>>) (x => x.DeletedEntities)));
    }

    private void Handle(IEnumerable<Form> entities)
    {
      foreach (Form entity in entities)
        this._workflowService.Delete(entity);
    }
  }
}
