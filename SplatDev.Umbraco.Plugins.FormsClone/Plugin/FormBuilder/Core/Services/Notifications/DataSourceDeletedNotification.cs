using FormBuilder.Core.DataSources;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class DataSourceDeletedNotification : DeletedNotification<FormDataSource>
    {
        public DataSourceDeletedNotification(FormDataSource target, EventMessages messages)
          : base(target, messages)
        {
        }

        public DataSourceDeletedNotification(IEnumerable<FormDataSource> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}