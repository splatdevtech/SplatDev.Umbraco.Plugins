using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FolderSavingNotification : SavingNotification<Folder>
    {
        public FolderSavingNotification(Folder target, EventMessages messages)
          : base(target, messages)
        {
        }

        public FolderSavingNotification(
          Folder target,
          EventMessages messages,
          Dictionary<string, object?> state)
          : base(target, messages)
        {
            State = state;
        }

        public FolderSavingNotification(IEnumerable<Folder> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}