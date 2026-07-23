
// Type: Umbraco.Forms.Core.NotificationHandlers.FormCreatedNotificationHandler
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Core.Services.Notifications;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Core.NotificationHandlers
{
  internal sealed class FormCreatedNotificationHandler : 
    INotificationHandler<FormCreatedNotification>,
    INotificationHandler
  {
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
    private readonly IUserService _userService;
    private readonly IUserFormSecurityStorage _userFormSecurityStorage;
    private readonly IUserGroupFormSecurityStorage _userGroupFormSecurityStorage;
    private readonly IOptions<SecuritySettings> _securitySettings;

    public FormCreatedNotificationHandler(
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IUserService userService,
      IUserFormSecurityStorage userFormSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings)
    {
      this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
      this._userService = userService;
      this._userFormSecurityStorage = userFormSecurityStorage;
      this._userGroupFormSecurityStorage = userGroupFormSecurityStorage;
      this._securitySettings = securitySettings;
    }

    public void Handle(FormCreatedNotification notification) => this.Handle(notification.CreatedEntity.Yield<Form>());

    public void Handle(IEnumerable<FormCreatedNotification> notifications) => this.Handle(notifications.Select<FormCreatedNotification, Form>((Func<FormCreatedNotification, Form>) (x => x.CreatedEntity)));

    private void Handle(IEnumerable<Form> entities)
    {
      IUser currentUser = this._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
      if (currentUser == null)
        return;
      foreach (Guid formId in entities.Select<Form, Guid>((Func<Form, Guid>) (x => x.Id)).Distinct<Guid>())
        FormServiceSecurityHelper.SetPermissionsForNewForm(formId, currentUser, this._userService, this._userFormSecurityStorage, this._userGroupFormSecurityStorage, this._securitySettings.Value.GrantAccessToNewFormsForUserGroups, this._securitySettings.Value.ManageSecurityWithUserGroups);
    }
  }
}
