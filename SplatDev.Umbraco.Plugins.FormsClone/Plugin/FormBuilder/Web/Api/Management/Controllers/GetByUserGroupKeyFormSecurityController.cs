using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Security;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Services;

using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single set of form security details for a user group.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "SectionAccessUsers")]
    public class GetByUserGroupKeyFormSecurityController(
      IFormService formService,
      IFolderService folderService,
      IUserGroupSecurityStorage userGroupSecurityStorage,
      IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
      IOptions<SecuritySettings> securitySettings,
      IUserGroupService userGroupService,
      IUserGroupStartFolderStorage userGroupStartFolderStorage) : FormUserGroupSecurityControllerBase(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userGroupService, userGroupStartFolderStorage)
    {
        /// <summary>
        /// Management API endpoint for retrieving a single set of form security details for a user by user Id.
        /// </summary>
        [HttpGet("{id:guid}/form-security")]
        [ProducesResponseType(typeof(FormSecurityForGroup), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByUserGroupKey(Guid id)
        {
            GetByUserGroupKeyFormSecurityController securityController = this;
            var async = await securityController.UserGroupService.GetAsync(id);
            if (async is null)
                return securityController.NotFound();
            FormSecurityForGroup formSecurity = new()
            {
                Key = id,
                Name = async.Name ?? "(user group)"
            };
            securityController.ApplyUserGroupSecurity(formSecurity, async.Id);
            securityController.ApplyPerFormSecurity(formSecurity, async);
            securityController.ApplyStartFolders(formSecurity, async.Id);
            return securityController.Ok(formSecurity);
        }

        private void ApplyUserGroupSecurity(FormSecurityForGroup formSecurity, int groupId)
        {
            var userGroupSecurity = UserGroupSecurityStorage.GetUserGroupSecurity(groupId);
            if (userGroupSecurity is not null)
            {
                formSecurity.UserGroupSecurity = userGroupSecurity;
            }
            else
            {
                UserGroupSecurity usersecurity = UserGroupSecurity.Create();
                usersecurity.UserGroupId = groupId;
                UserGroupSecurityStorage.InsertUserGroupSecurity(usersecurity);
                formSecurity.UserGroupSecurity = usersecurity;
            }
        }

        private void ApplyPerFormSecurity(FormSecurityForGroup formSecurity, IUserGroup userGroup)
        {
            List<UserGroupFormSecurity> groupFormSecurity1 = UserGroupFormSecurityStorage.GetAllUserGroupFormSecurity(userGroup.Id);
            IEnumerable<Form> source = FormService.Get();
            foreach (Form form1 in source)
            {
                Form form = form1;
                string formFieldSummary = form.GetFormFieldSummary();
                var groupFormSecurity2 = groupFormSecurity1.SingleOrDefault(x => x.Form == form.Id);
                if (groupFormSecurity2 is not null)
                {
                    groupFormSecurity2.FormName = form.Name;
                    groupFormSecurity2.FormCreated = form.Created;
                    groupFormSecurity2.Fields = formFieldSummary;
                }
                if (groupFormSecurity2 is null)
                {
                    bool flag = false;
                    string formsForUserGroups = SecuritySettings.GrantAccessToNewFormsForUserGroups;
                    if (!string.IsNullOrEmpty(formsForUserGroups))
                    {
                        if (formsForUserGroups.Split(
                        [
                            ','
                        ], StringSplitOptions.RemoveEmptyEntries).InvariantContains(userGroup.Alias))
                        {
                            flag = true;
                        }
                    }
                    UserGroupFormSecurity groupFormSecurity3 = new()
                    {
                        Form = form.Id,
                        HasAccess = flag,
                        UserGroupId = userGroup.Id,
                        FormName = form.Name,
                        FormCreated = form.Created,
                        Fields = formFieldSummary,
                        SecurityType = FormSecurityType.Full,
                        AllowInEditor = true
                    };
                    UserGroupFormSecurity formSecurity1 = groupFormSecurity3;
                    UserGroupFormSecurityStorage.InsertUserGroupFormSecurity(formSecurity1);
                    groupFormSecurity1.Add(formSecurity1);
                }
            }
            List<UserGroupFormSecurity> groupFormSecurityList = [];
            foreach (UserGroupFormSecurity groupFormSecurity4 in groupFormSecurity1)
            {
                UserGroupFormSecurity formSecurityDbEntry = groupFormSecurity4;
                if (source.SingleOrDefault(x => x.Id == formSecurityDbEntry.Form) is null)
                    groupFormSecurityList.Add(formSecurityDbEntry);
            }
            foreach (UserGroupFormSecurity formSecurity2 in groupFormSecurityList)
            {
                groupFormSecurity1.Remove(formSecurity2);
                UserGroupFormSecurityStorage.DeleteUserGroupFormSecurity(formSecurity2);
            }
            formSecurity.FormsSecurity = [.. groupFormSecurity1.OrderBy(x => x.FormName)];
        }

        private void ApplyStartFolders(FormSecurityForGroup formSecurity, int userGroupId) => formSecurity.StartFolderIds = [.. UserGroupStartFolderStorage.GetStartFolderKeys(userGroupId)];
    }
}