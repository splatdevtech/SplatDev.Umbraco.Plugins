using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FormCreatedNotification(Form target, EventMessages messages) : CreatedNotification<Form>(target, messages)
    {
    }
}