using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services;
using FormBuilder.Core.Services.Notifications;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Security;

using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace FormBuilder.Core.Notifications.Handlers
{
    internal sealed class FormCreatedNotificationHandler(
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IUserService userService,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings) :
      INotificationHandler<FormCreatedNotification>,
      INotificationHandler
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        private readonly IUserService _userService = userService;
        private readonly IUserFormSecurityStorage _userFormSecurityStorage = userFormSecurityStorage;
        private readonly IUserGroupFormSecurityStorage _userGroupFormSecurityStorage = userGroupFormSecurityStorage;
        private readonly IOptions<SecuritySettings> _securitySettings = securitySettings;

        public void Handle(FormCreatedNotification notification) => Handle(notification.CreatedEntity.Yield());

        public void Handle(IEnumerable<FormCreatedNotification> notifications) => Handle(notifications.Select(x => x.CreatedEntity));

        private void Handle(IEnumerable<Form> entities)
        {
            IUser? currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser is null)
                return;
            foreach (Guid formId in entities.Select(x => x.Id).Distinct())
                FormServiceSecurityHelper.SetPermissionsForNewForm(formId, currentUser, _userService, _userFormSecurityStorage, _userGroupFormSecurityStorage, _securitySettings.Value.GrantAccessToNewFormsForUserGroups, _securitySettings.Value.ManageSecurityWithUserGroups);
        }
    }
}