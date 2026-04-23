using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.QuickPoll.Models;
using SplatDev.Umbraco.Plugins.QuickPoll.Services;

namespace SplatDev.Umbraco.Plugins.QuickPoll.Composers;

public class QuickPollComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<QuickPollDbContext>(options =>
            options.UseSqlServer(
                builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<IQuickPollService, QuickPollService>();
    }
}
