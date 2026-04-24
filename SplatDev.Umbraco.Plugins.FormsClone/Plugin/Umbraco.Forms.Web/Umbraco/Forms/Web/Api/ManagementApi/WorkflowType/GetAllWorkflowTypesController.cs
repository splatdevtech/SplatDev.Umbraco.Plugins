
// Type: Umbraco.Forms.Web.Api.ManagementApi.WorkflowType.GetAllWorkflowTypesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.WorkflowType
{
  public class GetAllWorkflowTypesController : WorkflowTypeControllerBase
  {
    public GetAllWorkflowTypesController(
      WorkflowCollection workflowCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
      : base(workflowCollection, hostingEnvironment, formDesignSettings)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<WorkflowTypeWithSettings>), 200)]
    public IActionResult GetAll()
    {
      List<WorkflowTypeWithSettings> source = new List<WorkflowTypeWithSettings>();
      foreach (Umbraco.Forms.Core.WorkflowType workflow in (BuilderCollectionBase<Umbraco.Forms.Core.WorkflowType>) this.WorkflowCollection)
      {
        WorkflowTypeWithSettings typeWithSettings = this.CreateWorkflowTypeWithSettings(workflow);
        source.Add(typeWithSettings);
      }
      return (IActionResult) this.Ok((object) source.OrderBy<WorkflowTypeWithSettings, string>((Func<WorkflowTypeWithSettings, string>) (x => x.Name)));
    }
  }
}
