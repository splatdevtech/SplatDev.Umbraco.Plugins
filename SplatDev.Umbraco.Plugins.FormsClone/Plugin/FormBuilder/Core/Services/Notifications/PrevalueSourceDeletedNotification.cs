using FormBuilder.Core.Prevalues;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class PrevalueSourceDeletedNotification : DeletedNotification<FieldPrevalueSource>
    {
        public PrevalueSourceDeletedNotification(FieldPrevalueSource target, EventMessages messages)
          : base(target, messages)
        {
        }

        public PrevalueSourceDeletedNotification(
          IEnumerable<FieldPrevalueSource> target,
          EventMessages messages)
          : base(target, messages)
        {
        }
    }
}