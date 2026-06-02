using Umbraco.Cms.Web.Common;

var builder = WebApplication.CreateBuilder(args);
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .Build();
var app = builder.Build();
await app.BootUmbracoAsync();
app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndPoints();
        u.UseBackOfficeEndPoints();
        u.UseWebsiteEndPoints();
    });
await app.RunAsync();
