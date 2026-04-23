using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.Slider.Models;
using SplatDev.Umbraco.Plugins.Slider.Services;

namespace SplatDev.Umbraco.Plugins.Slider.Composers;

public class SliderComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        var connectionString = builder.Config.GetSection("ConnectionStrings")["umbracoDbDSN"];

        builder.Services.AddDbContext<SliderDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddScoped<ISliderService, SliderService>();
    }
}
