
// Type: Umbraco.Forms.Web.Api.ManagementApi.WorkflowType.GetByKeyWorkflowTypeController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.WorkflowType
{
  public class GetByKeyWorkflowTypeController : WorkflowTypeControllerBase
  {
    public GetByKeyWorkflowTypeController(
      WorkflowCollection workflowCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
      : base(workflowCollection, hostingEnvironment, formDesignSettings)
    {
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof (WorkflowTypeWithSettings), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetByKey(Guid id)
    {
      Umbraco.Forms.Core.WorkflowType type = this.WorkflowCollection.SingleOrDefault<Umbraco.Forms.Core.WorkflowType>((Func<Umbraco.Forms.Core.WorkflowType, bool>) (x => x.Id == id));
      return type == null ? (IActionResult) this.NotFound() : (IActionResult) this.Ok((object) this.CreateWorkflowTypeWithSettings(type));
    }
  }
}
