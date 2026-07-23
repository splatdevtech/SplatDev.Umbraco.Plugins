
// Type: Umbraco.Forms.Core.Attributes.BaseValidateFormsAntiForgeryTokenFilter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Configuration;


#nullable enable
namespace Umbraco.Forms.Core.Attributes
{
  internal abstract class BaseValidateFormsAntiForgeryTokenFilter
  {
    private readonly SecuritySettings _config;
    private readonly IAntiforgery _antiforgery;

    protected BaseValidateFormsAntiForgeryTokenFilter(
      IOptions<SecuritySettings> config,
      IAntiforgery antiforgery)
    {
      this._config = config.Value;
      this._antiforgery = antiforgery;
    }

    protected async Task HandleAuthorizationAsync(
      AuthorizationFilterContext context,
      Func<SecuritySettings, bool> configGetter)
    {
      if (!configGetter(this._config))
        return;
      await this._antiforgery.ValidateRequestAsync(context.HttpContext);
    }
  }
}
