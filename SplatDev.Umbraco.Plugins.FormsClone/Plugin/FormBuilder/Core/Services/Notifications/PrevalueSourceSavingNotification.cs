using FormBuilder.Core.Prevalues;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class PrevalueSourceSavingNotification : SavingNotification<FieldPrevalueSource>
    {
        public PrevalueSourceSavingNotification(FieldPrevalueSource target, EventMessages messages)
          : base(target, messages)
        {
        }

        public PrevalueSourceSavingNotification(
          IEnumerable<FieldPrevalueSource> target,
          EventMessages messages)
          : base(target, messages)
        {
        }
    }
}