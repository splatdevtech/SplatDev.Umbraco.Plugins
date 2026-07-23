using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.Notifications;

using static FormBuilder.Constants;

namespace FormBuilder.Core.Handlers
{
    internal sealed class MemberSavedDistributedCacheNotificationHandler(AppCaches appCaches) :
        SavedDistributedCacheNotificationHandlerBase<IMember, MemberSavedNotification>
    {
        private readonly AppCaches _appCaches = appCaches;

        protected override void Handle(IEnumerable<IMember> entities)
        {
            foreach (IEntity entity in entities)
                _appCaches.RuntimeCache.ClearByKey(FormBuilderCacheKeys.GetMemberCacheKey(entity.Key));
        }
    }
}