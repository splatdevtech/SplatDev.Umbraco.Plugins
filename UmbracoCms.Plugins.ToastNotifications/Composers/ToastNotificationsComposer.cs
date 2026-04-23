using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.ToastNotifications.Data;
using UmbracoCms.Plugins.ToastNotifications.Services;

namespace UmbracoCms.Plugins.ToastNotifications.Composers;

public class ToastNotificationsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddDbContext<ToastNotificationsDbContext>(o =>
            o.UseSqlServer(builder.Config.GetConnectionString("umbracoDbDSN") ?? string.Empty));

        builder.Services.AddScoped<IToastNotificationsService, ToastNotificationsService>();
    }
}
