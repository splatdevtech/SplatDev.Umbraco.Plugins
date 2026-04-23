using FormBuilder.Core.Prevalues;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class PrevalueSourceCreatingNotification(FieldPrevalueSource target, EventMessages messages) : CreatingNotification<FieldPrevalueSource>(target, messages)
    {
    }
}