namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Models;

public class ExportProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public ExportSelection Selection { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}

public class ExportProfileSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CategorySelection
{
    public bool IncludeAll { get; set; } = true;
    public List<string> Aliases { get; set; } = [];
    public List<int> NodeIds { get; set; } = [];
}

public class ExportSelection
{
    public CategorySelection Languages       { get; set; } = new();
    public CategorySelection DataTypes       { get; set; } = new();
    public CategorySelection DocumentTypes   { get; set; } = new();
    public CategorySelection MediaTypes      { get; set; } = new();
    public CategorySelection Templates       { get; set; } = new();
    public CategorySelection Media           { get; set; } = new();
    public CategorySelection Content         { get; set; } = new();
    public CategorySelection DictionaryItems { get; set; } = new();
    public CategorySelection Members         { get; set; } = new();
    public CategorySelection Users           { get; set; } = new();
}

public class AvailableItem
{
    public string Alias { get; set; } = string.Empty;
    public string Name  { get; set; } = string.Empty;
}

public class AvailableItemsResponse
{
    public List<AvailableItem> DataTypes       { get; set; } = [];
    public List<AvailableItem> DocumentTypes   { get; set; } = [];
    public List<AvailableItem> MediaTypes      { get; set; } = [];
    public List<AvailableItem> Templates       { get; set; } = [];
    public List<AvailableItem> Languages       { get; set; } = [];
    public List<AvailableItem> DictionaryItems { get; set; } = [];
    public List<AvailableItem> Members         { get; set; } = [];
    public List<AvailableItem> Users           { get; set; } = [];
}

public class TreeNode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TreeNode> Children { get; set; } = [];
}
