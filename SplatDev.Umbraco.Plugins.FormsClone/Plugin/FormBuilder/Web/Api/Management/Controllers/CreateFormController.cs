using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Mapping;

using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.Security;

using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for creating a form.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class CreateFormController(
      IFormService formService,
      IFolderService folderService,
      IWorkflowService workflowService,
      IFieldService fieldService,
      IFieldTypeStorage fieldTypeStorage,
      WorkflowCollection workflowCollection,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IFormsSecurity formsSecurity,
      IUmbracoMapper mapper,
      ILogger<CreateFormController> logger,
      IHtmlSanitizer htmlSanitizer,
      IOptions<FormDesignSettings> formDesignSettings) : FormControllerBase(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
    {
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;

        /// <summary>Management API endpoint for creating a form.</summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 403)]
        public IActionResult Create(FormDesign formData)
        {
            if (formData is null)
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            if (DoesFormExist(formData.Name, formData.FolderId))
            {
                ModelState.AddModelError("Name", "A form with the same name already exists");
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            }
            if (!IsFormValid(formData, _formDesignSettings))
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            CreateFormAndWorkflowsForPersistence(formData, out Form? form, out List<Workflow> workflows);
            FormService.Insert(form);
            WorkflowService.Insert(form, workflows);
            return CreatedAtAction<GetByKeyFormController>(controller => "GetByKey", form.Id);
        }

        private bool DoesFormExist(string formName, Guid? folderId) => FormService.Get().Any(x =>
        {
            if (x.Name is null || !x.Name.Equals(formName, StringComparison.CurrentCultureIgnoreCase))
                return false;
            Guid? folderId1 = x.FolderId;
            Guid? nullable = folderId;
            if (folderId1.HasValue != nullable.HasValue)
                return false;
            return !folderId1.HasValue || folderId1.GetValueOrDefault() == nullable.GetValueOrDefault();
        });
    }
}