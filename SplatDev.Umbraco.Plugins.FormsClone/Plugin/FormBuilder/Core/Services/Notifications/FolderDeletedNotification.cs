using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FolderDeletedNotification : DeletedNotification<Folder>
    {
        public FolderDeletedNotification(Folder target, EventMessages messages)
          : base(target, messages)
        {
        }

        public FolderDeletedNotification(IEnumerable<Folder> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}