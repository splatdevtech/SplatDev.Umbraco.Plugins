using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Notifications;

using Umbraco.Cms.Core.Cache;

namespace FormBuilder.Core.Handlers
{
    internal sealed class FormSavedDistributedCacheNotificationHandler(
      IFormsDistributedCache formsDbDistributedCache) :
        SavedDistributedCacheNotificationHandlerBase<Form, FormSavedNotification>
    {
        private readonly IFormsDistributedCache _formsDbDistributedCache = formsDbDistributedCache;

        protected override void Handle(IEnumerable<Form> entities) => _formsDbDistributedCache.RefreshForms(entities);
    }
}