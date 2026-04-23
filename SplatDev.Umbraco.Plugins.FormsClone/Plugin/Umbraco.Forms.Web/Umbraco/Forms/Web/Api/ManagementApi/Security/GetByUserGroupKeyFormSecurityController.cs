
// Type: Umbraco.Forms.Web.Api.ManagementApi.Security.GetByUserGroupKeyFormSecurityController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Security
{
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "SectionAccessUsers")]
    public class GetByUserGroupKeyFormSecurityController : FormUserGroupSecurityControllerBase
    {
        public GetByUserGroupKeyFormSecurityController(
          IFormService formService,
          IFolderService folderService,
          IUserGroupSecurityStorage userGroupSecurityStorage,
          IUserGroupFormSecurityStorage userGroupFormSecurityStorage,
          IOptions<SecuritySettings> securitySettings,
          IUserGroupService userGroupService,
          IUserGroupStartFolderStorage userGroupStartFolderStorage)
          : base(formService, folderService, userGroupSecurityStorage, userGroupFormSecurityStorage, securitySettings, userGroupService, userGroupStartFolderStorage)
        {
        }

        [HttpGet("{id:guid}/form-security")]
        [ProducesResponseType(typeof(FormSecurityForGroup), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByUserGroupKey(Guid id)
        {
            GetByUserGroupKeyFormSecurityController securityController = this;
            IUserGroup async = await securityController.UserGroupService.GetAsync(id);
            if (async == null)
                return securityController.NotFound();
            FormSecurityForGroup formSecurity = new FormSecurityForGroup()
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
            UserGroupSecurity userGroupSecurity = this.UserGroupSecurityStorage.GetUserGroupSecurity(groupId);
            if (userGroupSecurity != null)
            {
                formSecurity.UserGroupSecurity = userGroupSecurity;
            }
            else
            {
                UserGroupSecurity usersecurity = UserGroupSecurity.Create();
                usersecurity.UserGroupId = groupId;
                this.UserGroupSecurityStorage.InsertUserGroupSecurity(usersecurity);
                formSecurity.UserGroupSecurity = usersecurity;
            }
        }

        private void ApplyPerFormSecurity(FormSecurityForGroup formSecurity, IUserGroup userGroup)
        {
            List<UserGroupFormSecurity> groupFormSecurity1 = this.UserGroupFormSecurityStorage.GetAllUserGroupFormSecurity(userGroup.Id);
            IEnumerable<Umbraco.Forms.Core.Models.Form> source = this.FormService.Get();
            foreach (Umbraco.Forms.Core.Models.Form form1 in source)
            {
                Umbraco.Forms.Core.Models.Form form = form1;
                string formFieldSummary = form.GetFormFieldSummary();
                UserGroupFormSecurity groupFormSecurity2 = groupFormSecurity1.SingleOrDefault<UserGroupFormSecurity>(x => x.Form == form.Id);
                if (groupFormSecurity2 != null)
                {
                    groupFormSecurity2.FormName = form.Name;
                    groupFormSecurity2.FormCreated = form.Created;
                    groupFormSecurity2.Fields = formFieldSummary;
                }
                if (groupFormSecurity2 == null)
                {
                    bool flag = false;
                    string formsForUserGroups = this.SecuritySettings.GrantAccessToNewFormsForUserGroups;
                    if (!string.IsNullOrEmpty(formsForUserGroups))
                    {
                        if (formsForUserGroups.Split(new char[1]
                        {
              ','
                        }, StringSplitOptions.RemoveEmptyEntries).InvariantContains(userGroup.Alias))
                            flag = true;
                    }
                    UserGroupFormSecurity groupFormSecurity3 = new UserGroupFormSecurity();
                    groupFormSecurity3.Form = form.Id;
                    groupFormSecurity3.HasAccess = flag;
                    groupFormSecurity3.UserGroupId = userGroup.Id;
                    groupFormSecurity3.FormName = form.Name;
                    groupFormSecurity3.FormCreated = form.Created;
                    groupFormSecurity3.Fields = formFieldSummary;
                    groupFormSecurity3.SecurityType = FormSecurityType.Full;
                    groupFormSecurity3.AllowInEditor = true;
                    UserGroupFormSecurity formSecurity1 = groupFormSecurity3;
                    this.UserGroupFormSecurityStorage.InsertUserGroupFormSecurity(formSecurity1);
                    groupFormSecurity1.Add(formSecurity1);
                }
            }
            List<UserGroupFormSecurity> groupFormSecurityList = new List<UserGroupFormSecurity>();
            foreach (UserGroupFormSecurity groupFormSecurity4 in groupFormSecurity1)
            {
                UserGroupFormSecurity formSecurityDbEntry = groupFormSecurity4;
                if (source.SingleOrDefault<Umbraco.Forms.Core.Models.Form>(x => x.Id == formSecurityDbEntry.Form) == null)
                    groupFormSecurityList.Add(formSecurityDbEntry);
            }
            foreach (UserGroupFormSecurity formSecurity2 in groupFormSecurityList)
            {
                groupFormSecurity1.Remove(formSecurity2);
                this.UserGroupFormSecurityStorage.DeleteUserGroupFormSecurity(formSecurity2);
            }
            formSecurity.FormsSecurity = groupFormSecurity1.OrderBy<UserGroupFormSecurity, string>(x => x.FormName).ToList<UserGroupFormSecurity>();
        }

        private void ApplyStartFolders(FormSecurityForGroup formSecurity, int userGroupId) => formSecurity.StartFolderIds = this.UserGroupStartFolderStorage.GetStartFolderKeys(userGroupId).ToList<Guid>();
    }
}
