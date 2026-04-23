using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Workflows;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with workflow types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Workflow Type")]
    [Authorize(Policy = "SectionAccessForms")]
    [Route("/formBuilder/management/api/v1/workflow-type")]
    public abstract class WorkflowTypeControllerBase(
      WorkflowCollection workflowCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected WorkflowCollection WorkflowCollection { get; } = workflowCollection;

        /// <summary>
        /// Gets the         /// </summary>
        protected IHostingEnvironment HostingEnvironment { get; } = hostingEnvironment;

        /// <summary>
        /// Gets the         /// </summary>
        protected FormDesignSettings FormDesignSettings { get; } = formDesignSettings.Value;

        /// <summary>
        /// Creates a         /// </summary>
        protected WorkflowTypeWithSettings CreateWorkflowTypeWithSettings(
          WorkflowType type)
        {
            WorkflowTypeWithSettings providerType = new()
            {
                Id = type.Id,
                Alias = type.Alias,
                Name = type.Name,
                Description = type.Description,
                Icon = type.Icon,
                Group = type.Group
            };
            providerType.ApplySettings(type.Settings(), FormDesignSettings.SettingsCustomization.WorkflowTypes.GetValueForProviderType(type));
            return providerType;
        }
    }
}