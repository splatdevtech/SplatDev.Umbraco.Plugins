using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Notifications;

using Umbraco.Cms.Core.Cache;

namespace FormBuilder.Core.Handlers
{
    internal sealed class FormDeletedDistributedCacheNotificationHandler(
      IFormsDistributedCache formsDbDistributedCache) :
        DeletedDistributedCacheNotificationHandlerBase<Form, FormDeletedNotification>
    {
        private readonly IFormsDistributedCache _formsDbDistributedCache = formsDbDistributedCache;

        protected override void Handle(IEnumerable<Form> entities) => _formsDbDistributedCache.RemoveForms(entities);
    }
}