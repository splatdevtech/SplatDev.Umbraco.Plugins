using FormBuilder.Core.Prevalues;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class PrevalueSourceSavedNotification : SavedNotification<FieldPrevalueSource>
    {
        public PrevalueSourceSavedNotification(FieldPrevalueSource target, EventMessages messages)
          : base(target, messages)
        {
        }

        public PrevalueSourceSavedNotification(
          IEnumerable<FieldPrevalueSource> target,
          EventMessages messages)
          : base(target, messages)
        {
        }
    }
}