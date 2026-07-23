using FormBuilder.Core.Persistence.Fields;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class RecordFetchingNotification(Record target, EventMessages messages) : ObjectNotification<Record>(target, messages)
    {
        public Record Record => Target;
    }
}