using Microsoft.AspNetCore.Http;

namespace SplatDev.Umbraco.QueryStringFilters.Extensions
{
    public class QueryStringStripMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        //Middleware to mitigate malicious attacks. ex: /quote/by-luc-de-clapiers/clearness-ornaments-profound-thoughts&title=Clearness%20ornaments%20profound%20thoughts.%20-Luc%20de%20Clapiers
        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            //ignore backOffice urls
            if (path.Contains("/umbraco", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Remove any invalid characters in the path
            var separatorIndex = path.IndexOfAny(['?', '&']);
            if (separatorIndex != -1)
            {
                var cleanPath = path[..separatorIndex];
                context.Request.Path = new PathString(cleanPath);
            }

            // Extract allowed query parameters
            var queryParams = context.Request.Query;

            // Allowed parameters
            var allowedParams = new Dictionary<string, string?>
                {
                    { "random", queryParams["random"].FirstOrDefault() },
                    { "direction", queryParams["direction"].FirstOrDefault() },
                    { "sort", queryParams["sort"].FirstOrDefault() },
                    { "searchDataType", queryParams["searchDataType"].FirstOrDefault() },
                    { "query", queryParams["query"].FirstOrDefault() },
                    { "quoteId", queryParams["quoteId"].FirstOrDefault() },
                    { "profession", queryParams["profession"].FirstOrDefault() },
                    { "nationality", queryParams["nationality"].FirstOrDefault() },
                    { "month", queryParams["month"].FirstOrDefault() },
                    { "day", queryParams["day"].FirstOrDefault() },
                    { "letter", queryParams["letter"].FirstOrDefault() },
                    { "authorSlug", queryParams["authorSlug"].FirstOrDefault() },
                    { "quoteSlug", queryParams["quoteSlug"].FirstOrDefault() },
                    { "PageSize", queryParams["PageSize"].FirstOrDefault() },
                    { "Page", queryParams["Page"].FirstOrDefault() },
                    { "TotalPages", queryParams["TotalPages"].FirstOrDefault() },
                    { "TotalResults", queryParams["TotalResults"].FirstOrDefault() },
                    { "SearchTerm", queryParams["SearchTerm"].FirstOrDefault() },
                    { "SearchDataType", queryParams["SearchDataType"].FirstOrDefault() },
                    { "topic", queryParams["topic"].FirstOrDefault() } // Include topic if needed
                };

            // Rebuild the query string with only allowed parameters
            var newQueryString = QueryString.Empty;
            foreach (var param in allowedParams.Where(p => !string.IsNullOrEmpty(p.Value)))
            {
                newQueryString = newQueryString.Add(param.Key, param.Value ?? "");
            }

            context.Request.QueryString = newQueryString;

            await _next(context);
        }
    }
}
