using FormBuilder.Core.Persistence.Fields;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class RecordDeletingNotification : DeletingNotification<Record>
    {
        public RecordDeletingNotification(Record target, EventMessages messages)
          : base(target, messages)
        {
        }

        public RecordDeletingNotification(IEnumerable<Record> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}