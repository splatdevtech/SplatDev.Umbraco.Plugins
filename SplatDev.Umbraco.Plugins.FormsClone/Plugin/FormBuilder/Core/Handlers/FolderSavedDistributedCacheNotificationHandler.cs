using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Notifications;

using Umbraco.Cms.Core.Cache;

namespace FormBuilder.Core.Handlers
{
    internal sealed class FolderSavedDistributedCacheNotificationHandler(
      IFormsDistributedCache formsDbDistributedCache) :
        SavedDistributedCacheNotificationHandlerBase<Folder, FolderSavedNotification>
    {
        private readonly IFormsDistributedCache _formsDbDistributedCache = formsDbDistributedCache;

        protected override void Handle(IEnumerable<Folder> entities) => _formsDbDistributedCache.RefreshFolders(entities);
    }
}