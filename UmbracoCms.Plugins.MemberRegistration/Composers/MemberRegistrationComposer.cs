using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.MemberRegistration.Models;
using UmbracoCms.Plugins.MemberRegistration.Services;

namespace UmbracoCms.Plugins.MemberRegistration.Composers;

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
