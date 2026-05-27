using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class ExportSelectionFilterTests
{
    [Fact]
    public void CategorySelection_Defaults_IncludeAllTrue_EmptyAliases_EmptyNodeIds()
    {
        var selection = new CategorySelection();

        Assert.True(selection.IncludeAll);
        Assert.Empty(selection.Aliases);
        Assert.Empty(selection.NodeIds);
    }

    [Fact]
    public void CategorySelection_WhenIncludeAllFalse_AliasesAndNodeIdsAreRespected()
    {
        var selection = new CategorySelection
        {
            IncludeAll = false,
            Aliases = ["blogPost", "newsItem"],
            NodeIds = [1001, 1002]
        };

        Assert.False(selection.IncludeAll);
        Assert.Equal(2, selection.Aliases.Count);
        Assert.Contains("blogPost", selection.Aliases);
        Assert.Equal(2, selection.NodeIds.Count);
        Assert.Contains(1001, selection.NodeIds);
    }

    [Fact]
    public void ExportSelection_Defaults_AllTenCategoriesPresent_AllIncludeAll()
    {
        var exportSelection = new ExportSelection();

        Assert.NotNull(exportSelection.Languages);
        Assert.NotNull(exportSelection.DataTypes);
        Assert.NotNull(exportSelection.DocumentTypes);
        Assert.NotNull(exportSelection.MediaTypes);
        Assert.NotNull(exportSelection.Templates);
        Assert.NotNull(exportSelection.Media);
        Assert.NotNull(exportSelection.Content);
        Assert.NotNull(exportSelection.DictionaryItems);
        Assert.NotNull(exportSelection.Members);
        Assert.NotNull(exportSelection.Users);

        Assert.True(exportSelection.Languages.IncludeAll);
        Assert.True(exportSelection.DataTypes.IncludeAll);
        Assert.True(exportSelection.DocumentTypes.IncludeAll);
        Assert.True(exportSelection.MediaTypes.IncludeAll);
        Assert.True(exportSelection.Templates.IncludeAll);
        Assert.True(exportSelection.Media.IncludeAll);
        Assert.True(exportSelection.Content.IncludeAll);
        Assert.True(exportSelection.DictionaryItems.IncludeAll);
        Assert.True(exportSelection.Members.IncludeAll);
        Assert.True(exportSelection.Users.IncludeAll);
    }

    [Fact]
    public void ExportProfile_DefaultId_IsZero()
    {
        var profile = new ExportProfile();
        Assert.Equal(0, profile.Id);
        Assert.False(profile.IsActive);
        Assert.NotNull(profile.Selection);
    }

    [Fact]
    public void CategorySelection_ExcludeAll_WhenFalseAndBothListsEmpty()
    {
        var cs = new CategorySelection { IncludeAll = false, Aliases = [], NodeIds = [] };
        Assert.False(cs.IncludeAll);
        Assert.Empty(cs.Aliases);
        Assert.Empty(cs.NodeIds);
    }
}
