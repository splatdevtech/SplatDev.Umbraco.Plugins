using FormBuilder.Core.Prevalues;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class PrevalueSourceDeletingNotification : DeletingNotification<FieldPrevalueSource>
    {
        public PrevalueSourceDeletingNotification(FieldPrevalueSource target, EventMessages messages)
          : base(target, messages)
        {
        }

        public PrevalueSourceDeletingNotification(
          IEnumerable<FieldPrevalueSource> target,
          EventMessages messages)
          : base(target, messages)
        {
        }
    }
}