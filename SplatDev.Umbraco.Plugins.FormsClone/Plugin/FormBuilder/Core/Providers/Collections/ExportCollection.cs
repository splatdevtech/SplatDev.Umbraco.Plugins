using FormBuilder.Core.Exceptions;
using FormBuilder.Core.Export;
using FormBuilder.Core.Providers.Builders;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Collections
{
    public class ExportCollection(Func<IEnumerable<ExportType>> items) : BuilderCollectionBase<ExportType>(CollectionBuilderHelper.GetItemsWithHighestPriority(items().ToArray()))
    {
        public ExportType this[Guid id]
        {
            get
            {
                ExportType? exportType = this.FirstOrDefault(x => x.Id == id);
                if (exportType is not null)
                    return exportType;

                DefaultInterpolatedStringHandler interpolatedStringHandler = new(45, 1);
                interpolatedStringHandler.AppendLiteral("Unable to find the Export Type with the GUID ");
                interpolatedStringHandler.AppendFormatted(id);
                throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
            }
        }
    }
}