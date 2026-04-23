using SplatDev.Umbraco.Plugins.LazyLoad.Models;

namespace SplatDev.Umbraco.Plugins.LazyLoad.Services;

public interface ILazyLoadService
{
    LazyLoadSettings GetSettings();
    void SaveSettings(LazyLoadSettings settings);
}
