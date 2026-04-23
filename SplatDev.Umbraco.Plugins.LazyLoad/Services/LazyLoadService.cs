using Microsoft.Extensions.Configuration;
using SplatDev.Umbraco.Plugins.LazyLoad.Models;

namespace SplatDev.Umbraco.Plugins.LazyLoad.Services;

public class LazyLoadService : ILazyLoadService
{
    private readonly IConfiguration _configuration;
    private LazyLoadSettings _settings;

    public LazyLoadService(IConfiguration configuration)
    {
        _configuration = configuration;
        _settings = new LazyLoadSettings();

        var section = _configuration.GetSection("LazyLoad");
        if (section.Exists())
        {
            _settings.Enabled = section.GetValue<bool>("Enabled", true);
            _settings.Placeholder = section.GetValue<string>("Placeholder")
                ?? "data:image/gif;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=";
            _settings.LazyLoadIframes = section.GetValue<bool>("LazyLoadIframes", true);
        }
    }

    public LazyLoadSettings GetSettings() => _settings;

    public void SaveSettings(LazyLoadSettings settings)
    {
        _settings = settings;
    }
}
