using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SplatDev.DigitalBookCurator.Core.Context;
using SplatDev.DigitalBookCurator.Core.Models;
using SplatDev.DigitalBookCurator.Core.Repositories;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace SplatDev.Umbraco.Plugins.PdfCurator.Components;

public class CuratorComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        IConfiguration config = builder.Config;
        var curatorSettings = config.GetSection("CuratorSettings");
        config.Bind(curatorSettings);

        CuratorSettings settings = new()
        {
            Origin = config["CuratorSettings:Origin"] ?? "",
            Destination = config["CuratorSettings:Destination"] ?? "",
            DeleteEmptyFolders = bool.Parse(config["CuratorSettings:DeleteEmptyFolders"] ?? "false")
        };
        builder.Services.AddDbContext<CuratorDbContext>(options => options.UseSqlite("name=ConnectionStrings:CuratorDb"));
        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<FileManagerService>();
    }
}
