using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FormCreatingNotification(Form target, EventMessages messages) : CreatingNotification<Form>(target, messages)
    {
    }
}