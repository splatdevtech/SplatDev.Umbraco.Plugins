using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.MemberRegistration.Models;
using SplatDev.Umbraco.Plugins.MemberRegistration.Services;

namespace SplatDev.Umbraco.Plugins.MemberRegistration.Composers;

public class MemberRegistrationComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<MemberRegistrationDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<IMemberRegistrationService, MemberRegistrationService>();
    }
}
