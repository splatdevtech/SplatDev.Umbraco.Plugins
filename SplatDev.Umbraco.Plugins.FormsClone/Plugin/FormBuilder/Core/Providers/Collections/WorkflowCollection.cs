using FormBuilder.Core.Exceptions;
using FormBuilder.Core.Providers.Builders;
using FormBuilder.Core.Workflows;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Collections
{
    public class WorkflowCollection(Func<IEnumerable<WorkflowType>> items) : BuilderCollectionBase<WorkflowType>(CollectionBuilderHelper.GetItemsWithHighestPriority(items().ToArray()))
    {
        public WorkflowType this[Guid id]
        {
            get
            {
                WorkflowType? workflowType = this.FirstOrDefault(x => x.Id == id);
                if (workflowType is not null)
                    return workflowType;

                DefaultInterpolatedStringHandler interpolatedStringHandler = new(42, 1);
                interpolatedStringHandler.AppendLiteral("Unable to find the Workflow with the GUID ");
                interpolatedStringHandler.AppendFormatted(id);
                throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
            }
        }
    }
}