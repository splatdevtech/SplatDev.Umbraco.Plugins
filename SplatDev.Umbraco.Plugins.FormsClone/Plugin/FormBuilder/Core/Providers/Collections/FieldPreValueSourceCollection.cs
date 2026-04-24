using FormBuilder.Core.Exceptions;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Builders;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Collections
{
    public class FieldPrevalueSourceCollection(Func<IEnumerable<FieldPrevalueSourceType>> items) : BuilderCollectionBase<FieldPrevalueSourceType>(CollectionBuilderHelper.GetItemsWithHighestPriority([.. items()]))
    {
        public FieldPrevalueSourceType this[Guid id]
        {
            get
            {
                FieldPrevalueSourceType? preValueSourceType = this.FirstOrDefault(x => x.Id == id);
                if (preValueSourceType is not null)
                    return preValueSourceType;

                DefaultInterpolatedStringHandler interpolatedStringHandler = new(55, 1);
                interpolatedStringHandler.AppendLiteral("Unable to find the Field Prevalue Source with the GUID ");
                interpolatedStringHandler.AppendFormatted(id);
                throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
            }
        }
    }
}