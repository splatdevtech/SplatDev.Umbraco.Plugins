using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FolderCreatedNotification(Folder target, EventMessages messages) : CreatedNotification<Folder>(target, messages)
    {
    }
}