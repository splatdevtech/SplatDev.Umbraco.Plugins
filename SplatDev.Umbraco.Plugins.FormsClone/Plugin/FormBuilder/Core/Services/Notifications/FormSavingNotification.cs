using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FormSavingNotification : SavingNotification<Form>
    {
        public FormSavingNotification(Form target, EventMessages messages)
          : base(target, messages)
        {
        }

        public FormSavingNotification(
          Form target,
          EventMessages messages,
          Dictionary<string, object?> state)
          : base(target, messages)
        {
            State = state;
        }

        public FormSavingNotification(IEnumerable<Form> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}