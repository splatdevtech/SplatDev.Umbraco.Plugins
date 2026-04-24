using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FormDeletedNotification : DeletedNotification<Form>
    {
        public FormDeletedNotification(Form target, EventMessages messages)
          : base(target, messages)
        {
        }

        public FormDeletedNotification(IEnumerable<Form> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}