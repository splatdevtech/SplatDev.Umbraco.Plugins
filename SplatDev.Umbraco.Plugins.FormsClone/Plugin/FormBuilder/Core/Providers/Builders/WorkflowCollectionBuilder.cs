using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Builders
{
    public class WorkflowCollectionBuilder :
      LazyCollectionBuilderBase<WorkflowCollectionBuilder, WorkflowCollection, WorkflowType>
    {
        protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Transient;

        protected override WorkflowCollectionBuilder This => this;
    }
}