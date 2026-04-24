using FormBuilder.Core.Configuration;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FormBuilder.Core.Filters
{
    internal sealed class ValidateFormsApiAntiForgeryTokenFilter(
      IOptions<SecuritySettings> config,
      IAntiforgery antiforgery) :
        BaseValidateFormsAntiForgeryTokenFilter(config, antiforgery),
        IAsyncAuthorizationFilter,
        IFilterMetadata
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context) => await HandleAuthorizationAsync(context, x => x.EnableAntiForgeryTokenForFormsApi);
    }
}