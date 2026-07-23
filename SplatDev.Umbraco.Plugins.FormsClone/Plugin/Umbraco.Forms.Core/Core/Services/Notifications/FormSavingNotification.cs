
// Type: Umbraco.Forms.Core.Services.Notifications.FormSavingNotification
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Services.Notifications
{
  public sealed class FormSavingNotification : SavingNotification<Form>
  {
    public FormSavingNotification(Form target, EventMessages messages)
      : base(target, messages)
    {
    }

    public FormSavingNotification(
      Form target,
      EventMessages messages,
      Dictionary<string, object?> state)
      : base(target, messages)
    {
      this.State = (IDictionary<string, object>) state;
    }

    public FormSavingNotification(IEnumerable<Form> target, EventMessages messages)
      : base(target, messages)
    {
    }
  }
}
