using FormBuilder.Core.DataSources;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class DataSourceSavedNotification : SavedNotification<FormDataSource>
    {
        public DataSourceSavedNotification(FormDataSource target, EventMessages messages)
          : base(target, messages)
        {
        }

        public DataSourceSavedNotification(IEnumerable<FormDataSource> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}