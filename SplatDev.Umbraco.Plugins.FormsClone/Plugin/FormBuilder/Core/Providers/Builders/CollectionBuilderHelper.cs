namespace FormBuilder.Core.Providers.Builders
{
    public static class CollectionBuilderHelper
    {
        public static Func<IEnumerable<T>> GetItemsWithHighestPriority<T>(T[] fieldTypes) where T : ProviderBase
        {
            Type[] first = fieldTypes is not null ? fieldTypes.Select(x => x.GetType()).ToArray() : throw new ArgumentNullException(nameof(fieldTypes));
            List<T> result = [];
            foreach (T fieldType in fieldTypes)
            {
                T current = fieldType;
                Type type = current.GetType();
                if (!first.Except(
                [
                    type
                ]).Any(new Func<Type, bool>(type.IsAssignableFrom)) || fieldTypes.Count(x => x.Id.Equals(current.Id)) == 1)
                    result.Add(current);
            }
            IGrouping<Guid, T>[] array = [.. result.GroupBy(x => x.Id).Where(x => x.Count() > 1)];
            if (array.Length != 0)
                throw new InvalidOperationException("Multiple FieldTypes with the same ID that is not inherited. The IDs: " + string.Join(", ", array.Select(x => x.Key)));
            return () => result;
        }
    }
}