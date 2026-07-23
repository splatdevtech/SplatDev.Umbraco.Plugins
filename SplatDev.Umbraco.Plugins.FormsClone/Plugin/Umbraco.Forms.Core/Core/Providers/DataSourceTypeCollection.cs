
// Type: Umbraco.Forms.Core.Providers.DataSourceTypeCollection
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Composing;
using Umbraco.Forms.Core.Exceptions;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
    public class DataSourceTypeCollection : BuilderCollectionBase<FormDataSourceType>
    {
        public DataSourceTypeCollection(Func<IEnumerable<FormDataSourceType>> items)
          : base(CollectionBuilderHelper.GetItemsWithHighestPriority<FormDataSourceType>(items().ToArray<FormDataSourceType>()))
        {
        }

        public virtual FormDataSourceType this[Guid id]
        {
            get
            {
                FormDataSourceType formDataSourceType = this.FirstOrDefault<FormDataSourceType>(x => x.Id == id);
                if (formDataSourceType != null)
                    return formDataSourceType;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
                interpolatedStringHandler.AppendLiteral("Unable to find the Form DataSource with the GUID ");
                interpolatedStringHandler.AppendFormatted<Guid>(id);
                throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
            }
        }
    }
}
