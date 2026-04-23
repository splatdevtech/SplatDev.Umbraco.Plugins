
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.CreateFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
  public class CreateFormController : FormControllerBase
  {
    private readonly FormDesignSettings _formDesignSettings;

    public CreateFormController(
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
      IOptions<FormDesignSettings> formDesignSettings)
      : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, (ILogger) logger, htmlSanitizer)
    {
      this._formDesignSettings = formDesignSettings.Value;
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(typeof (ProblemDetails), 400)]
    [ProducesResponseType(typeof (ProblemDetails), 403)]
    public IActionResult Create(FormDesign formData)
    {
      if (formData == null)
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      if (this.DoesFormExist(formData.Name, formData.FolderId))
      {
        this.ModelState.AddModelError("Name", "A form with the same name already exists");
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      }
      if (!this.IsFormValid(formData, this._formDesignSettings))
        return (IActionResult) this.BadRequest((object) new SimpleValidationModel(ModelStateExtensions.ToErrorDictionary(this.ModelState)));
      Umbraco.Forms.Core.Models.Form form;
      List<Umbraco.Forms.Core.Models.Workflow> workflows;
      this.CreateFormAndWorkflowsForPersistence(formData, out form, out workflows);
      this.FormService.Insert(form);
      this.WorkflowService.Insert(form, (IEnumerable<Umbraco.Forms.Core.Models.Workflow>) workflows);
      return (IActionResult) this.CreatedAtAction<GetByKeyFormController>((Expression<Func<GetByKeyFormController, string>>) (controller => "GetByKey"), form.Id);
    }

    private bool DoesFormExist(string formName, Guid? folderId) => this.FormService.Get().Any<Umbraco.Forms.Core.Models.Form>((Func<Umbraco.Forms.Core.Models.Form, bool>) (x =>
    {
      if (x.Name == null || !(x.Name.ToLower() == formName.ToLower()))
        return false;
      Guid? folderId1 = x.FolderId;
      Guid? nullable = folderId;
      if (folderId1.HasValue != nullable.HasValue)
        return false;
      return !folderId1.HasValue || folderId1.GetValueOrDefault() == nullable.GetValueOrDefault();
    }));
  }
}
