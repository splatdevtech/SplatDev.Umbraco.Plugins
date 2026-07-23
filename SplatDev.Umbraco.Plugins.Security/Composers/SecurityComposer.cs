using Joonasw.AspNetCore.SecurityHeaders;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace SplatDev.Umbraco.Plugins.Security.Composers
{
    public class SecurityComposer : IComposer
    {
        public readonly TimeSpan? MAX_AGE = new(7, 0, 0, 0);
        public void Compose(IUmbracoBuilder builder)
        {
            string defaultSrc = builder.Config.GetValue<string>("CSP:default-src") ?? "";
            string scriptSrc = builder.Config.GetValue<string>("CSP:script-src") ?? "";
            string fontsSrc = builder.Config.GetValue<string>("CSP:font-src") ?? "";
            string framesSrc = builder.Config.GetValue<string>("CSP:frames-src") ?? "";
            string frameAncestors = builder.Config.GetValue<string>("CSP:frame-ancestors") ?? "";
            string imageSrc = builder.Config.GetValue<string>("CSP:image-src") ?? "";
            string connectionsSrc = builder.Config.GetValue<string>("CSP:connection-src") ?? "";

            builder.Services.AddCsp(nonceByteAmount: 32);
            var isProduction = builder.Config.GetValue<bool>("Umbraco:CMS:HostingEnvironment:IsProduction");
            if (isProduction)
            {
                builder.Services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = MAX_AGE ?? new TimeSpan(30, 0, 0, 0);
                });
            }

            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter("Security")
                {
                    PrePipeline = app =>
                    {
                        app.UseXfo(options => options.SameOrigin());
                        app.UseXContentTypeOptions(new XContentTypeOptionsOptions
                        {
                            AllowSniffing = false
                        });

                        app.Use(async (context, next) =>
                            {
                                context.Response.Headers.Remove("Server");
                                context.Response.Headers.Remove("X-Powered-By");
                                context.Response.Headers.Remove("X-XSS-Protection");
                                context.Response.Headers.Remove("Permissions-Policy");
                                context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
                                await next();
                            });
                        app.UseHsts();
                        app.UseXXssProtection(options => options.EnabledWithBlockMode());
                        if (isProduction)
                        {
                            app.UseHttpsRedirection();
                        }
                        app.UseReferrerPolicy(options =>
                            {
                                options.StrictOriginWhenCrossOrigin();
                            });

                        app.UseCsp(csp =>
                        {
                            var defaultSrcBuilder = csp.ByDefaultAllow.FromSelf();
                            if (!string.IsNullOrWhiteSpace(defaultSrc)) defaultSrcBuilder.From(defaultSrc);
                            var scriptSrcBuilder = csp.AllowScripts.FromSelf().AllowUnsafeEval().AddNonce().AllowUnsafeInline().AddNonce();
                            if (!string.IsNullOrWhiteSpace(scriptSrc)) scriptSrcBuilder.From(scriptSrc);
                            scriptSrcBuilder.AddNonce();
                            csp.AllowStyles.FromAnywhere().AllowUnsafeInline();
                            var imageSrcBuilder = csp.AllowImages.FromAnywhere().DataScheme();
                            if (!string.IsNullOrWhiteSpace(imageSrc)) imageSrcBuilder.From(imageSrc);
                            var fontsSrcBuilder = csp.AllowFonts.FromSelf();
                            if (!string.IsNullOrWhiteSpace(fontsSrc)) fontsSrcBuilder.From(fontsSrc);
                            var framesSrcBuilder = csp.AllowFrames.FromSelf();
                            if (!string.IsNullOrWhiteSpace(framesSrc)) framesSrcBuilder.From(framesSrc);
                            framesSrcBuilder.OnlyOverHttps();
                            var connectionsSrcBuilder = csp.AllowConnections.ToSelf();
                            if (!string.IsNullOrWhiteSpace(connectionsSrc)) connectionsSrcBuilder.To(connectionsSrc);
                            csp.OnSendingHeader = context =>
                            {
                                context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/umbraco");
                                return Task.CompletedTask;
                            };
                        });
                        app.Use(async (context, next) =>
                        {
                            _ = context.Response.Headers.ContentSecurityPolicy.Append($"frame-ancestors {frameAncestors}");
                            await next();
                        });
                    }
                });
            });

            if (builder.Config.GetValue<bool>("DataProtection:Enabled"))
            {

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Running on Windows
                    builder.Services.AddDataProtection()
                        .PersistKeysToFileSystem(new DirectoryInfo(@"C:\temp"))
                        .SetApplicationName("QuoteTabApplication")
                        .ProtectKeysWithDpapi();
                }
                else
                {
                    builder.Services.AddDataProtection()
                        .PersistKeysToFileSystem(new DirectoryInfo(builder.Config.GetValue<string>("DataProtection:PathToPersistKeys")!))
                        .SetApplicationName(builder.Config.GetValue<string>("DataProtection:ApplicationName")!)
                        .ProtectKeysWithCertificate(new X509Certificate2(builder.Config.GetValue<string>("DataProtection:PathToCertificate")!, builder.Config.GetValue<string>("DataProtection:Password")!));
                }
            }
        }
    }
}
