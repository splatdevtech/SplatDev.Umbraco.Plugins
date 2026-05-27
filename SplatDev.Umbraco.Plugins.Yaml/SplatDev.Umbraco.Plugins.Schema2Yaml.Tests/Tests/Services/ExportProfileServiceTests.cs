using Microsoft.Extensions.Logging;
using NSubstitute;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Migrations;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using System.Text.Json;
using System.Threading;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class ExportProfileServiceTests
{
    private readonly IUmbracoDatabase _database;
    private readonly IScope _scope;
    private readonly IScopeProvider _scopeProvider;
    private readonly ExportProfileService _sut;

    public ExportProfileServiceTests()
    {
        _database      = Substitute.For<IUmbracoDatabase>();
        _scope         = Substitute.For<IScope>();
        _scope.Database.Returns(_database);
        _scopeProvider = Substitute.For<IScopeProvider>();
        _scopeProvider.CreateScope().Returns(_scope);
        var logger     = Substitute.For<ILogger<ExportProfileService>>();
        _sut           = new ExportProfileService(_scopeProvider, logger);
    }

    [Fact]
    public async Task GetAllAsync_MapsDtosToSummaries()
    {
        var dtoList = new List<ExportProfileDto>
        {
            new() { Id = 1, Name = "Schema Only", IsActive = true,
                    SelectionJson = "{}", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
        };
        _database.FetchAsync<ExportProfileDto>(Arg.Any<string>())
            .Returns(dtoList);

        var result = await _sut.GetAllAsync();

        Assert.Single(result);
        Assert.Equal(1,             result[0].Id);
        Assert.Equal("Schema Only", result[0].Name);
        Assert.True(result[0].IsActive);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsNull_WhenNoActiveProfile()
    {
        var emptyList = new List<ExportProfileDto>();
        _database.FetchAsync<ExportProfileDto>(Arg.Any<string>())
            .Returns(emptyList);

        var result = await _sut.GetActiveAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveAsync_DeserializesSelection()
    {
        var sel = new ExportSelection();
        sel.DocumentTypes.IncludeAll = false;
        sel.DocumentTypes.Aliases    = ["article"];
        var json = JsonSerializer.Serialize(sel,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var dtoList = new List<ExportProfileDto>
        {
            new() { Id = 2, Name = "Test", IsActive = true,
                    SelectionJson = json, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
        };
        _database.FetchAsync<ExportProfileDto>(Arg.Any<string>())
            .Returns(dtoList);

        var result = await _sut.GetActiveAsync();

        Assert.NotNull(result);
        Assert.Equal(2,       result!.Id);
        Assert.Equal("Test",  result.Name);
        Assert.True(result.IsActive);
        Assert.False(result.Selection.DocumentTypes.IncludeAll);
        Assert.Contains("article", result.Selection.DocumentTypes.Aliases);
    }
}
