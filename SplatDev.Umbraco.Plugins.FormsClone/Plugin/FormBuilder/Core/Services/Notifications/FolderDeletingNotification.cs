using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FolderDeletingNotification : DeletingNotification<Folder>
    {
        public FolderDeletingNotification(Folder target, EventMessages messages)
          : base(target, messages)
        {
        }

        public FolderDeletingNotification(IEnumerable<Folder> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}