using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FormSavedNotification : SavedNotification<Form>
    {
        public FormSavedNotification(Form target, EventMessages messages)
          : base(target, messages)
        {
        }

        public FormSavedNotification(IEnumerable<Form> target, EventMessages messages)
          : base(target, messages)
        {
        }

        public FormSavedNotification(Form target, Guid? movedFromParentId, EventMessages messages)
          : base(target, messages)
        {
            MovedFromParentId = movedFromParentId;
        }

        public Guid? MovedFromParentId { get; set; }
    }
}