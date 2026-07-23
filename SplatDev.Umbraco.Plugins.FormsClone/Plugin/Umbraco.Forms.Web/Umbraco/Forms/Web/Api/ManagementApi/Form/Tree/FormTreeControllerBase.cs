
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.Tree.FormTreeControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Management.ViewModels;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi.Form.Tree;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form.Tree
{
    [ApiExplorerSettings(GroupName = "Form")]
    [Authorize(Policy = "BackOfficeAccess")]
    [Route("/umbraco/forms/management/api/v1/tree/form")]
    public abstract class FormTreeControllerBase : FormsManagementApiControllerBase
    {
        private readonly IFormService _formService;
        private readonly IFolderService _folderService;
        private readonly IFormsSecurity _formsSecurity;

        protected FormTreeControllerBase(
          IFormService formService,
          IFolderService folderService,
          IFormsSecurity formsSecurity)
        {
            this._formService = formService;
            this._folderService = folderService;
            this._formsSecurity = formsSecurity;
        }

        protected FormTreeControllerBase.FormTreeAccess GetFormTreeAccessForCurrentUser()
        {
            bool flag1 = this._formsSecurity.CanCurrentUserManageForms();
            bool flag2 = this._formsSecurity.CanCurrentUserViewEntries();
            if (flag1 & flag2)
                return FormTreeControllerBase.FormTreeAccess.Full;
            if (flag1)
                return FormTreeControllerBase.FormTreeAccess.Design;
            return flag2 ? FormTreeControllerBase.FormTreeAccess.ViewEntries : FormTreeControllerBase.FormTreeAccess.None;
        }

        protected IEnumerable<FormTreeItemResponseModel> GetFoldersAndForms(
          bool foldersOnly,
          bool ignoreStartFolders,
          FormTreeControllerBase.FormTreeAccess treeAccess,
          Guid? parentId = null)
        {
            List<FormTreeItemResponseModel> foldersAndForms = new List<FormTreeItemResponseModel>();
            bool flag = false;
            string rootPath;
            IEnumerable<Umbraco.Forms.Core.Models.Folder> folders;
            if (!parentId.HasValue)
            {
                rootPath = "-1";
                if (!ignoreStartFolders)
                {
                    IList<Guid> list = this._formsSecurity.GetStartFolderKeysForCurrentUser().ToList<Guid>();
                    flag = list.Count > 0;
                    if (list.Count == 0)
                        folders = this._folderService.GetAtRoot();
                    else if (list.Count == 1)
                    {
                        Guid parentId1 = list.First<Guid>();
                        parentId = new Guid?(parentId1);
                        folders = this._folderService.GetChildren(parentId1);
                    }
                    else
                        folders = this._folderService.Get(list.ToArray<Guid>());
                }
                else
                    folders = this._folderService.GetAtRoot();
            }
            else
            {
                rootPath = this._folderService.GetPath(parentId.Value);
                folders = this._folderService.GetChildren(parentId.Value);
            }
            foreach (Umbraco.Forms.Core.Models.Folder folder in folders)
            {
                FormTreeItemResponseModel itemResponse = this.CreateItemResponse(folder, parentId, rootPath);
                foldersAndForms.Add(itemResponse);
            }
            if (foldersOnly)
                return foldersAndForms;
            if (!parentId.HasValue & flag)
                return foldersAndForms;
            IList<FormSlim> source = !parentId.HasValue ? this._formService.GetAtRootSlim().ToList<FormSlim>() : (IList<FormSlim>)this._formService.GetFromFolderSlim(parentId.Value).ToList<FormSlim>();
            IEnumerable<Guid> formsIdsWithAccess = this._formsSecurity.FilterFormIdsForCurrentUser(source.Select<FormSlim, Guid>(x => x.Id));
            foreach (FormSlim form in (IEnumerable<FormSlim>)source.Where<FormSlim>(x => formsIdsWithAccess.Contains<Guid>(x.Id)).OrderBy<FormSlim, string>(x => x.Name).ToList<FormSlim>())
            {
                FormTreeItemResponseModel itemResponse = this.CreateItemResponse(form, parentId, rootPath);
                foldersAndForms.Add(itemResponse);
            }
            return foldersAndForms;
        }

        private FormTreeItemResponseModel CreateItemResponse(
          Umbraco.Forms.Core.Models.Folder folder,
          Guid? parentId,
          string rootPath)
        {
            FormTreeItemResponseModel itemResponse = new FormTreeItemResponseModel();
            itemResponse.Id = folder.Id;
            itemResponse.HasChildren = true;
            itemResponse.Name = folder.Name;
            itemResponse.IsFolder = true;
            itemResponse.Parent = parentId.HasValue ? new ReferenceByIdModel(parentId.Value) : null;
            itemResponse.Path = rootPath + "," + folder.Id.ToString();
            return itemResponse;
        }

        private FormTreeItemResponseModel CreateItemResponse(
          FormSlim form,
          Guid? parentId,
          string rootPath)
        {
            FormTreeItemResponseModel itemResponse = new FormTreeItemResponseModel();
            itemResponse.Id = form.Id;
            itemResponse.HasChildren = false;
            itemResponse.Name = form.Name;
            itemResponse.IsFolder = false;
            itemResponse.Parent = parentId.HasValue ? new ReferenceByIdModel(parentId.Value) : null;
            itemResponse.Path = rootPath + "," + form.Id.ToString();
            return itemResponse;
        }

        [Flags]
        protected enum FormTreeAccess
        {
            None = 0,
            Design = 1,
            ViewEntries = 2,
            Full = ViewEntries | Design, // 0x00000003
        }
    }
}
