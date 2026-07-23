using FormBuilder.Core.Models;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Management.ViewModels;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with the forms tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Form")]
    [Authorize(Policy = "BackOfficeAccess")]
    [Route("/formBuilder/management/api/v1/tree/form")]
    public abstract class FormTreeControllerBase(
      IFormService formService,
      IFolderService folderService,
      IFormsSecurity formsSecurity) : FormsManagementApiControllerBase
    {
        private readonly IFormService _formService = formService;
        private readonly IFolderService _folderService = folderService;
        private readonly IFormsSecurity _formsSecurity = formsSecurity;

        /// <summary>
        /// Determines access to the form's tree for the current backoffice user.
        /// </summary>
        protected FormTreeAccess GetFormTreeAccessForCurrentUser()
        {
            bool flag1 = _formsSecurity.CanCurrentUserManageForms();
            bool flag2 = _formsSecurity.CanCurrentUserViewEntries();
            if (flag1 & flag2)
                return FormTreeAccess.Full;
            if (flag1)
                return FormTreeAccess.Design;
            return flag2 ? FormTreeAccess.ViewEntries : FormTreeAccess.None;
        }

        /// <summary>Retrieves folders and forms for the provider path.</summary>
        protected IEnumerable<FormTreeItemResponseModel> GetFoldersAndForms(
          bool foldersOnly,
          bool ignoreStartFolders,
          Guid? parentId = null)
        {
            List<FormTreeItemResponseModel> foldersAndForms = [];
            bool flag = false;
            string rootPath;
            IEnumerable<Folder> folders;
            if (!parentId.HasValue)
            {
                rootPath = "-1";
                if (!ignoreStartFolders)
                {
                    IList<Guid> list = [.. _formsSecurity.GetStartFolderKeysForCurrentUser()];
                    flag = list.Count > 0;
                    if (list.Count == 0)
                        folders = _folderService.GetAtRoot();
                    else if (list.Count == 1)
                    {
                        Guid parentId1 = list.First();
                        parentId = new Guid?(parentId1);
                        folders = _folderService.GetChildren(parentId1);
                    }
                    else
                        folders = _folderService.Get([.. list]);
                }
                else
                    folders = _folderService.GetAtRoot();
            }
            else
            {
                rootPath = _folderService.GetPath(parentId.Value);
                folders = _folderService.GetChildren(parentId.Value);
            }
            foreach (Folder folder in folders)
            {
                FormTreeItemResponseModel itemResponse = CreateItemResponse(folder, parentId, rootPath);
                foldersAndForms.Add(itemResponse);
            }
            if (foldersOnly)
                return foldersAndForms;
            if (!parentId.HasValue & flag)
                return foldersAndForms;
            IList<FormSlim> source = !parentId.HasValue ? [.. _formService.GetAtRootSlim()] : [.. _formService.GetFromFolderSlim(parentId.Value)];
            IEnumerable<Guid> formsIdsWithAccess = _formsSecurity.FilterFormIdsForCurrentUser(source.Select(x => x.Id));
            foreach (FormSlim form in (IEnumerable<FormSlim>)[.. source.Where(x => formsIdsWithAccess.Contains(x.Id)).OrderBy(x => x.Name)])
            {
                FormTreeItemResponseModel itemResponse = CreateItemResponse(form, parentId, rootPath);
                foldersAndForms.Add(itemResponse);
            }
            return foldersAndForms;
        }

        private static FormTreeItemResponseModel CreateItemResponse(
          Folder folder,
          Guid? parentId,
          string rootPath)
        {
            FormTreeItemResponseModel itemResponse = new()
            {
                Id = folder.Id,
                HasChildren = true,
                Name = folder.Name,
                IsFolder = true,
                Parent = parentId.HasValue ? new ReferenceByIdModel(parentId.Value) : null,
                Path = rootPath + "," + folder.Id.ToString()
            };
            return itemResponse;
        }

        private static FormTreeItemResponseModel CreateItemResponse(
          FormSlim form,
          Guid? parentId,
          string rootPath)
        {
            FormTreeItemResponseModel itemResponse = new()
            {
                Id = form.Id,
                HasChildren = false,
                Name = form.Name,
                IsFolder = false,
                Parent = parentId.HasValue ? new ReferenceByIdModel(parentId.Value) : null,
                Path = rootPath + "," + form.Id.ToString()
            };
            return itemResponse;
        }

        /// <summary>
        /// An enum used to simplify tracking access to form features available from the tree for the current user.
        /// </summary>
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