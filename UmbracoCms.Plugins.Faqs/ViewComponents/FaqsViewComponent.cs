using Microsoft.AspNetCore.Mvc;
using UmbracoCms.Plugins.Faqs.Services;

namespace UmbracoCms.Plugins.Faqs.ViewComponents;

public class FaqsViewComponent : ViewComponent
{
    private readonly IFaqsService _service;

    public FaqsViewComponent(IFaqsService service)
    {
        _service = service;
    }

    public async Task<IViewComponentResult> InvokeAsync(
        string? categorySlug = null,
        bool publishedOnly = true,
        string? searchQuery = null)
    {
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var results = await _service.SearchAsync(searchQuery, publishedOnly);
            ViewBag.SearchQuery = searchQuery;
            ViewBag.Categories = await _service.GetCategoriesAsync(publishedOnly);
            ViewBag.IsSearch = true;
            return View(results);
        }

        if (!string.IsNullOrWhiteSpace(categorySlug))
        {
            var category = await _service.GetCategoryBySlugAsync(categorySlug, publishedOnly);
            if (category is null)
            {
                ViewBag.Categories = await _service.GetCategoriesAsync(publishedOnly);
                ViewBag.IsSearch = false;
                return View(Enumerable.Empty<object>());
            }

            ViewBag.Categories = await _service.GetCategoriesAsync(publishedOnly);
            ViewBag.IsSearch = false;
            return View(category.Items);
        }

        // Default: all published items grouped by category
        var categories = await _service.GetCategoriesAsync(publishedOnly);
        ViewBag.Categories = categories;
        ViewBag.IsSearch = false;
        var items = await _service.GetItemsAsync(null, publishedOnly);
        return View(items);
    }
}
