using UmbracoCms.Plugins.LazyLoad.Models;

namespace UmbracoCms.Plugins.LazyLoad.Services;

public interface ILazyLoadService
{
    LazyLoadSettings GetSettings();
    void SaveSettings(LazyLoadSettings settings);
}
