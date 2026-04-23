using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Text.RegularExpressions;
using System.Threading.RateLimiting;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Plugins.RedirectManager.Models;

namespace Umbraco.Plugins.RedirectManager.Composers
{
    public partial class RegexRedirectComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var config = builder.Config.GetSection("RedirectSettings").Get<RedirectConfiguration>();
            config ??= new RedirectConfiguration
            {
                BasePath = "/quotes",
                ValidateParameters = true,
                RateLimitPermit = 10,
                RateLimitWindowSeconds = 60
            }; //if not set in appsettings.json, use defaults

            // Register rate limiting first
            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "-1.-1.-1.-1",
                        partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = config.RateLimitPermit,
                            Window = TimeSpan.FromSeconds(config.RateLimitWindowSeconds)
                        }));
            });

            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter("RedirectMiddleware")
                {
                    PreRouting = app =>
                    {
                        app.UseRewriter(new RewriteOptions().Add(context =>
                        {
                            var request = context.HttpContext.Request;
                            var path = request.Path.Value?.ToLowerInvariant() ?? "";
                            var response = context.HttpContext.Response;

                            // Combined regex patterns
                            var patterns = new[]
                            {
                                new {
                                    Pattern = @"^/quotes/by-(?<author>[a-z0-9-]+)/(?<page>\d+)$",
                                    Replacement = $"{config.BasePath}/by-{{author}}?page={{page}}"
                                },
                                new {
                                    Pattern = @"^/quotes/about-(?<topic>[a-z0-9-]+)/(?<page>\d+)$",
                                    Replacement = $"{config.BasePath}/about-{{topic}}?page={{page}}"
                                },
                                new {
                                    Pattern = @"^/(?<author>[a-z0-9-]+)-quotes-about-(?<topic>[a-z0-9-]+)$",
                                    Replacement = $"{config.BasePath}/by-{{author}}?topic={{topic}}"
                                },
                                new {
                                    Pattern = @"^/authors/(?<letter>[a-zA-Z])/(?<page>\d+)/?$",
                                    Replacement = "/authors/{letter}?page={page}"
                                },
                                new {
                                    Pattern = @"^/top-(?<category>[a-z-]+)-quotes/?$",
                                    Replacement = $"{config.BasePath}/about-{{category}}"
                                }
                            };

                            foreach (var rule in patterns)
                            {
                                var match = Regex.Match(path, rule.Pattern);
                                if (match.Success)
                                {
                                    var parameters = match.Groups
                                        .OfType<Group>()
                                        .ToDictionary(g => g.Name, g => g.Value);

                                    if (config.ValidateParameters)
                                    {
                                        if (parameters.ContainsKey("page") && !int.TryParse(parameters["page"], out _) ||
                                            parameters.ContainsKey("topic") && !BasePattern().IsMatch(parameters["topic"]) ||
                                            parameters.ContainsKey("letter") && !LetterPattern().IsMatch(parameters["letter"]) ||
                                            parameters.ContainsKey("category") && !CategoryPattern().IsMatch(parameters["category"]))
                                        {
                                            return;
                                        }
                                    }

                                    var newPath = rule.Replacement;
                                    foreach (var param in parameters)
                                    {
                                        newPath = newPath.Replace($"{{{param.Key}}}", param.Value);
                                    }

                                    context.HttpContext.Response.Redirect(newPath, permanent: true);
                                    context.Result = RuleResult.EndResponse;
                                    return;
                                }
                            }
                        }));
                    }
                });
            });
        }

        [GeneratedRegex("^[a-z0-9-]+$")]
        public static partial Regex BasePattern();
        [GeneratedRegex("^[a-zA-Z]$")]
        public static partial Regex LetterPattern();
        [GeneratedRegex("^[a-z-]+$")]
        public static partial Regex CategoryPattern();
    }
}