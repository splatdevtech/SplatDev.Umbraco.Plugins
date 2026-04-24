using FormBuilder.Core.Prevalues;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class PrevalueSourceCreatedNotification(FieldPrevalueSource target, EventMessages messages) : CreatedNotification<FieldPrevalueSource>(target, messages)
    {
    }
}