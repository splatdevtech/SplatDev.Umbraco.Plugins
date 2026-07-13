using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Workflow.Core.Contracts;
using SplatDev.Umbraco.Workflow.Core.Engine;
using SplatDev.Umbraco.Workflow.Persistence.Repositories;

namespace SplatDev.Umbraco.Workflow.Persistence;

public class WorkflowComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<IWorkflowDefinitionStore, WorkflowDefinitionStore>();
        builder.Services.AddScoped<IWorkflowInstanceStore, WorkflowInstanceStore>();
        builder.Services.AddScoped<IWorkflowEngine, WorkflowEngine>();
    }
}
