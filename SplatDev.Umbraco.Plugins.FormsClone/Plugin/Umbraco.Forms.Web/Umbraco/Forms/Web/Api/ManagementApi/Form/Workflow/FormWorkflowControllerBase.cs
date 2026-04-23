
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.Workflow.FormWorkflowControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form.Workflow
{
  [ApiExplorerSettings(GroupName = "Form")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/form-workflow")]
  public abstract class FormWorkflowControllerBase : FormsManagementApiControllerBase
  {
    protected FormWorkflowControllerBase(WorkflowCollection workflowCollection) => this.WorkflowCollection = workflowCollection;

    protected WorkflowCollection WorkflowCollection { get; }
  }
}
