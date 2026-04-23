using Microsoft.EntityFrameworkCore;
using UmbracoCms.Plugins.Slider.Models;

namespace UmbracoCms.Plugins.Slider.Services;

public class SliderService : ISliderService
{
    private readonly SliderDbContext _db;

    public SliderService(SliderDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<SliderConfig>> GetSlidersAsync()
        => await _db.Sliders.Include(s => s.Slides).ToListAsync();

    public async Task<SliderConfig?> GetSliderAsync(int id)
        => await _db.Sliders.Include(s => s.Slides).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<SliderConfig> CreateSliderAsync(SliderConfig slider)
    {
        _db.Sliders.Add(slider);
        await _db.SaveChangesAsync();
        return slider;
    }

    public async Task<SliderConfig> UpdateSliderAsync(SliderConfig slider)
    {
        _db.Sliders.Update(slider);
        await _db.SaveChangesAsync();
        return slider;
    }

    public async Task DeleteSliderAsync(int id)
    {
        var slider = await _db.Sliders.FindAsync(id);
        if (slider is not null)
        {
            _db.Sliders.Remove(slider);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Slide>> GetSlidesAsync(int sliderId)
        => await _db.Slides.Where(s => s.SliderId == sliderId).OrderBy(s => s.SortOrder).ToListAsync();

    public async Task<Slide> AddSlideAsync(Slide slide)
    {
        _db.Slides.Add(slide);
        await _db.SaveChangesAsync();
        return slide;
    }

    public async Task<Slide> UpdateSlideAsync(Slide slide)
    {
        _db.Slides.Update(slide);
        await _db.SaveChangesAsync();
        return slide;
    }

    public async Task DeleteSlideAsync(int id)
    {
        var slide = await _db.Slides.FindAsync(id);
        if (slide is not null)
        {
            _db.Slides.Remove(slide);
            await _db.SaveChangesAsync();
        }
    }
}
