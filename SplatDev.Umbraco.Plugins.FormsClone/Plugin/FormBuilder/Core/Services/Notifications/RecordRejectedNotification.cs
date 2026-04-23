using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

using Umbraco.Cms.Core.Events;

namespace FormBuilder.Core.Services.Notifications
{
    public class RecordRejectedNotification(Record target, EventMessages messages, Form form) : RecordStateChangeNotification(target, messages, form)
    {
    }
}