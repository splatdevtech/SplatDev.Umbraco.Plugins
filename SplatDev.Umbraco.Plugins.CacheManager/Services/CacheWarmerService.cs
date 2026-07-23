using HtmlAgilityPack;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;

using SplatDev.Umbraco.Plugins.CacheManager.Models;
using SplatDev.Umbraco.Plugins.CacheManager.Repositories;

namespace SplatDev.Umbraco.Plugins.CacheManager.Services
{
    public class CacheWarmerService(
        ILogger<CacheWarmerService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        CacheWarmerEntryRepository cacheEntryRepository)
    {
        private readonly ILogger<CacheWarmerService> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IConfiguration _configuration = configuration;
        private readonly CacheWarmerEntryRepository _cacheEntryRepository = cacheEntryRepository;
        private readonly ConcurrentDictionary<string, bool> _visitedUrls = new();
        private readonly ConcurrentQueue<(string Url, int Depth)> _urlsToVisit = new();
        private static int task = 0;

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var enabled = _configuration.GetValue("CacheWarmer:Enabled", false);
            if (!enabled) return;

            task = _cacheEntryRepository.GetLatestTask() + 1;

            if (task >= 10)
            {
                _cacheEntryRepository.DeleteAll();
                task = 1;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    try
                    {
                        if (_urlsToVisit.IsEmpty)
                        {
                            // Initialize with the base URL
                            var baseUrl = _configuration["Umbraco:CMS:WebRouting:UmbracoApplicationUrl"];
                            if (string.IsNullOrEmpty(baseUrl))
                            {
                                _logger.LogError("Umbraco Application URL is not configured in appsettings.json");
                                return;
                            }
                            _urlsToVisit.Enqueue((baseUrl, 0));
                        }
                        await WarmCacheAsync(stoppingToken);
                    }
                    finally { }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while initiating cache warming");
                }
            }
        }

        private async Task WarmCacheAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            using var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-cache-warmer", "true");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("QuoteTabCacheWarmerBot/1.0");

            var baseUrl = _configuration["Umbraco:CMS:WebRouting:UmbracoApplicationUrl"] ?? "";
            var maxUrls = _configuration.GetValue("CacheWarmer:MaxUrls", 1000);
            var delayMs = _configuration.GetValue("CacheWarmer:DelayBetweenRequestsMs", 1000);
            var maxDepth = _configuration.GetValue("CacheWarmer:MaxDepth", 5);

            int processedUrls = 0;

            while (_urlsToVisit.TryDequeue(out var urlInfo) && processedUrls < maxUrls && !stoppingToken.IsCancellationRequested)
            {
                var (url, depth) = urlInfo;
                if (_visitedUrls.ContainsKey(url) || depth > maxDepth) continue;

                try
                {
                    var response = await client.GetAsync(url, stoppingToken);
                    response.EnsureSuccessStatusCode();
                    _cacheEntryRepository.AddCacheEntry(new CacheWarmerEntry
                    {
                        Url = url,
                        Description = $"Warmed cache for: {url} (Depth: {depth})",
                        CacheTime = DateTime.Now,
                        Task = task
                    });

                    _visitedUrls.TryAdd(url, true);
                    processedUrls++;

                    if (response.Content.Headers.ContentType?.MediaType == "text/html" && depth < maxDepth)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        var links = doc.DocumentNode.SelectNodes("//a[@href]")
                            ?.Select(a => a.GetAttributeValue("href", ""))
                            .Where(href => !string.IsNullOrEmpty(href))
                            .Select(href => new Uri(new Uri(baseUrl), href).ToString())
                            .Where(u => u.StartsWith(baseUrl) && !_visitedUrls.ContainsKey(u))
                            .Distinct();

                        if (links != null)
                        {
                            foreach (var link in links)
                            {
                                _urlsToVisit.Enqueue((link, depth + 1));
                            }
                        }
                    }

                    await Task.Delay(delayMs, stoppingToken);
                }
                catch (Exception ex)
                {
                    //_logger.LogWarning(ex, "Manually Failed to warm cache for: {Url}", url);
                    _cacheEntryRepository.AddCacheEntry(new CacheWarmerEntry
                    {
                        Url = url,
                        Description = $"Manually Failed to warm cache for: {url} -> {ex.Message}",
                        Task = task
                    });
                }
            }

            _logger.LogInformation("Manually Cache warming completed. Processed {Count} URLs", processedUrls);
        }
    }
}