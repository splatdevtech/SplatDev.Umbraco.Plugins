using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using SplatDev.Umbraco.Authorization.Ldap.Middleware;

namespace SplatDev.Umbraco.Authorization.Ldap.StartupFilters
{
    public class LdapStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            app.UseMiddleware<LdapFrontEndAuthorizedMiddleware>();
            next(app);
        };
    }
}
