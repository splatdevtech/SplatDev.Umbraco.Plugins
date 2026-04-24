using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FormDeletingNotification : DeletingNotification<Form>
    {
        public FormDeletingNotification(Form target, EventMessages messages)
          : base(target, messages)
        {
        }

        public FormDeletingNotification(IEnumerable<Form> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}