
// Type: Umbraco.Forms.Core.Services.Notifications.FormSavedNotification
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Services.Notifications
{
  public sealed class FormSavedNotification : SavedNotification<Form>
  {
    public FormSavedNotification(Form target, EventMessages messages)
      : base(target, messages)
    {
    }

    public FormSavedNotification(IEnumerable<Form> target, EventMessages messages)
      : base(target, messages)
    {
    }

    public FormSavedNotification(Form target, Guid? movedFromParentId, EventMessages messages)
      : base(target, messages)
    {
      this.MovedFromParentId = movedFromParentId;
    }

    public Guid? MovedFromParentId { get; set; }
  }
}
