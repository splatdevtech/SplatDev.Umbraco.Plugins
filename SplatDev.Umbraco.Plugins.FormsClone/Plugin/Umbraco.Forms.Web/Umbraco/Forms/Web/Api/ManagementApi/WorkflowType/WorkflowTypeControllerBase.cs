
// Type: Umbraco.Forms.Web.Api.ManagementApi.WorkflowType.WorkflowTypeControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.WorkflowType
{
  [ApiExplorerSettings(GroupName = "Workflow Type")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/workflow-type")]
  public abstract class WorkflowTypeControllerBase : FormsManagementApiControllerBase
  {
    protected WorkflowTypeControllerBase(
      WorkflowCollection workflowCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
    {
      this.WorkflowCollection = workflowCollection;
      this.HostingEnvironment = hostingEnvironment;
      this.FormDesignSettings = formDesignSettings.Value;
    }

    protected WorkflowCollection WorkflowCollection { get; }

    protected IHostingEnvironment HostingEnvironment { get; }

    protected FormDesignSettings FormDesignSettings { get; }

    protected WorkflowTypeWithSettings CreateWorkflowTypeWithSettings(
      Umbraco.Forms.Core.WorkflowType type)
    {
      WorkflowTypeWithSettings providerType = new WorkflowTypeWithSettings();
      providerType.Id = type.Id;
      providerType.Alias = type.Alias;
      providerType.Name = type.Name;
      providerType.Description = type.Description;
      providerType.Icon = type.Icon;
      providerType.Group = type.Group;
      providerType.ApplySettings(type.Settings(), this.HostingEnvironment, this.FormDesignSettings.SettingsCustomization.WorkflowTypes.GetValueForProviderType((ProviderBase) type));
      return providerType;
    }
  }
}
