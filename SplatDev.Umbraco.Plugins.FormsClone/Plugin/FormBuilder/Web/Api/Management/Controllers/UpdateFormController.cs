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
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for updating a form.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class UpdateFormController(
      IFormService formService,
      IFolderService folderService,
      IWorkflowService workflowService,
      IFieldService fieldService,
      IFieldTypeStorage fieldTypeStorage,
      WorkflowCollection workflowCollection,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IFormsSecurity formsSecurity,
      IUmbracoMapper mapper,
      ILogger<UpdateFormController> logger,
      IHtmlSanitizer htmlSanitizer,
      IOptions<FormDesignSettings> formDesignSettings) : FormControllerBase(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
    {
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;

        /// <summary>Management API endpoint for updating a form.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public IActionResult Update(Guid id, FormDesign formData)
        {
            if (formData is null)
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            Form? form1 = FormService.Get(id);
            if (form1 is null)
                return NotFound();
            if (!ValidateAccessToForm(form1))
                return Forbid();
            if (!IsFormValid(formData, _formDesignSettings))
                return BadRequest(new SimpleValidationModel(ModelState.ToErrorDictionary()));
            IUser? currentUser = (BackOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser) ?? throw new InvalidOperationException("Cannot access current user to determine access to sensitive data.");
            if (!currentUser.HasAccessToSensitiveData() && IsSensitiveDataBeingExposed(formData))
                return Forbid();
            CreateFormAndWorkflowsForPersistence(formData, out Form form2, out List<Workflow>? workflows);
            TryUpdateFormAndWorkflows(form2, workflows);
            return Ok();
        }

        private bool IsSensitiveDataBeingExposed(Form form)
        {
            List<Field>? source = FormService.Get(form.Id)?.AllFields ?? [];
            foreach (Field allField in form.AllFields)
            {
                var field = allField;
                Field? field1 = source.FirstOrDefault(p => p.Id == field.Id);
                if (field1 is not null && field1.ContainsSensitiveData && !field.ContainsSensitiveData)
                    return true;
            }
            return false;
        }
    }
}