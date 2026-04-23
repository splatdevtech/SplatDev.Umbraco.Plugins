
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.UpdateFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
    public class UpdateFormController : FormControllerBase
    {
        private readonly FormDesignSettings _formDesignSettings;

        public UpdateFormController(
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
          IOptions<FormDesignSettings> formDesignSettings)
          : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
        {
            this._formDesignSettings = formDesignSettings.Value;
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public IActionResult Update(Guid id, FormDesign formData)
        {
            if (formData == null)
                return this.BadRequest(new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
            Umbraco.Forms.Core.Models.Form form1 = this.FormService.Get(id);
            if (form1 == null)
                return this.NotFound();
            if (!this.ValidateAccessToForm(form1))
                return this.Forbid();
            if (!this.IsFormValid(formData, this._formDesignSettings))
                return this.BadRequest(new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
            IUser currentUser = this.BackOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser == null)
                throw new InvalidOperationException("Cannot access current user to determine access to sensitive data.");
            if (!currentUser.HasAccessToSensitiveData() && this.IsSensitiveDataBeingExposed(formData))
                return this.Forbid();
            Umbraco.Forms.Core.Models.Form form2;
            List<Umbraco.Forms.Core.Models.Workflow> workflows;
            this.CreateFormAndWorkflowsForPersistence(formData, out form2, out workflows);
            this.TryUpdateFormAndWorkflows(form2, workflows);
            return this.Ok();
        }

        private bool IsSensitiveDataBeingExposed(Umbraco.Forms.Core.Models.Form form)
        {
            List<Umbraco.Forms.Core.Models.Field> source = this.FormService.Get(form.Id)?.AllFields ?? new List<Umbraco.Forms.Core.Models.Field>();
            foreach (Umbraco.Forms.Core.Models.Field allField in form.AllFields)
            {
                Umbraco.Forms.Core.Models.Field field = allField;
                Umbraco.Forms.Core.Models.Field field1 = source.FirstOrDefault<Umbraco.Forms.Core.Models.Field>(p => p.Id == field.Id);
                if (field1 != null && field1.ContainsSensitiveData && !field.ContainsSensitiveData)
                    return true;
            }
            return false;
        }
    }
}
