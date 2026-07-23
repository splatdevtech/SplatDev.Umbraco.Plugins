namespace SplatDev.Umbraco.Pagination.Extensions
{
    public static class PaginationExtensions
    {
        public static int GetTotalPages(long totalResults, int pageSize)
        {
            var totalPages = (int)Math.Ceiling((decimal)totalResults / pageSize);
            return totalPages > 0 ? totalPages : 1;
        }

        public static IEnumerable<T> PagedResults<T>(this IEnumerable<T> list, int page, int pageSize) where T : class?
        {
            return list.Skip((page - 1) * pageSize).Take(pageSize);
        }

        private static readonly Random rng = new();

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
    }
}
