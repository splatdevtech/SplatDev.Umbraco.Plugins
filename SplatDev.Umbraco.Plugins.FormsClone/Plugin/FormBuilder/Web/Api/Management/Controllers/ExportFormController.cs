using FormBuilder.Core.Configuration;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;

namespace FormBuilder.Web.Api.Management.Controllers
{
    public class ExportFormController(
      IFormService formService,
      IFolderService folderService,
      IWorkflowService workflowService,
      IFieldService fieldService,
      IFieldTypeStorage fieldTypeStorage,
      WorkflowCollection workflowCollection,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IFormsSecurity formsSecurity,
      IUmbracoMapper mapper,
      ILogger<ExportFormController> logger,
      IHtmlSanitizer htmlSanitizer,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : FormControllerBase(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
    {
        private readonly IHostingEnvironment _hostingEnvironment = hostingEnvironment;
        private readonly IOptions<FormDesignSettings> _formDesignSettings = formDesignSettings;

        [HttpGet("export")]
        [ProducesResponseType(typeof(Core.Models.File), 200)]
        public IActionResult ExportForm(Guid guid)
        {
            FormDesign form = GetWithWorkflowsByGuid(guid) ?? throw new NullReferenceException("No form found with id " + guid.ToString());
            byte[] bytes = Encoding.UTF8.GetBytes(SerializeFormWithWorkflows(form));
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(10, 1);
            interpolatedStringHandler.AppendLiteral("form-");
            interpolatedStringHandler.AppendFormatted(form.Id);
            interpolatedStringHandler.AppendLiteral(".json");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            return File(bytes, "application/octet-stream", stringAndClear);
        }

        private static string SerializeFormWithWorkflows(FormDesign form)
        {
            JsonSerializerOptions options = FormsJsonSerializerOptions.Default;
            options.WriteIndented = true;
            return JsonSerializer.Serialize(form, options);
        }

        private FormDesign GetWithWorkflowsByGuid(Guid guid)
        {
            Form form = FormService.Get(guid) ?? throw new NullReferenceException("form");
            ValidateAccessToForm(form);
            foreach (Field allField in form.AllFields)
            {
                FieldType? fieldTypeByField = FieldTypeStorage.GetFieldTypeByField(allField);
                if (fieldTypeByField is not null && fieldTypeByField.SupportsPreValues && allField.PreValueSourceId != Guid.Empty)
                    allField.PreValues = [];
            }
            foreach (FieldSet fieldSet in form.Pages.SelectMany(x => x.FieldSets))
            {
                if (fieldSet.Id == Guid.Empty)
                    fieldSet.Id = Guid.NewGuid();
            }
            FormDesign? withWorkflowsByGuid = Mapper.Map<FormDesign>(form) ?? throw new InvalidOperationException("Could not map from object");
            withWorkflowsByGuid.FormWorkflows = GetWorkflowsForForm(form);
            return withWorkflowsByGuid;
        }
    }
}