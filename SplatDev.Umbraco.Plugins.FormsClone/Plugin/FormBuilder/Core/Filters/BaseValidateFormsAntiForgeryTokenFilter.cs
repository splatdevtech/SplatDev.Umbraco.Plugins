using FormBuilder.Core.Configuration;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FormBuilder.Core.Filters
{
    internal abstract class BaseValidateFormsAntiForgeryTokenFilter(
      IOptions<SecuritySettings> config,
      IAntiforgery antiforgery)
    {
        private readonly SecuritySettings _config = config.Value;
        private readonly IAntiforgery _antiforgery = antiforgery;

        protected async Task HandleAuthorizationAsync(
          AuthorizationFilterContext context,
          Func<SecuritySettings, bool> configGetter)
        {
            if (!configGetter(_config))
                return;
            await _antiforgery.ValidateRequestAsync(context.HttpContext);
        }
    }
}