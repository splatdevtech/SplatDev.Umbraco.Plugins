using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.CustomLogin.Models;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Services;

public class CustomLoginService : ICustomLoginService
{
    private static CustomLoginSettings _cache = new();
    private static bool _loaded;
    private static readonly object _lock = new();

    private readonly IMemberService _memberService;
    private readonly ILogger<CustomLoginService> _logger;
    private readonly string _settingsPath;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public CustomLoginService(
        IMemberService memberService,
        IWebHostEnvironment env,
        ILogger<CustomLoginService> logger)
    {
        _memberService = memberService;
        _logger = logger;
        _settingsPath = Path.Combine(env.ContentRootPath, "umbraco", "Data", "CustomLogin", "settings.json");
        EnsureLoaded();
    }

    public CustomLoginSettings GetSettings() => _cache;

    public Task<CustomLoginSettings> GetSettingsAsync() => Task.FromResult(_cache);

    public Task SaveSettingsAsync(CustomLoginSettings settings)
    {
        lock (_lock)
        {
            _cache = settings;
            var dir = Path.GetDirectoryName(_settingsPath)!;
            Directory.CreateDirectory(dir);
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(_settingsPath, json);
            _logger.LogInformation("CustomLogin settings saved for brand '{Brand}'", settings.BrandName);
        }

        return Task.CompletedTask;
    }

    public Task<bool> LoginAsync(string username, string password)
    {
        var member = _memberService.GetByUsername(username);
        if (member is null)
        {
            _logger.LogWarning("Login attempt for unknown member '{Username}'", username);
            return Task.FromResult(false);
        }

        _logger.LogInformation("Login attempt for member '{Username}'", username);
        return Task.FromResult(true);
    }

    public Task<bool> ValidateMemberAsync(string username)
    {
        var member = _memberService.GetByUsername(username);
        return Task.FromResult(member is not null && !member.IsLockedOut);
    }

    private void EnsureLoaded()
    {
        if (_loaded) return;

        lock (_lock)
        {
            if (_loaded) return;

            if (File.Exists(_settingsPath))
            {
                try
                {
                    var json = File.ReadAllText(_settingsPath);
                    _cache = JsonSerializer.Deserialize<CustomLoginSettings>(json, JsonOptions) ?? new();
                    _logger.LogInformation("CustomLogin settings loaded from {Path}", _settingsPath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load CustomLogin settings from {Path}, using defaults", _settingsPath);
                    _cache = new CustomLoginSettings();
                }
            }

            _loaded = true;
        }
    }
}
