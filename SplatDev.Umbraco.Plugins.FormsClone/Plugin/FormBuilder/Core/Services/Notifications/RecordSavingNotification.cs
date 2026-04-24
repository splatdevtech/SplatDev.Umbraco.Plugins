using FormBuilder.Core.Persistence.Fields;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class RecordSavingNotification : SavingNotification<Record>
    {
        public RecordSavingNotification(Record target, EventMessages messages)
          : base(target, messages)
        {
        }

        public RecordSavingNotification(IEnumerable<Record> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}