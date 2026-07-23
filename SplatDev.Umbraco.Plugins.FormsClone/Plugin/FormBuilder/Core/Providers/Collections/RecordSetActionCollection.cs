using FormBuilder.Core.Exceptions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Builders;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Collections
{
    public class RecordSetActionCollection(Func<IEnumerable<RecordSetActionType>> items) : BuilderCollectionBase<RecordSetActionType>(CollectionBuilderHelper.GetItemsWithHighestPriority(items().ToArray()))
    {
        public RecordSetActionType this[Guid id]
        {
            get
            {
                RecordSetActionType? recordSetActionType = this.FirstOrDefault(x => x.Id == id);
                if (recordSetActionType is not null)
                    return recordSetActionType;

                DefaultInterpolatedStringHandler interpolatedStringHandler = new(50, 1);
                interpolatedStringHandler.AppendLiteral("Unable to find the RecordSet Action with the GUID ");
                interpolatedStringHandler.AppendFormatted(id);
                throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
            }
        }
    }
}