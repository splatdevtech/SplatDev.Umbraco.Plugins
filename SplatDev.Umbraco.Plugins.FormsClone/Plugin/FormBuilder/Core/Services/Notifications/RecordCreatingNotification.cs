using FormBuilder.Core.Persistence.Fields;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class RecordCreatingNotification : SavingNotification<Record>
    {
        public RecordCreatingNotification(Record target, EventMessages messages)
          : base(target, messages)
        {
        }

        public RecordCreatingNotification(IEnumerable<Record> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}