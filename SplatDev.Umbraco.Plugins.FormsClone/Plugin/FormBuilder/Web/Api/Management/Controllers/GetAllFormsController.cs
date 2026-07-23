using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Mapping;

using Umbraco.Cms.Core.Security;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for retrieving all forms.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetAllFormsController(
      IFormService formService,
      IFolderService folderService,
      IWorkflowService workflowService,
      IFieldService fieldService,
      IFieldTypeStorage fieldTypeStorage,
      WorkflowCollection workflowCollection,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IFormsSecurity formsSecurity,
      IUmbracoMapper mapper,
      ILogger<GetAllFormsController> logger,
      IHtmlSanitizer htmlSanitizer) : FormControllerBase(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
    {
        /// <summary>Management API endpoint for retrieving all forms.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BasicForm>), 200)]
        public IActionResult GetAllForms()
        {
            List<BasicForm> basicFormList = [];
            if (FormsSecurity.CanCurrentUserManageForms())
            {
                IList<Form> list = [.. FormService.Get()];
                IList<Guid> filteredFormIds = [.. FormsSecurity.FilterFormIdsForCurrentUser(list.Select(x => x.Id))];
                foreach (Form form in (IEnumerable<Form>)list.Where(x => filteredFormIds.Contains(x.Id)).ToList().OrderByDescending(x => x.Created))
                    basicFormList.Add(CreateBasicForm(form));
            }
            return Ok(basicFormList);
        }
    }
}