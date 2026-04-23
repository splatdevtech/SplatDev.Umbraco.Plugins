using UmbracoCms.Plugins.Slider.Models;

namespace UmbracoCms.Plugins.Slider.Services;

public interface ISliderService
{
    Task<IEnumerable<SliderConfig>> GetSlidersAsync();
    Task<SliderConfig?> GetSliderAsync(int id);
    Task<SliderConfig> CreateSliderAsync(SliderConfig slider);
    Task<SliderConfig> UpdateSliderAsync(SliderConfig slider);
    Task DeleteSliderAsync(int id);
    Task<IEnumerable<Slide>> GetSlidesAsync(int sliderId);
    Task<Slide> AddSlideAsync(Slide slide);
    Task<Slide> UpdateSlideAsync(Slide slide);
    Task DeleteSlideAsync(int id);
}
