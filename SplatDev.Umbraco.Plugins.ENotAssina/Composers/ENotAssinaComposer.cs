using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Polly;
using Polly.Extensions.Http;
using SplatDev.Umbraco.Plugins.ENotAssina.Models;
using SplatDev.Umbraco.Plugins.ENotAssina.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace SplatDev.Umbraco.Plugins.ENotAssina.Composers;

/// <summary>
/// Registers all e-Not Assina plugin services into the Umbraco DI container.
/// Discovered automatically by Umbraco's <see cref="IComposer"/> scan on startup —
/// no explicit registration required in the host application when installed via NuGet.
/// </summary>
public class ENotAssinaComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // ── Serve embedded App_Plugins as static files ─────────────────────────
        // Registers the assembly's embedded resources under /App_Plugins/ENotAssina/
        // so the Lit 3 dashboard.js and umbraco-package.json are accessible at
        // runtime without copying files to wwwroot. Works for both direct project
        // references and NuGet package installs.
        builder.Services.AddOptions<StaticFileOptions>()
            .PostConfigure<IWebHostEnvironment>((opts, environment) =>
            {
                var assembly = typeof(ENotAssinaComposer).Assembly;
                var hasManifest = assembly.GetManifestResourceNames()
                    .Any(n => n.EndsWith("Manifest.xml", StringComparison.Ordinal));

                if (hasManifest)
                {
                    var embeddedProvider = new ManifestEmbeddedFileProvider(assembly, root: "App_Plugins");
                    var baseProvider = opts.FileProvider ?? environment.WebRootFileProvider;
                    opts.FileProvider = new CompositeFileProvider(baseProvider, embeddedProvider);
                }
            });

        // ── Options ────────────────────────────────────────────────────────────
        builder.Services.Configure<ENotAssinaOptions>(
            builder.Config.GetSection(ENotAssinaOptions.SectionKey));

        // ── HTTP client ────────────────────────────────────────────────────────
        builder.Services.AddTransient<ENotAssinaAuthHandler>();

        builder.Services.AddHttpClient(ENotAssinaDefaults.HttpClientName, (sp, client) =>
        {
            var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ENotAssinaOptions>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<ENotAssinaAuthHandler>()
        .AddPolicyHandler(RetryPolicy());

        // ── Core service ───────────────────────────────────────────────────────
        builder.Services.AddScoped<IENotAssinaService, ENotAssinaService>();
    }

    /// <summary>Three retries with exponential back-off on transient HTTP errors.</summary>
    private static IAsyncPolicy<HttpResponseMessage> RetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
}
