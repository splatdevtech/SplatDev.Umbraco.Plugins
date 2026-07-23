using FormBuilder.Core.DataSources;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class DataSourceSavingNotification : SavingNotification<FormDataSource>
    {
        public DataSourceSavingNotification(FormDataSource target, EventMessages messages)
          : base(target, messages)
        {
        }

        public DataSourceSavingNotification(IEnumerable<FormDataSource> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}