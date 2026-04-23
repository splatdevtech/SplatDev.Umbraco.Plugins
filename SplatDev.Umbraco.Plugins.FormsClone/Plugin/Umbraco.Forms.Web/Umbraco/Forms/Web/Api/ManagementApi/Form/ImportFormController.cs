
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.ImportFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.TemporaryFile;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
    public class ImportFormController : FormControllerBase
    {
        private readonly IOptions<FormDesignSettings> _formDesignSettings;
        private readonly ITemporaryFileService _temporaryFileService;

        public ImportFormController(
          IFormService formService,
          IFolderService folderService,
          IWorkflowService workflowService,
          IFieldService fieldService,
          IFieldTypeStorage fieldTypeStorage,
          WorkflowCollection workflowCollection,
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          IFormsSecurity formsSecurity,
          IUmbracoMapper mapper,
          ILogger<ImportFormController> logger,
          IHtmlSanitizer htmlSanitizer,
          IOptions<FormDesignSettings> formDesignSettings,
          ITemporaryFileService temporaryFileService)
          : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
        {
            this._formDesignSettings = formDesignSettings;
            this._temporaryFileService = temporaryFileService;
        }

        [HttpPost("import")]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 403)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> ImportForm(
          ImportFormController.ImportFormModel model)
        {
            ImportFormController importFormController = this;
            FormDesign formData = ImportFormController.DeserializeFormWithWorkflows(await importFormController._temporaryFileService.GetAsync(model.FileKey).ConfigureAwait(false) ?? throw new InvalidOperationException("Could not get temporary uploaded file to import."));
            if (!importFormController.IsFormValid(formData, importFormController._formDesignSettings.Value))
                return importFormController.BadRequest(new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(importFormController.ModelState)));
            formData.FolderId = model.FolderId;
            Umbraco.Forms.Core.Models.Form form;
            List<Umbraco.Forms.Core.Models.Workflow> workflows;
            importFormController.CreateFormAndWorkflowsForPersistence(formData, out form, out workflows);
            if (!importFormController.FormService.Exists(formData.Id))
            {
                importFormController.FormService.Insert(form);
                importFormController.WorkflowService.Insert(form, workflows);
                Umbraco.Cms.Core.Attempt<TemporaryFileModel, TemporaryFileOperationStatus> attempt = await importFormController._temporaryFileService.DeleteAsync(model.FileKey).ConfigureAwait(false);
                return importFormController.Ok(form.Id);
            }
            if (!importFormController.ValidateAccessToForm(form))
                return importFormController.Forbid();
            importFormController.TryUpdateFormAndWorkflows(form, workflows);
            Umbraco.Cms.Core.Attempt<TemporaryFileModel, TemporaryFileOperationStatus> attempt1 = await importFormController._temporaryFileService.DeleteAsync(model.FileKey).ConfigureAwait(false);
            return importFormController.Ok(form.Id);
        }

        private static FormDesign DeserializeFormWithWorkflows(TemporaryFileModel tempFile)
        {
            using (Stream utf8Json = tempFile.OpenReadStream())
                return JsonSerializer.Deserialize<FormDesign>(utf8Json, FormsJsonSerializerOptions.Default) ?? throw new InvalidOperationException("Could not deserialize the temporary form file for form '" + tempFile.FileName + "'.");
        }

        public record ImportFormModel
        {
            [Required]
            public Guid FileKey { get; init; }

            public Guid? FolderId { get; init; }
        }
    }
}
