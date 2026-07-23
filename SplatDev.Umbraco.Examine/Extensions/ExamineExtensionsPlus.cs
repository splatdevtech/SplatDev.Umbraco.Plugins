using Examine;
using Examine.Search;

using Microsoft.Extensions.Logging;

using SplatDev.Umbraco.Examine.Extensions;
using SplatDev.Umbraco.Examine.Models;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;

using static Umbraco.Cms.Core.Constants;

namespace SplatDev.Umbraco.Examine.Extensions
{
    public static class ExamineExtensionsPlus
    {
        private const string Message = "Examine: Exception: {ex} | Message: {msg} | Stack Trace: {stack}";
        public static IExamineValue[]? Boost(this string[] terms, float boost)
        {
            if (terms is null) return null;
            List<IExamineValue> values = [];
            foreach (var item in terms)
                values.Add(item.Boost(boost));

            return [.. values];
        }

        public static IExamineValue[]? Fuzzy(this string[] terms)
        {
            if (terms is null) return null;
            List<IExamineValue> values = [];
            foreach (var item in terms)
                values.Add(item.Fuzzy());

            return [.. values];
        }

        /// <summary>
        /// Searches the specified search results. Only works for one language
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchResults">The search results.</param>
        /// <param name="examineManager">The examine manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="umbracoContextFactory">The umbraco context factory.</param>
        /// <param name="propertyAliases">The property aliases.</param>
        /// <param name="documentTypeAlias">The document type alias.</param>
        /// <returns>Search Results Model</returns>
        /// <exception cref="InvalidOperationException">$"No index found by name {UmbracoIndexes.ExternalIndexName}</exception>
        public static IExamineExtensionsWithResults Search<TController>(this IExamineExtensionsWithResults searchResults,
            IExamineManager examineManager,
            ILogger<TController> logger,
            IUmbracoContextFactory umbracoContextFactory,
            string[] propertyAliases,
            string documentTypeAlias = "") where TController : class
        {
            try
            {
                if (!examineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex index))
                    throw new InvalidOperationException($"No index found by name {UmbracoIndexes.ExternalIndexName}");

                var searcher = index.Searcher;
                var criteria = searcher.CreateQuery(IndexTypes.Content, BooleanOperation.And);
                IBooleanOperation? examineQuery = null;
                if (!string.IsNullOrEmpty(documentTypeAlias)) examineQuery = criteria.NodeTypeAlias(documentTypeAlias);//.And().Field("umbracoNaviHide", "0");
                else examineQuery = criteria.Field("__Published", "y");//.And().Field("umbracoNaviHide", "0");

                if (!string.IsNullOrEmpty(searchResults!.Keywords))
                {
                    if (searchResults.Keywords.Contains(' '))
                    {
                        string[] terms = searchResults.Keywords.Split(' ');
                        examineQuery.And().GroupedOr(propertyAliases, terms);
                    }
                    else
                    {
                        examineQuery.And().GroupedOr(propertyAliases, searchResults.Keywords.MultipleCharacterWildcard());
                    }
                }

                int pageIndex = searchResults.CurrentPage - 1;
                int pageSize = searchResults.ItemsPerPage;

                ISearchResults searchResult = examineQuery.Execute();
                IEnumerable<ISearchResult> pagedResults = searchResult;
                var results = GetContentFromResults(pagedResults, umbracoContextFactory);

                int totalResults = Convert.ToInt32(searchResult.TotalItemCount);
                searchResults.TotalItems = totalResults;
                searchResults.TotalPages = (totalResults + searchResults.ItemsPerPage - 1) / searchResults.ItemsPerPage;
                searchResults.Results = results.Skip(pageIndex * pageSize).Take(searchResults.ItemsPerPage).ToList();
                int lastPage = searchResults.TotalPages;
#pragma warning disable IDE0028 // Simplify collection initialization
                List<int> pageRange = new();
#pragma warning restore IDE0028 // Simplify collection initialization
                if (searchResults.CurrentPage > 1) pageRange.Add(searchResults.CurrentPage - 1);
                pageRange.Add(searchResults.CurrentPage);
                if (searchResults.CurrentPage < searchResults.TotalPages) pageRange.Add(searchResults.CurrentPage + 1);
                searchResults.PageRange = [.. pageRange];

            }
            catch (Exception e)
            {
                logger.LogError(e,
                                Message,
                                e.InnerException != null ? e.InnerException.ToString() : "",
                                e.Message != null ? e.Message.ToString() : "",
                                e.StackTrace);
            }

            return searchResults;
        }

        private static List<IPublishedContent> GetContentFromResults(IEnumerable<ISearchResult> pagedResults, IUmbracoContextFactory umbracoContextFactory)
        {
            List<IPublishedContent> pageDetails = [];
            using (UmbracoContextReference umbracoContextReference = umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (ISearchResult result in pagedResults)
                {
                    if (int.TryParse(result.Id, out int nodeId))
                    {
                        IPublishedContentCache contentHelper = umbracoContextReference.UmbracoContext.Content!;
                        if (contentHelper.GetById(nodeId) is IPublishedContent page)
                            pageDetails.Add(page);
                    }
                }
            }
            var pageDetailsOrdered = pageDetails.OrderByDescending(x => x.Value<DateTime>("publishDate"));
            return pageDetailsOrdered.Select(x => x).ToList();
        }
    }
}
