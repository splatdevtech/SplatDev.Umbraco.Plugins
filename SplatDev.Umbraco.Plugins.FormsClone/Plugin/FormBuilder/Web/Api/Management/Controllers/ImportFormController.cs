using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

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
using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    public class ImportFormController(
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
      ITemporaryFileService temporaryFileService) : FormControllerBase(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
    {
        private readonly IOptions<FormDesignSettings> _formDesignSettings = formDesignSettings;
        private readonly ITemporaryFileService _temporaryFileService = temporaryFileService;

        [HttpPost("import")]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 403)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> ImportForm(
          ImportFormModel model)
        {
            ImportFormController importFormController = this;
            FormDesign formData = DeserializeFormWithWorkflows(await importFormController._temporaryFileService.GetAsync(model.FileKey).ConfigureAwait(false) ?? throw new InvalidOperationException("Could not get temporary uploaded file to import."));
            if (!importFormController.IsFormValid(formData, importFormController._formDesignSettings.Value))
                return importFormController.BadRequest(new SimpleValidationModel(importFormController.ModelState.ToErrorDictionary()));
            formData.FolderId = model.FolderId;
            importFormController.CreateFormAndWorkflowsForPersistence(formData, out Form form, out List<Workflow> workflows);
            if (!importFormController.FormService.Exists(formData.Id))
            {
                importFormController.FormService.Insert(form);
                importFormController.WorkflowService.Insert(form, workflows);
                _ = await importFormController._temporaryFileService.DeleteAsync(model.FileKey).ConfigureAwait(false);
                return importFormController.Ok(form.Id);
            }
            if (!importFormController.ValidateAccessToForm(form))
                return importFormController.Forbid();
            importFormController.TryUpdateFormAndWorkflows(form, workflows);
            _ = await importFormController._temporaryFileService.DeleteAsync(model.FileKey).ConfigureAwait(false);
            return importFormController.Ok(form.Id);
        }

        private static FormDesign DeserializeFormWithWorkflows(TemporaryFileModel tempFile)
        {
            using Stream utf8Json = tempFile.OpenReadStream();
            return JsonSerializer.Deserialize<FormDesign>(utf8Json, FormsJsonSerializerOptions.Default) ?? throw new InvalidOperationException("Could not deserialize the temporary form file for form '" + tempFile.FileName + "'.");
        }

        /// <summary>Model POSTed to import a form.</summary>
        public record ImportFormModel
        {
            /// <summary>
            /// Gets or sets the file containing the serialized form definition.
            /// </summary>
            [Required]
            public Guid FileKey { get; init; }

            /// <summary>
            /// Gets or sets the Id of the folder in which to import the form (null if importing to root).
            /// </summary>
            public Guid? FolderId { get; init; }

            [Obsolete("Constructors of types with required members are not supported in this version of your compiler.", true)]
            public ImportFormModel()
            {
            }
        }
    }
}