using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FolderSavedNotification : SavedNotification<Folder>
    {
        public FolderSavedNotification(Folder target, EventMessages messages)
          : base(target, messages)
        {
        }

        public FolderSavedNotification(IEnumerable<Folder> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}