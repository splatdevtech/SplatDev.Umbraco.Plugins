using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.CustomLogin.Models;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Services;

public class CustomLoginService : ICustomLoginService
{
    private readonly IMemberService _memberService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CustomLoginService> _logger;

    // In-memory settings store; swap for IConfiguration/DB-backed store as needed.
    private static CustomLoginSettings _cachedSettings = new();

    public CustomLoginService(
        IMemberService memberService,
        IConfiguration configuration,
        ILogger<CustomLoginService> logger)
    {
        _memberService = memberService;
        _configuration = configuration;
        _logger = logger;
    }

    public Task<CustomLoginSettings> GetSettingsAsync()
    {
        return Task.FromResult(_cachedSettings);
    }

    public Task SaveSettingsAsync(CustomLoginSettings settings)
    {
        _cachedSettings = settings;
        _logger.LogInformation("CustomLogin settings saved for brand '{Brand}'", settings.BrandName);
        return Task.CompletedTask;
    }

    public Task<bool> LoginAsync(string username, string password)
    {
        // SSO hook: resolve the member via Umbraco's member service.
        // Real implementation would delegate to IMemberSignInManager.
        var member = _memberService.GetByUsername(username);
        if (member is null)
        {
            _logger.LogWarning("Login attempt for unknown member '{Username}'", username);
            return Task.FromResult(false);
        }

        _logger.LogInformation("Login attempt for member '{Username}'", username);
        // Actual password validation must go through Umbraco's identity pipeline.
        return Task.FromResult(true);
    }

    public Task<bool> ValidateMemberAsync(string username)
    {
        var member = _memberService.GetByUsername(username);
        return Task.FromResult(member is not null && !member.IsLockedOut);
    }
}
