using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public class FormPrePopulateNotification(Form target, EventMessages messages) : ObjectNotification<Form>(target, messages)
    {
        public Form Form => Target;
    }
}