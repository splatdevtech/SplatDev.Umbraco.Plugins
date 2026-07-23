using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Providers.Builders;

using Umbraco.Cms.Core.Composing;

namespace FormBuilder.Core.Providers.Collections
{
    public class FieldCollection(Func<IEnumerable<FieldType>> items) : BuilderCollectionBase<FieldType>(CollectionBuilderHelper.GetItemsWithHighestPriority(items().ToArray()))
    {
        public FieldType? this[Guid id] => this.FirstOrDefault(x => x.Id == id) ?? null;
    }
}