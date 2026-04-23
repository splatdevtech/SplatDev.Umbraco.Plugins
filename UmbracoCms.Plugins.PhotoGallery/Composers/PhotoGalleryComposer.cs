using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoCms.Plugins.PhotoGallery.Models;
using UmbracoCms.Plugins.PhotoGallery.Services;

namespace UmbracoCms.Plugins.PhotoGallery.Composers;

public class PhotoGalleryComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        var connectionString = builder.Config.GetSection("ConnectionStrings")["umbracoDbDSN"];

        builder.Services.AddDbContext<PhotoGalleryDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddScoped<IPhotoGalleryService, PhotoGalleryService>();
    }
}
