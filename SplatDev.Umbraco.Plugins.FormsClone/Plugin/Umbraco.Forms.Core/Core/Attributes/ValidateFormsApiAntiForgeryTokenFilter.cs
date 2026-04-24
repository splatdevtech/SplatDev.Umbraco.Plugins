
// Type: Umbraco.Forms.Core.Attributes.ValidateFormsApiAntiForgeryTokenFilter
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
  internal sealed class ValidateFormsApiAntiForgeryTokenFilter : 
    BaseValidateFormsAntiForgeryTokenFilter,
    IAsyncAuthorizationFilter,
    IFilterMetadata
  {
    public ValidateFormsApiAntiForgeryTokenFilter(
      IOptions<SecuritySettings> config,
      IAntiforgery antiforgery)
      : base(config, antiforgery)
    {
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context) => await this.HandleAuthorizationAsync(context, (Func<SecuritySettings, bool>) (x => x.EnableAntiForgeryTokenForFormsApi));
  }
}
