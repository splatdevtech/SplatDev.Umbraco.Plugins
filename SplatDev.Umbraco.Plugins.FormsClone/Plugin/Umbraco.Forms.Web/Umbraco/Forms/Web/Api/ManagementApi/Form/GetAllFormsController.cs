
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.GetAllFormsController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Security;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
  public class GetAllFormsController : FormControllerBase
  {
    public GetAllFormsController(
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
      IHtmlSanitizer htmlSanitizer)
      : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, (ILogger) logger, htmlSanitizer)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<BasicForm>), 200)]
    public IActionResult GetAllForms()
    {
      List<BasicForm> basicFormList = new List<BasicForm>();
      if (this.FormsSecurity.CanCurrentUserManageForms())
      {
        IList<Umbraco.Forms.Core.Models.Form> list = (IList<Umbraco.Forms.Core.Models.Form>) this.FormService.Get().ToList<Umbraco.Forms.Core.Models.Form>();
        IList<Guid> filteredFormIds = (IList<Guid>) this.FormsSecurity.FilterFormIdsForCurrentUser(list.Select<Umbraco.Forms.Core.Models.Form, Guid>((Func<Umbraco.Forms.Core.Models.Form, Guid>) (x => x.Id))).ToList<Guid>();
        foreach (Umbraco.Forms.Core.Models.Form form in (IEnumerable<Umbraco.Forms.Core.Models.Form>) list.Where<Umbraco.Forms.Core.Models.Form>((Func<Umbraco.Forms.Core.Models.Form, bool>) (x => filteredFormIds.Contains(x.Id))).ToList<Umbraco.Forms.Core.Models.Form>().OrderByDescending<Umbraco.Forms.Core.Models.Form, DateTime>((Func<Umbraco.Forms.Core.Models.Form, DateTime>) (x => x.Created)))
          basicFormList.Add(this.CreateBasicForm(form));
      }
      return (IActionResult) this.Ok((object) basicFormList);
    }
  }
}
