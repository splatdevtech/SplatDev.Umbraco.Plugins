using Umbraco.Cms.Core.Models.PublishedContent;

namespace SplatDev.Umbraco.Common.Extensions
{
    public static class TypeCheckingExtensions
    {
        public static bool AliasEquals<T>(this string alias) where T : PublishedElementModel
        {
            return alias.Equals(typeof(T).Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ExcludedAliases(this string alias, string[] exclusionList)
        {
            return exclusionList.Contains(alias, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
