using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class FilteredTreeExporterTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private static ContentExporter CreateContentExporter(
        IContentService contentService,
        bool includeContent = true)
    {
        var templateService = Substitute.For<ITemplateService>();
        var options         = Options.Create(new Schema2YamlOptions { IncludeContent = includeContent, MaxHierarchyDepth = 10 });
        var logger          = Substitute.For<ILogger<ContentExporter>>();
        return new ContentExporter(contentService, templateService, options, logger);
    }

    private static IContent CreateFakeContent(int id, string name)
    {
        var contentType = Substitute.For<ISimpleContentType>();
        contentType.Alias.Returns("testDocType");

        var content = Substitute.For<IContent>();
        content.Id.Returns(id);
        content.Name.Returns(name);
        content.ContentType.Returns(contentType);
        content.Published.Returns(true);
        content.SortOrder.Returns(0);
        content.TemplateId.Returns((int?)null);
        content.Properties.Returns(new PropertyCollection([]));
        return content;
    }

    // ── Test 1: ExcludeAll returns empty ─────────────────────────────────────

    [Fact]
    public async Task ExportAsync_ExcludeAll_ReturnsEmpty()
    {
        var contentService = Substitute.For<IContentService>();
        contentService.GetRootContent().Returns([]);

        var sut = CreateContentExporter(contentService);

        var result = await sut.ExportAsync(new CategorySelection { IncludeAll = false, NodeIds = [] });

        Assert.Empty(result);
    }

    // ── Test 2: Selected root node includes all its children ─────────────────

    [Fact]
    public async Task ExportAsync_SelectedRootNode_IncludesChildren()
    {
        var root  = CreateFakeContent(100, "Home");
        var child = CreateFakeContent(101, "About");

        var contentService = Substitute.For<IContentService>();
        contentService.GetRootContent().Returns([root]);

        // root has one child
        contentService
            .GetPagedChildren(100, 0, int.MaxValue, out Arg.Any<long>())
            .Returns(x => { x[3] = 1L; return new[] { child }; });

        // child has no children
        contentService
            .GetPagedChildren(101, 0, int.MaxValue, out Arg.Any<long>())
            .Returns(x => { x[3] = 0L; return Array.Empty<IContent>(); });

        var sut = CreateContentExporter(contentService);

        var result = await sut.ExportAsync(new CategorySelection { IncludeAll = false, NodeIds = [100] });

        Assert.Single(result);
        Assert.Equal("Home", result[0].Name);
        Assert.Single(result[0].Children);
        Assert.Equal("About", result[0].Children[0].Name);
    }
}
