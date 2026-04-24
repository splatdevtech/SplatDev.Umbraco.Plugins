using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public abstract class RecordStateChangeNotification(Record target, EventMessages messages, Form form) : ObjectNotification<Record>(target, messages)
    {
        public Record Record => Target;

        public Form Form { get; } = form;
    }
}