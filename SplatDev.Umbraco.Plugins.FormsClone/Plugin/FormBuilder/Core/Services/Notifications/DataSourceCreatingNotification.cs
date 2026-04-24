using FormBuilder.Core.DataSources;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class DataSourceCreatingNotification(FormDataSource target, EventMessages messages) : CreatingNotification<FormDataSource>(target, messages)
    {
    }
}