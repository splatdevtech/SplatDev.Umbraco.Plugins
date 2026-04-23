namespace FormBuilder.Web.Extensions
{
    /// <summary>Provides extension methods on enumerable collections.</summary>
    public static class EnumerableExtensionMethods
    {
        /// <summary>
        /// Extension method of IEnumerable to confirm if PreValues are unique.
        /// </summary>
        /// <remarks>
        /// See: http://stackoverflow.com/questions/5391264/best-way-to-find-out-if-ienumerable-has-unique-values
        /// </remarks>
        public static bool ContainsUniqueItems<T>(this IEnumerable<T> values)
        {
            HashSet<T> set = new();
            return values.All(item => set.Add(item));
        }
    }
}