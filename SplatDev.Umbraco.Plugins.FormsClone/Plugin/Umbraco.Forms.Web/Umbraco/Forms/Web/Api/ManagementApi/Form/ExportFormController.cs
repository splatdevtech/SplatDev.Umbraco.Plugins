
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.ExportFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
  public class ExportFormController : FormControllerBase
  {
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly IOptions<FormDesignSettings> _formDesignSettings;

    public ExportFormController(
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
      IOptions<FormDesignSettings> formDesignSettings)
      : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, (ILogger) logger, htmlSanitizer)
    {
      this._hostingEnvironment = hostingEnvironment;
      this._formDesignSettings = formDesignSettings;
    }

    [HttpGet("export")]
    [ProducesResponseType(typeof (File), 200)]
    public IActionResult ExportForm(Guid guid)
    {
      FormDesign form = this.GetWithWorkflowsByGuid(guid) ?? throw new NullReferenceException("No form found with id " + guid.ToString());
      byte[] bytes = Encoding.UTF8.GetBytes(ExportFormController.SerializeFormWithWorkflows(form));
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
      interpolatedStringHandler.AppendLiteral("form-");
      interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
      interpolatedStringHandler.AppendLiteral(".json");
      string stringAndClear = interpolatedStringHandler.ToStringAndClear();
      return (IActionResult) this.File(bytes, "application/octet-stream", stringAndClear);
    }

    private static string SerializeFormWithWorkflows(FormDesign form)
    {
      JsonSerializerOptions options = FormsJsonSerializerOptions.Default;
      options.WriteIndented = true;
      return JsonSerializer.Serialize<FormDesign>(form, options);
    }

    private FormDesign GetWithWorkflowsByGuid(Guid guid)
    {
      Umbraco.Forms.Core.Models.Form form = this.FormService.Get(guid) ?? throw new NullReferenceException("form");
      this.ValidateAccessToForm(form);
      foreach (Umbraco.Forms.Core.Models.Field allField in form.AllFields)
      {
                Umbraco.Forms.Core.FieldType fieldTypeByField = this.FieldTypeStorage.GetFieldTypeByField(allField);
        if (fieldTypeByField != null && fieldTypeByField.SupportsPreValues && allField.PreValueSourceId != Guid.Empty)
          allField.PreValues = (IEnumerable<FieldPrevalue>) new List<FieldPrevalue>();
      }
      foreach (FieldSet fieldSet in form.Pages.SelectMany<Page, FieldSet>((Func<Page, IEnumerable<FieldSet>>) (x => (IEnumerable<FieldSet>) x.FieldSets)))
      {
        if (fieldSet.Id == Guid.Empty)
          fieldSet.Id = Guid.NewGuid();
      }
      FormDesign withWorkflowsByGuid = this.Mapper.Map<FormDesign>((object) form);
      if (withWorkflowsByGuid == null)
        throw new InvalidOperationException("Could not map from object");
      withWorkflowsByGuid.FormWorkflows = this.GetWorkflowsForForm(form);
      return withWorkflowsByGuid;
    }
  }
}
