using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.Slider.Models;
using UmbracoCms.Plugins.Slider.Services;

namespace UmbracoCms.Plugins.Slider.Composers;

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
