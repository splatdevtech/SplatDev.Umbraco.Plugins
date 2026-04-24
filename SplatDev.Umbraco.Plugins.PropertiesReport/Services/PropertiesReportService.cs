using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.PropertiesReport.Models;

namespace SplatDev.Umbraco.Plugins.PropertiesReport.Services;

public class PropertiesReportService : IPropertiesReportService
{
    private readonly IContentTypeService _contentTypeService;

    public PropertiesReportService(IContentTypeService contentTypeService)
    {
        _contentTypeService = contentTypeService;
    }

    public PropertiesReportResult GetReport()
    {
        var items = new List<PropertyReportItem>();
        var contentTypes = _contentTypeService.GetAll();

        foreach (var contentType in contentTypes)
        {
            foreach (var group in contentType.PropertyGroups)
            {
                foreach (var property in group.PropertyTypes)
                {
                    items.Add(new PropertyReportItem
                    {
                        ContentTypeName = contentType.Name ?? string.Empty,
                        ContentTypeAlias = contentType.Alias,
                        PropertyName = property.Name ?? string.Empty,
                        PropertyAlias = property.Alias,
                        PropertyEditorAlias = property.PropertyEditorAlias,
                        GroupName = group.Name ?? string.Empty
                    });
                }
            }

            // Properties not in any group
            foreach (var property in contentType.PropertyTypes.Where(p =>
                !contentType.PropertyGroups.SelectMany(g => g.PropertyTypes).Any(gp => gp.Alias == p.Alias)))
            {
                items.Add(new PropertyReportItem
                {
                    ContentTypeName = contentType.Name ?? string.Empty,
                    ContentTypeAlias = contentType.Alias,
                    PropertyName = property.Name ?? string.Empty,
                    PropertyAlias = property.Alias,
                    PropertyEditorAlias = property.PropertyEditorAlias,
                    GroupName = string.Empty
                });
            }
        }

        return new PropertiesReportResult
        {
            Items = items,
            TotalProperties = items.Count,
            TotalContentTypes = contentTypes.Count()
        };
    }

    public PropertiesReportResult GetByContentType(string alias)
    {
        var items = new List<PropertyReportItem>();
        var contentType = _contentTypeService.Get(alias);

        if (contentType is null)
        {
            return new PropertiesReportResult
            {
                Items = items,
                TotalProperties = 0,
                TotalContentTypes = 0
            };
        }

        foreach (var group in contentType.PropertyGroups)
        {
            foreach (var property in group.PropertyTypes)
            {
                items.Add(new PropertyReportItem
                {
                    ContentTypeName = contentType.Name ?? string.Empty,
                    ContentTypeAlias = contentType.Alias,
                    PropertyName = property.Name ?? string.Empty,
                    PropertyAlias = property.Alias,
                    PropertyEditorAlias = property.PropertyEditorAlias,
                    GroupName = group.Name ?? string.Empty
                });
            }
        }

        foreach (var property in contentType.PropertyTypes.Where(p =>
            !contentType.PropertyGroups.SelectMany(g => g.PropertyTypes).Any(gp => gp.Alias == p.Alias)))
        {
            items.Add(new PropertyReportItem
            {
                ContentTypeName = contentType.Name ?? string.Empty,
                ContentTypeAlias = contentType.Alias,
                PropertyName = property.Name ?? string.Empty,
                PropertyAlias = property.Alias,
                PropertyEditorAlias = property.PropertyEditorAlias,
                GroupName = string.Empty
            });
        }

        return new PropertiesReportResult
        {
            Items = items,
            TotalProperties = items.Count,
            TotalContentTypes = 1
        };
    }
}
