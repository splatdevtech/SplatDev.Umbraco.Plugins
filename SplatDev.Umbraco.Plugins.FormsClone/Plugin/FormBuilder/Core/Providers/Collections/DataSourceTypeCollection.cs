using FormBuilder.Core.DataSources;
using FormBuilder.Core.Exceptions;
using FormBuilder.Core.Providers.Builders;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Collections
{
    public class DataSourceTypeCollection(Func<IEnumerable<FormDataSourceType>> items) : BuilderCollectionBase<FormDataSourceType>(CollectionBuilderHelper.GetItemsWithHighestPriority(items().ToArray()))
    {
        public virtual FormDataSourceType this[Guid id]
        {
            get
            {
                FormDataSourceType? formDataSourceType = this.FirstOrDefault(x => x.Id == id);
                if (formDataSourceType is not null)
                    return formDataSourceType;

                DefaultInterpolatedStringHandler interpolatedStringHandler = new(49, 1);
                interpolatedStringHandler.AppendLiteral("Unable to find the Form DataSource with the GUID ");
                interpolatedStringHandler.AppendFormatted(id);
                throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
            }
        }
    }
}