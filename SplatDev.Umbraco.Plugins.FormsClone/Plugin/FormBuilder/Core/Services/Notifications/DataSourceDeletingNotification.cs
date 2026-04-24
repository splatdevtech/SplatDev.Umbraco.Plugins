using FormBuilder.Core.DataSources;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class DataSourceDeletingNotification : DeletingNotification<FormDataSource>
    {
        public DataSourceDeletingNotification(FormDataSource target, EventMessages messages)
          : base(target, messages)
        {
        }

        public DataSourceDeletingNotification(
          IEnumerable<FormDataSource> target,
          EventMessages messages)
          : base(target, messages)
        {
        }
    }
}