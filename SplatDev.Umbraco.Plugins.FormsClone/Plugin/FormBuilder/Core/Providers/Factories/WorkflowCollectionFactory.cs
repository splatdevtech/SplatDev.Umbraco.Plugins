using FormBuilder.Core.Providers.Collections;

using Microsoft.Extensions.DependencyInjection;

namespace FormBuilder.Core.Providers.Factories
{
    public class WorkflowCollectionFactory(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public WorkflowCollection GetWorkflowCollection() => _serviceProvider.GetRequiredService<WorkflowCollection>();
    }
}