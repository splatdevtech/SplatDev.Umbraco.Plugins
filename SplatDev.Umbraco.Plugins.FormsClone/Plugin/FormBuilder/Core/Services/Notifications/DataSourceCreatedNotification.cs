using FormBuilder.Core.DataSources;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class DataSourceCreatedNotification(FormDataSource target, EventMessages messages) : CreatedNotification<FormDataSource>(target, messages)
    {
    }
}