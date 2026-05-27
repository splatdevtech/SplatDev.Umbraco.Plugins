# Cherry-Pick Export Dialog Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add named export profiles to Schema2Yaml — each specifying which categories and specific entities to include — with a Configure dialog, active-profile badge on the Export button, and server-side persistence in a new Umbraco database table.

**Architecture:** New `schema2yamlExportProfiles` DB table (Umbraco migration pipeline) stores profiles as JSON-serialised `ExportSelection`. `ExportProfileService` handles CRUD + activation via `IScopeProvider`. All 10 exporters gain a `ExportAsync(CategorySelection)` overload applying the filter semantics. The dashboard Lit component gains a two-column `<uui-dialog>`: profile list (left) + category/entity selection (right), and rewires Export/Download buttons to use the active profile's selection when set.

**Tech Stack:** C# / .NET, Umbraco 14–17, NPoco/PetaPoco (`IScopeProvider`), `System.Text.Json`, Umbraco `MigrationBase` + `PackageMigrationPlan`, Lit 3 (`LitElement`), Umbraco UUI components, xUnit + NSubstitute.

---

## File Map

### New files
| File | Responsibility |
|---|---|
| `src/Migrations/AddExportProfilesTable.cs` | NPoco DTO, migration step, migration plan |
| `src/Models/ExportProfileModels.cs` | `ExportProfile`, `ExportSelection`, `CategorySelection`, helper DTOs |
| `src/Services/ExportProfileService.cs` | `IExportProfileService` interface + implementation |
| `src/Controllers/ExportProfileController.cs` | CRUD + activation REST endpoints |
| `src/Controllers/ExportItemsController.cs` | `/available`, `/contenttree`, `/mediatree` |

### Modified files
| File | Change |
|---|---|
| `src/Composers/Schema2YamlComposer.cs` | Register `IExportProfileService` |
| `src/Services/SchemaExportService.cs` | Add `ExportToYamlAsync(ExportSelection)` + `ExportToZipAsync(ExportSelection)` |
| `src/Services/LanguageExporter.cs` | Add `virtual ExportAsync(CategorySelection)` |
| `src/Services/DataTypeExporter.cs` | Add `virtual ExportAsync(CategorySelection)` |
| `src/Services/DocumentTypeExporter.cs` | Add `virtual ExportAsync(CategorySelection)` |
| `src/Services/MediaTypeExporter.cs` | Add `virtual ExportAsync(CategorySelection)` |
| `src/Services/TemplateExporter.cs` | Add `virtual ExportAsync(CategorySelection)` |
| `src/Services/DictionaryExporter.cs` | Add `virtual ExportAsync(CategorySelection)` |
| `src/Services/MemberExporter.cs` | Add `virtual ExportAsync(CategorySelection)` |
| `src/Services/UserExporter.cs` | Add `virtual ExportAsync(CategorySelection)` |
| `src/Services/ContentExporter.cs` | Add `virtual ExportAsync(CategorySelection)` with tree traversal |
| `src/Services/MediaExporter.cs` | Add `virtual ExportAsync(CategorySelection)` with tree traversal |
| `src/Controllers/SchemaExportController.cs` | Add `POST /exportselected`, `POST /downloadyamlselected`, `POST /downloadzipselected` |
| `wwwroot/App_Plugins/Schema2Yaml/dashboard.js` | Configure dialog, active-profile badge, filtered export wiring |

### Test files (new)
| File | Covers |
|---|---|
| `Tests/Services/ExportSelectionFilterTests.cs` | `CategorySelection` filter semantics |
| `Tests/Services/ExportProfileServiceTests.cs` | Service CRUD and activation |
| `Tests/Services/FilteredExporterTests.cs` | `DocumentTypeExporter.ExportAsync(CategorySelection)` |
| `Tests/Services/FilteredTreeExporterTests.cs` | `ContentExporter.ExportAsync(CategorySelection)` |
| `Tests/Integration/FilteredSchemaExportServiceTests.cs` | Orchestration with selection |

---

## Filter semantics (reference for all exporter tasks)

| `IncludeAll` | List | Result |
|---|---|---|
| `true` | any | Return everything |
| `false` | empty | Return `[]` (excluded entirely) |
| `false` | non-empty aliases | Return items whose alias is in the list |
| `false` | non-empty nodeIds (Content/Media) | Return those subtrees + all descendants |

---

## Tasks

---

### Task 1: ExportProfile models

**Files:**
- Create: `src/Models/ExportProfileModels.cs`
- Create: `Tests/Services/ExportSelectionFilterTests.cs`

- [ ] **Step 1: Create models file**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml/src/Models/ExportProfileModels.cs`:

```csharp
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
```

- [ ] **Step 2: Write filter semantics tests**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Services/ExportSelectionFilterTests.cs`:

```csharp
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class ExportSelectionFilterTests
{
    [Fact]
    public void CategorySelection_DefaultsToIncludeAll()
    {
        var cs = new CategorySelection();
        Assert.True(cs.IncludeAll);
        Assert.Empty(cs.Aliases);
        Assert.Empty(cs.NodeIds);
    }

    [Fact]
    public void ExportSelection_AllCategoriesDefaultToIncludeAll()
    {
        var s = new ExportSelection();
        Assert.True(s.Languages.IncludeAll);
        Assert.True(s.DataTypes.IncludeAll);
        Assert.True(s.DocumentTypes.IncludeAll);
        Assert.True(s.MediaTypes.IncludeAll);
        Assert.True(s.Templates.IncludeAll);
        Assert.True(s.Content.IncludeAll);
        Assert.True(s.Media.IncludeAll);
        Assert.True(s.DictionaryItems.IncludeAll);
        Assert.True(s.Members.IncludeAll);
        Assert.True(s.Users.IncludeAll);
    }

    [Fact]
    public void CategorySelection_ExcludeAll_WhenFalseAndEmptyAliases()
    {
        var cs = new CategorySelection { IncludeAll = false, Aliases = [] };
        Assert.False(cs.IncludeAll);
        Assert.Empty(cs.Aliases);
    }

    [Fact]
    public void CategorySelection_FilterByAlias_WhenFalseAndNonEmptyAliases()
    {
        var cs = new CategorySelection { IncludeAll = false, Aliases = ["article", "blog"] };
        Assert.False(cs.IncludeAll);
        Assert.Contains("article", cs.Aliases);
        Assert.Contains("blog", cs.Aliases);
    }
}
```

- [ ] **Step 3: Run tests**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test --filter "ExportSelectionFilterTests" -v minimal
```
Expected: 4 pass.

- [ ] **Step 4: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Models/ExportProfileModels.cs"
git add "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Services/ExportSelectionFilterTests.cs"
git commit -m "feat: add ExportProfile, ExportSelection, CategorySelection models"
```

---

### Task 2: DB migration

**Files:**
- Create: `src/Migrations/AddExportProfilesTable.cs`

- [ ] **Step 1: Create the migration file**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml/src/Migrations/AddExportProfilesTable.cs`:

```csharp
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Migrations;

[TableName("schema2yamlExportProfiles")]
[PrimaryKey("id", autoIncrement: true)]
[ExplicitColumns]
public class ExportProfileDto
{
    [Column("id")]
    [PrimaryKeyColumn(AutoIncrement = true)]
    public int Id { get; set; }

    [Column("name")]
    [Length(255)]
    [NullSetting(NullSetting = NullSettings.NotNull)]
    public string Name { get; set; } = string.Empty;

    [Column("isActive")]
    [NullSetting(NullSetting = NullSettings.NotNull)]
    public bool IsActive { get; set; }

    [Column("selectionJson")]
    [SpecialDbType(SpecialDbTypes.NTEXT)]
    [NullSetting(NullSetting = NullSettings.NotNull)]
    public string SelectionJson { get; set; } = string.Empty;

    [Column("createdDate")]
    [NullSetting(NullSetting = NullSettings.NotNull)]
    public DateTime CreatedDate { get; set; }

    [Column("modifiedDate")]
    [NullSetting(NullSetting = NullSettings.NotNull)]
    public DateTime ModifiedDate { get; set; }
}

public class AddExportProfilesTableMigration : MigrationBase
{
    public AddExportProfilesTableMigration(IMigrationContext context) : base(context) { }

    protected override void Migrate()
    {
        if (!TableExists("schema2yamlExportProfiles"))
            Create.Table<ExportProfileDto>().Do();
    }
}

public class Schema2YamlMigrationPlan : PackageMigrationPlan
{
    public Schema2YamlMigrationPlan() : base("Schema2Yaml") { }

    protected override void DefinePlan()
    {
        From(string.Empty)
            .To<AddExportProfilesTableMigration>("2026-05-15-001-add-export-profiles");
    }
}
```

- [ ] **Step 2: Build**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml"
dotnet build
```
Expected: 0 errors. If NPoco annotation types are missing, add `using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;` (check usings in other model files for the exact namespace).

- [ ] **Step 3: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Migrations/AddExportProfilesTable.cs"
git commit -m "feat: add DB migration for schema2yamlExportProfiles table"
```

---

### Task 3: ExportProfileService

**Files:**
- Create: `src/Services/ExportProfileService.cs`
- Create: `Tests/Services/ExportProfileServiceTests.cs`

- [ ] **Step 1: Write failing tests**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Services/ExportProfileServiceTests.cs`:

```csharp
using Microsoft.Extensions.Logging;
using NSubstitute;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Migrations;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using System.Text.Json;
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
        _database     = Substitute.For<IUmbracoDatabase>();
        _scope        = Substitute.For<IScope>();
        _scope.Database.Returns(_database);
        _scopeProvider = Substitute.For<IScopeProvider>();
        _scopeProvider.CreateScope().Returns(_scope);
        var logger    = Substitute.For<ILogger<ExportProfileService>>();
        _sut          = new ExportProfileService(_scopeProvider, logger);
    }

    [Fact]
    public async Task GetAllAsync_MapsDtosToSummaries()
    {
        _database.FetchAsync<ExportProfileDto>(Arg.Any<string>())
            .Returns(new List<ExportProfileDto>
            {
                new() { Id = 1, Name = "Schema Only", IsActive = true,
                        SelectionJson = "{}", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
            });

        var result = await _sut.GetAllAsync();

        Assert.Single(result);
        Assert.Equal(1,             result[0].Id);
        Assert.Equal("Schema Only", result[0].Name);
        Assert.True(result[0].IsActive);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsNull_WhenNoActiveProfile()
    {
        _database.FetchAsync<ExportProfileDto>(Arg.Any<string>())
            .Returns(new List<ExportProfileDto>());

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

        _database.FetchAsync<ExportProfileDto>(Arg.Any<string>())
            .Returns(new List<ExportProfileDto>
            {
                new() { Id = 2, Name = "Test", IsActive = true,
                        SelectionJson = json, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
            });

        var result = await _sut.GetActiveAsync();

        Assert.NotNull(result);
        Assert.False(result!.Selection.DocumentTypes.IncludeAll);
        Assert.Contains("article", result.Selection.DocumentTypes.Aliases);
    }
}
```

- [ ] **Step 2: Run to verify failure**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test --filter "ExportProfileServiceTests" -v minimal
```
Expected: Build error — `ExportProfileService` does not exist yet.

- [ ] **Step 3: Create the service**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/ExportProfileService.cs`:

```csharp
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Migrations;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

public interface IExportProfileService
{
    Task<List<ExportProfileSummary>> GetAllAsync();
    Task<ExportProfile?> GetActiveAsync();
    Task<ExportProfile> GetByIdAsync(int id);
    Task<ExportProfile> CreateAsync(string name, ExportSelection selection);
    Task<ExportProfile> UpdateAsync(int id, string name, ExportSelection selection);
    Task DeleteAsync(int id);
    Task ActivateAsync(int id);
    Task DeactivateAsync();
}

public class ExportProfileService : IExportProfileService
{
    private readonly IScopeProvider _scopeProvider;
    private readonly ILogger<ExportProfileService> _logger;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExportProfileService(IScopeProvider scopeProvider, ILogger<ExportProfileService> logger)
    {
        _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        _logger        = logger        ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<ExportProfileSummary>> GetAllAsync()
    {
        using var scope = _scopeProvider.CreateScope();
        var dtos = await scope.Database.FetchAsync<ExportProfileDto>(
            "SELECT id, name, isActive FROM schema2yamlExportProfiles ORDER BY name");
        scope.Complete();
        return dtos.Select(d => new ExportProfileSummary
            { Id = d.Id, Name = d.Name, IsActive = d.IsActive }).ToList();
    }

    public async Task<ExportProfile?> GetActiveAsync()
    {
        using var scope = _scopeProvider.CreateScope();
        var dtos = await scope.Database.FetchAsync<ExportProfileDto>(
            "SELECT * FROM schema2yamlExportProfiles WHERE isActive = 1");
        scope.Complete();
        return dtos.Count == 0 ? null : Map(dtos[0]);
    }

    public async Task<ExportProfile> GetByIdAsync(int id)
    {
        using var scope = _scopeProvider.CreateScope();
        var dto = await scope.Database.SingleOrDefaultAsync<ExportProfileDto>(
            "SELECT * FROM schema2yamlExportProfiles WHERE id = @0", id);
        scope.Complete();
        if (dto is null) throw new KeyNotFoundException($"Export profile {id} not found");
        return Map(dto);
    }

    public async Task<ExportProfile> CreateAsync(string name, ExportSelection selection)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var now = DateTime.UtcNow;
        var dto = new ExportProfileDto
        {
            Name         = name,
            IsActive     = false,
            SelectionJson = JsonSerializer.Serialize(selection, _json),
            CreatedDate  = now,
            ModifiedDate = now
        };
        using var scope = _scopeProvider.CreateScope();
        await scope.Database.InsertAsync(dto);
        scope.Complete();
        _logger.LogInformation("Created export profile: {Name} (id={Id})", name, dto.Id);
        return Map(dto);
    }

    public async Task<ExportProfile> UpdateAsync(int id, string name, ExportSelection selection)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        using var scope = _scopeProvider.CreateScope();
        var dto = await scope.Database.SingleOrDefaultAsync<ExportProfileDto>(
            "SELECT * FROM schema2yamlExportProfiles WHERE id = @0", id)
            ?? throw new KeyNotFoundException($"Export profile {id} not found");
        dto.Name         = name;
        dto.SelectionJson = JsonSerializer.Serialize(selection, _json);
        dto.ModifiedDate = DateTime.UtcNow;
        await scope.Database.UpdateAsync(dto);
        scope.Complete();
        return Map(dto);
    }

    public async Task DeleteAsync(int id)
    {
        using var scope = _scopeProvider.CreateScope();
        await scope.Database.ExecuteAsync(
            "DELETE FROM schema2yamlExportProfiles WHERE id = @0", id);
        scope.Complete();
        _logger.LogInformation("Deleted export profile id={Id}", id);
    }

    public async Task ActivateAsync(int id)
    {
        using var scope = _scopeProvider.CreateScope();
        var count = await scope.Database.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM schema2yamlExportProfiles WHERE id = @0", id);
        if (count == 0) throw new KeyNotFoundException($"Export profile {id} not found");
        await scope.Database.ExecuteAsync(
            "UPDATE schema2yamlExportProfiles SET isActive = 0");
        await scope.Database.ExecuteAsync(
            "UPDATE schema2yamlExportProfiles SET isActive = 1, modifiedDate = @0 WHERE id = @1",
            DateTime.UtcNow, id);
        scope.Complete();
        _logger.LogInformation("Activated export profile id={Id}", id);
    }

    public async Task DeactivateAsync()
    {
        using var scope = _scopeProvider.CreateScope();
        await scope.Database.ExecuteAsync(
            "UPDATE schema2yamlExportProfiles SET isActive = 0");
        scope.Complete();
    }

    private static ExportProfile Map(ExportProfileDto dto) => new()
    {
        Id           = dto.Id,
        Name         = dto.Name,
        IsActive     = dto.IsActive,
        Selection    = JsonSerializer.Deserialize<ExportSelection>(dto.SelectionJson, _json) ?? new(),
        CreatedDate  = dto.CreatedDate,
        ModifiedDate = dto.ModifiedDate
    };
}
```

- [ ] **Step 4: Run tests**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test --filter "ExportProfileServiceTests" -v minimal
```
Expected: 3 pass. If `FetchAsync` / `InsertAsync` are not on `IUmbracoDatabase` in your Umbraco version, use the sync variants `Fetch` / `Insert` instead and remove the `await`.

- [ ] **Step 5: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/ExportProfileService.cs"
git add "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Services/ExportProfileServiceTests.cs"
git commit -m "feat: add IExportProfileService and ExportProfileService"
```

---

### Task 4: ExportProfileController

**Files:**
- Create: `src/Controllers/ExportProfileController.cs`

- [ ] **Step 1: Create the controller**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml/src/Controllers/ExportProfileController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Controllers;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Controllers;

[Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
public class ExportProfileController : UmbracoApiController
{
    private readonly IExportProfileService _profiles;
    private readonly ILogger<ExportProfileController> _logger;

    public ExportProfileController(IExportProfileService profiles, ILogger<ExportProfileController> logger)
    {
        _profiles = profiles ?? throw new ArgumentNullException(nameof(profiles));
        _logger   = logger   ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET /umbraco/api/exportprofile/list
    [HttpGet]
    public async Task<IActionResult> List()
    {
        try { return Ok(await _profiles.GetAllAsync()); }
        catch (Exception ex) { return Err(ex, "list profiles"); }
    }

    // GET /umbraco/api/exportprofile/active
    [HttpGet]
    public async Task<IActionResult> Active()
    {
        try
        {
            var p = await _profiles.GetActiveAsync();
            return p is null ? NoContent() : Ok(p);
        }
        catch (Exception ex) { return Err(ex, "get active profile"); }
    }

    // GET /umbraco/api/exportprofile/get/{id}
    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        try { return Ok(await _profiles.GetByIdAsync(id)); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (Exception ex) { return Err(ex, $"get profile {id}"); }
    }

    // POST /umbraco/api/exportprofile/create
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
            return BadRequest(new { error = "Name is required" });
        try { return Ok(await _profiles.CreateAsync(request.Name, request.Selection ?? new())); }
        catch (Exception ex) { return Err(ex, "create profile"); }
    }

    // PUT /umbraco/api/exportprofile/update/{id}
    [HttpPut]
    public async Task<IActionResult> Update(int id, [FromBody] ProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
            return BadRequest(new { error = "Name is required" });
        try { return Ok(await _profiles.UpdateAsync(id, request.Name, request.Selection ?? new())); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (Exception ex) { return Err(ex, $"update profile {id}"); }
    }

    // DELETE /umbraco/api/exportprofile/delete/{id}
    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        try { await _profiles.DeleteAsync(id); return NoContent(); }
        catch (Exception ex) { return Err(ex, $"delete profile {id}"); }
    }

    // POST /umbraco/api/exportprofile/activate/{id}
    [HttpPost]
    public async Task<IActionResult> Activate(int id)
    {
        try { await _profiles.ActivateAsync(id); return Ok(); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (Exception ex) { return Err(ex, $"activate profile {id}"); }
    }

    // POST /umbraco/api/exportprofile/deactivate
    [HttpPost]
    public async Task<IActionResult> Deactivate()
    {
        try { await _profiles.DeactivateAsync(); return Ok(); }
        catch (Exception ex) { return Err(ex, "deactivate profile"); }
    }

    private ObjectResult Err(Exception ex, string op)
    {
        _logger.LogError(ex, "Failed to {Op}", op);
        return StatusCode(500, new { error = $"Failed to {op}", message = ex.Message });
    }
}

public class ProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public ExportSelection? Selection { get; set; }
}
```

- [ ] **Step 2: Build**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml"
dotnet build
```
Expected: 0 errors.

- [ ] **Step 3: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Controllers/ExportProfileController.cs"
git commit -m "feat: add ExportProfileController (CRUD + activation endpoints)"
```

---

### Task 5: ExportItemsController

**Files:**
- Create: `src/Controllers/ExportItemsController.cs`

- [ ] **Step 1: Create the controller**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml/src/Controllers/ExportItemsController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Controllers;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Controllers;

[Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
public class ExportItemsController : UmbracoApiController
{
    private readonly IDataTypeService    _dataTypes;
    private readonly IContentTypeService _contentTypes;
    private readonly IMediaTypeService   _mediaTypes;
    private readonly IFileService        _files;
    private readonly ILocalizationService _localization;
    private readonly IContentService     _content;
    private readonly IMediaService       _media;
    private readonly IMemberService      _members;
    private readonly IUserService        _users;
    private readonly ILogger<ExportItemsController> _logger;

    public ExportItemsController(
        IDataTypeService dataTypes, IContentTypeService contentTypes,
        IMediaTypeService mediaTypes, IFileService files,
        ILocalizationService localization, IContentService content,
        IMediaService media, IMemberService members,
        IUserService users, ILogger<ExportItemsController> logger)
    {
        _dataTypes    = dataTypes;
        _contentTypes = contentTypes;
        _mediaTypes   = mediaTypes;
        _files        = files;
        _localization = localization;
        _content      = content;
        _media        = media;
        _members      = members;
        _users        = users;
        _logger       = logger;
    }

    // GET /umbraco/api/exportitems/available
    [HttpGet]
    public IActionResult Available()
    {
        try
        {
            return Ok(new AvailableItemsResponse
            {
                DataTypes = _dataTypes.GetAll()
                    .Select(dt => new AvailableItem { Alias = dt.Name, Name = dt.Name })
                    .OrderBy(x => x.Name).ToList(),

                DocumentTypes = _contentTypes.GetAll()
                    .Select(ct => new AvailableItem { Alias = ct.Alias, Name = ct.Name ?? ct.Alias })
                    .OrderBy(x => x.Name).ToList(),

                MediaTypes = _mediaTypes.GetAll()
                    .Select(mt => new AvailableItem { Alias = mt.Alias, Name = mt.Name ?? mt.Alias })
                    .OrderBy(x => x.Name).ToList(),

                Templates = _files.GetTemplates()
                    .Select(t => new AvailableItem { Alias = t.Alias, Name = t.Name ?? t.Alias })
                    .OrderBy(x => x.Name).ToList(),

                Languages = _localization.GetAllLanguages()
                    .Select(l => new AvailableItem { Alias = l.IsoCode, Name = l.CultureName ?? l.IsoCode })
                    .OrderBy(x => x.Name).ToList(),

                DictionaryItems = _localization.GetDictionaryItemDescendants(null)
                    .Select(d => new AvailableItem { Alias = d.ItemKey, Name = d.ItemKey })
                    .OrderBy(x => x.Name).ToList(),

                Members = _members.GetAllMembers()
                    .Select(m => new AvailableItem { Alias = m.Email, Name = m.Name ?? m.Email })
                    .OrderBy(x => x.Name).ToList(),

                Users = _users.GetAll(0, int.MaxValue, out _)
                    .Select(u => new AvailableItem { Alias = u.Email, Name = u.Name ?? u.Email })
                    .OrderBy(x => x.Name).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available items");
            return StatusCode(500, new { error = "Failed to get available items", message = ex.Message });
        }
    }

    // GET /umbraco/api/exportitems/contenttree
    [HttpGet]
    public IActionResult ContentTree()
    {
        try
        {
            var roots = _content.GetRootContent();
            return Ok(roots.Select(BuildContentNode).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get content tree");
            return StatusCode(500, new { error = "Failed to get content tree", message = ex.Message });
        }
    }

    // GET /umbraco/api/exportitems/mediatree
    [HttpGet]
    public IActionResult MediaTree()
    {
        try
        {
            var roots = _media.GetRootMedia();
            return Ok(roots.Select(BuildMediaNode).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get media tree");
            return StatusCode(500, new { error = "Failed to get media tree", message = ex.Message });
        }
    }

    private TreeNode BuildContentNode(IContent n) => new()
    {
        Id       = n.Id,
        Name     = n.Name ?? "(unnamed)",
        Children = _content.GetPagedChildren(n.Id, 0, int.MaxValue, out _)
                           .Select(BuildContentNode).ToList()
    };

    private TreeNode BuildMediaNode(IMedia n) => new()
    {
        Id       = n.Id,
        Name     = n.Name ?? "(unnamed)",
        Children = _media.GetPagedChildren(n.Id, 0, int.MaxValue, out _)
                         .Select(BuildMediaNode).ToList()
    };
}
```

- [ ] **Step 2: Build**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml"
dotnet build
```
Expected: 0 errors. If `_members.GetAllMembers()` or `_users.GetAll()` differ in your Umbraco version, check the interface signature and adjust accordingly.

- [ ] **Step 3: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Controllers/ExportItemsController.cs"
git commit -m "feat: add ExportItemsController (available items + content/media trees)"
```

---

### Task 6: Register in Composer

**Files:**
- Modify: `src/Composers/Schema2YamlComposer.cs`

- [ ] **Step 1: Open the file and add the IExportProfileService registration**

Read `src/Composers/Schema2YamlComposer.cs`. Inside the `Compose` method, after the existing `AddScoped` calls, add:

```csharp
builder.Services.AddScoped<IExportProfileService, ExportProfileService>();
```

Also add the using at the top of the file if not already present:
```csharp
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
```

The `Schema2YamlMigrationPlan` (from `src/Migrations/AddExportProfilesTable.cs`) and `ExportItemsController` are both auto-discovered by Umbraco from the assembly — no explicit registration needed.

- [ ] **Step 2: Build**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml"
dotnet build
```
Expected: 0 errors.

- [ ] **Step 3: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Composers/Schema2YamlComposer.cs"
git commit -m "feat: register IExportProfileService in DI"
```

---

### Task 7: Filtered overloads for 8 flat exporters

**Files:**
- Modify: `src/Services/LanguageExporter.cs`
- Modify: `src/Services/DataTypeExporter.cs`
- Modify: `src/Services/DocumentTypeExporter.cs`
- Modify: `src/Services/MediaTypeExporter.cs`
- Modify: `src/Services/TemplateExporter.cs`
- Modify: `src/Services/DictionaryExporter.cs`
- Modify: `src/Services/MemberExporter.cs`
- Modify: `src/Services/UserExporter.cs`
- Create: `Tests/Services/FilteredExporterTests.cs`

The overload pattern is identical for all flat exporters. Mark the method `virtual` so it can be substituted in integration tests.

**Template (replace `T`, `ExportXxx`, and the alias field per exporter):**
```csharp
public virtual async Task<List<ExportXxx>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.Aliases.Count == 0)
        return [];
    var all = await ExportAsync();
    if (filter.IncludeAll) return all;
    return all.Where(x => filter.Aliases.Contains(x.<AliasField>)).ToList();
}
```

**Alias fields per exporter:**
| Exporter | Return type | Alias field |
|---|---|---|
| `LanguageExporter` | `ExportLanguage` | `IsoCode` |
| `DataTypeExporter` | `ExportDataType` | `Alias` |
| `DocumentTypeExporter` | `ExportDocumentType` | `Alias` |
| `MediaTypeExporter` | `ExportMediaType` | `Alias` |
| `TemplateExporter` | `ExportTemplate` | `Alias` |
| `DictionaryExporter` | `ExportDictionaryItem` | `Key` |
| `MemberExporter` | `ExportMember` | `Email` |
| `UserExporter` | `ExportUser` | `Email` |

- [ ] **Step 1: Write failing test (using DocumentTypeExporter as representative)**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Services/FilteredExporterTests.cs`:

```csharp
using Microsoft.Extensions.Logging;
using NSubstitute;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class FilteredExporterTests
{
    private readonly IContentTypeService _cts;
    private readonly IDataTypeService    _dts;
    private readonly DocumentTypeExporter _sut;

    public FilteredExporterTests()
    {
        _cts = Substitute.For<IContentTypeService>();
        _dts = Substitute.For<IDataTypeService>();
        var logger = Substitute.For<ILogger<DocumentTypeExporter>>();

        IContentType Make(string alias, string name)
        {
            var ct = Substitute.For<IContentType>();
            ct.Alias.Returns(alias);
            ct.Name.Returns(name);
            ct.Icon.Returns("icon");
            ct.IsElement.Returns(false);
            ct.AllowedAsRoot.Returns(false);
            ct.AllowedContentTypes.Returns(Array.Empty<ContentTypeSort>());
            ct.ContentTypeComposition.Returns(Array.Empty<IContentTypeComposition>());
            ct.AllowedTemplates.Returns(Array.Empty<ITemplate>());
            ct.DefaultTemplate.Returns((ITemplate?)null);
            ct.PropertyGroups.Returns(new PropertyGroupCollection(Array.Empty<PropertyGroup>()));
            ct.PropertyTypes.Returns(Array.Empty<IPropertyType>());
            return ct;
        }

        _cts.GetAll().Returns(new[] { Make("article", "Article"), Make("blog", "Blog") });
        _sut = new DocumentTypeExporter(_cts, _dts, logger);
    }

    [Fact]
    public async Task Filter_IncludeAll_ReturnsAll()
    {
        var result = await _sut.ExportAsync(new CategorySelection { IncludeAll = true });
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Filter_ExcludeAll_ReturnsEmpty()
    {
        var result = await _sut.ExportAsync(new CategorySelection { IncludeAll = false, Aliases = [] });
        Assert.Empty(result);
    }

    [Fact]
    public async Task Filter_SpecificAlias_ReturnsOnlyMatch()
    {
        var result = await _sut.ExportAsync(
            new CategorySelection { IncludeAll = false, Aliases = ["article"] });
        Assert.Single(result);
        Assert.Equal("article", result[0].Alias);
    }
}
```

- [ ] **Step 2: Run to verify failure**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test --filter "FilteredExporterTests" -v minimal
```
Expected: Build error — `ExportAsync(CategorySelection)` overload does not exist.

- [ ] **Step 3: Add overload to DocumentTypeExporter**

In `src/Services/DocumentTypeExporter.cs`, add after the existing `ExportAsync()` method:

```csharp
public virtual async Task<List<ExportDocumentType>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.Aliases.Count == 0)
        return [];
    var all = await ExportAsync();
    if (filter.IncludeAll) return all;
    return all.Where(dt => filter.Aliases.Contains(dt.Alias)).ToList();
}
```

Add `using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;` at the top if not present.

- [ ] **Step 4: Run tests**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test --filter "FilteredExporterTests" -v minimal
```
Expected: 3 pass.

- [ ] **Step 5: Add overloads to the remaining 7 flat exporters**

Apply the template to each file. Copy the exact method below into each exporter after its existing `ExportAsync()`.

**`LanguageExporter.cs`:**
```csharp
public virtual async Task<List<ExportLanguage>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.Aliases.Count == 0) return [];
    var all = await ExportAsync();
    if (filter.IncludeAll) return all;
    return all.Where(l => filter.Aliases.Contains(l.IsoCode)).ToList();
}
```

**`DataTypeExporter.cs`:**
```csharp
public virtual async Task<List<ExportDataType>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.Aliases.Count == 0) return [];
    var all = await ExportAsync();
    if (filter.IncludeAll) return all;
    return all.Where(dt => filter.Aliases.Contains(dt.Alias)).ToList();
}
```

**`MediaTypeExporter.cs`:**
```csharp
public virtual async Task<List<ExportMediaType>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.Aliases.Count == 0) return [];
    var all = await ExportAsync();
    if (filter.IncludeAll) return all;
    return all.Where(mt => filter.Aliases.Contains(mt.Alias)).ToList();
}
```

**`TemplateExporter.cs`:**
```csharp
public virtual async Task<List<ExportTemplate>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.Aliases.Count == 0) return [];
    var all = await ExportAsync();
    if (filter.IncludeAll) return all;
    return all.Where(t => filter.Aliases.Contains(t.Alias)).ToList();
}
```

**`DictionaryExporter.cs`:**
```csharp
public virtual async Task<List<ExportDictionaryItem>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.Aliases.Count == 0) return [];
    var all = await ExportAsync();
    if (filter.IncludeAll) return all;
    return all.Where(d => filter.Aliases.Contains(d.Key)).ToList();
}
```

**`MemberExporter.cs`:**
```csharp
public virtual async Task<List<ExportMember>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.Aliases.Count == 0) return [];
    var all = await ExportAsync();
    if (filter.IncludeAll) return all;
    return all.Where(m => filter.Aliases.Contains(m.Email)).ToList();
}
```

**`UserExporter.cs`:**
```csharp
public virtual async Task<List<ExportUser>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.Aliases.Count == 0) return [];
    var all = await ExportAsync();
    if (filter.IncludeAll) return all;
    return all.Where(u => filter.Aliases.Contains(u.Email)).ToList();
}
```

- [ ] **Step 6: Build**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml"
dotnet build
```
Expected: 0 errors.

- [ ] **Step 7: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/LanguageExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/DataTypeExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/DocumentTypeExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/MediaTypeExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/TemplateExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/DictionaryExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/MemberExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/UserExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Services/FilteredExporterTests.cs"
git commit -m "feat: add CategorySelection filter overloads to all 8 flat exporters"
```

---

### Task 8: Filtered overloads for Content and Media exporters

**Files:**
- Modify: `src/Services/ContentExporter.cs`
- Modify: `src/Services/MediaExporter.cs`
- Create: `Tests/Services/FilteredTreeExporterTests.cs`

Rule: when a node's ID is in `NodeIds`, export it and all its descendants fully. Non-selected nodes are traversed only to find selected descendants.

- [ ] **Step 1: Write failing tests**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Services/FilteredTreeExporterTests.cs`:

```csharp
using Microsoft.Extensions.Logging;
using NSubstitute;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class FilteredTreeExporterTests
{
    private readonly IContentService _contentService;
    private readonly IFileService    _fileService;

    public FilteredTreeExporterTests()
    {
        _contentService = Substitute.For<IContentService>();
        _fileService    = Substitute.For<IFileService>();
    }

    private static IContent FakeNode(int id, string name, string typeAlias)
    {
        var ct = Substitute.For<IContentType>();
        ct.Alias.Returns(typeAlias);
        var n = Substitute.For<IContent>();
        n.Id.Returns(id);
        n.Name.Returns(name);
        n.ContentType.Returns(ct);
        n.SortOrder.Returns(0);
        n.Published.Returns(true);
        n.TemplateId.Returns((int?)null);
        n.Properties.Returns(new PropertyCollection());
        return n;
    }

    [Fact]
    public async Task ExportAsync_ExcludeAll_ReturnsEmpty()
    {
        _contentService.GetRootContent().Returns(Array.Empty<IContent>());
        var sut = new ContentExporter(_contentService, _fileService,
            Substitute.For<ILogger<ContentExporter>>());

        var result = await sut.ExportAsync(
            new CategorySelection { IncludeAll = false, NodeIds = [] });

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExportAsync_SelectedRootNode_IncludesChildren()
    {
        var root  = FakeNode(100, "Home",  "home");
        var child = FakeNode(101, "About", "page");

        _contentService.GetRootContent().Returns(new[] { root });
        _contentService.GetPagedChildren(100, 0, int.MaxValue, out Arg.Any<long>())
            .Returns(x => { x[3] = 1L; return new[] { child }; });
        _contentService.GetPagedChildren(101, 0, int.MaxValue, out Arg.Any<long>())
            .Returns(x => { x[3] = 0L; return Array.Empty<IContent>(); });

        var sut = new ContentExporter(_contentService, _fileService,
            Substitute.For<ILogger<ContentExporter>>());

        var result = await sut.ExportAsync(
            new CategorySelection { IncludeAll = false, NodeIds = [100] });

        Assert.Single(result);
        Assert.Equal("Home", result[0].Name);
        Assert.Single(result[0].Children);
        Assert.Equal("About", result[0].Children[0].Name);
    }
}
```

- [ ] **Step 2: Run to verify failure**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test --filter "FilteredTreeExporterTests" -v minimal
```
Expected: Build error — `ContentExporter.ExportAsync(CategorySelection)` does not exist.

- [ ] **Step 3: Add overload to ContentExporter**

Open `src/Services/ContentExporter.cs`. The existing class has `ExportAsync()` and a private `ExportNode(IContent)`. Add after `ExportAsync()`:

```csharp
public virtual async Task<List<ExportContent>> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.NodeIds.Count == 0)
        return [];

    if (filter.IncludeAll)
        return await ExportAsync();

    _logger.LogInformation("Filtered content export: {Count} node IDs", filter.NodeIds.Count);

    var result = new List<ExportContent>();
    foreach (var root in _contentService.GetRootContent())
        CollectFilteredContent(root, filter.NodeIds, result);

    return result;
}

private void CollectFilteredContent(IContent node, List<int> nodeIds, List<ExportContent> result)
{
    if (nodeIds.Contains(node.Id))
    {
        result.Add(ExportNode(node)); // exports node + all descendants
        return;
    }
    // traverse children looking for selected nodes
    foreach (var child in _contentService.GetPagedChildren(node.Id, 0, int.MaxValue, out _))
        CollectFilteredContent(child, nodeIds, result);
}
```

Add `using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;` if not already present.

Note: `_contentService` and `ExportNode` are existing private fields/methods in `ContentExporter` — verify their exact names match the current file before saving.

- [ ] **Step 4: Add overload to MediaExporter**

Open `src/Services/MediaExporter.cs`. The existing class has `ExportAsync()` returning `(List<ExportMedia>, Dictionary<string, byte[]>)` and a private `ExportMediaItem(...)`. Add after `ExportAsync()`:

```csharp
public virtual async Task<(List<ExportMedia>, Dictionary<string, byte[]>)> ExportAsync(CategorySelection filter)
{
    if (!filter.IncludeAll && filter.NodeIds.Count == 0)
        return ([], []);

    if (filter.IncludeAll)
        return await ExportAsync();

    _logger.LogInformation("Filtered media export: {Count} node IDs", filter.NodeIds.Count);

    var exported = new List<ExportMedia>();
    var files    = new Dictionary<string, byte[]>();
    foreach (var root in _mediaService.GetRootMedia())
        CollectFilteredMedia(root, filter.NodeIds, exported, files, string.Empty);

    return (exported, files);
}

private void CollectFilteredMedia(
    Umbraco.Cms.Core.Models.IMedia node,
    List<int> nodeIds,
    List<ExportMedia> exported,
    Dictionary<string, byte[]> files,
    string folder)
{
    if (nodeIds.Contains(node.Id))
    {
        ExportMediaItem(node, exported, files, folder); // exports node + all descendants
        return;
    }
    foreach (var child in _mediaService.GetPagedChildren(node.Id, 0, int.MaxValue, out _))
        CollectFilteredMedia(child, nodeIds, exported, files, folder);
}
```

Verify `_mediaService`, `ExportMediaItem`, and its signature match the current file exactly.

- [ ] **Step 5: Run tests**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test --filter "FilteredTreeExporterTests" -v minimal
```
Expected: 2 pass. If `ContentExporter` constructor signature differs from what the test uses, adjust the `new ContentExporter(...)` call in the test to match.

- [ ] **Step 6: Build**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml"
dotnet build
```
Expected: 0 errors.

- [ ] **Step 7: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/ContentExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/MediaExporter.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Services/FilteredTreeExporterTests.cs"
git commit -m "feat: add NodeIds filter overloads to ContentExporter and MediaExporter"
```

---

### Task 9: Filtered SchemaExportService

**Files:**
- Modify: `src/Services/SchemaExportService.cs`
- Create: `Tests/Integration/FilteredSchemaExportServiceTests.cs`

- [ ] **Step 1: Write failing test**

Create `SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Integration/FilteredSchemaExportServiceTests.cs`:

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Configuration;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using SplatDev.Umbraco.Plugins.Schema2Yaml.Services;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Integration;

public class FilteredSchemaExportServiceTests
{
    private readonly DocumentTypeExporter _docTypeExporter;
    private readonly SchemaExportService  _sut;

    public FilteredSchemaExportServiceTests()
    {
        // Substituting concrete classes requires virtual methods — added in Task 7/8
        var langExp  = Substitute.For<LanguageExporter>();
        var dtExp    = Substitute.For<DataTypeExporter>();
        _docTypeExporter = Substitute.For<DocumentTypeExporter>();
        var mtExp    = Substitute.For<MediaTypeExporter>();
        var tmplExp  = Substitute.For<TemplateExporter>();
        var mediaExp = Substitute.For<MediaExporter>();
        var cntExp   = Substitute.For<ContentExporter>();
        var dictExp  = Substitute.For<DictionaryExporter>();
        var mbrExp   = Substitute.For<MemberExporter>();
        var usrExp   = Substitute.For<UserExporter>();
        var verDet   = Substitute.For<UmbracoVersionDetector>();
        verDet.GetVersionString().Returns("14.0.0");

        langExp.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportLanguage>()));
        dtExp.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportDataType>()));
        _docTypeExporter.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportDocumentType>()));
        mtExp.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportMediaType>()));
        tmplExp.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportTemplate>()));
        mediaExp.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult<(List<ExportMedia>, Dictionary<string, byte[]>)>(([], [])));
        cntExp.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportContent>()));
        dictExp.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportDictionaryItem>()));
        mbrExp.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportMember>()));
        usrExp.ExportAsync(Arg.Any<CategorySelection>())
            .Returns(Task.FromResult(new List<ExportUser>()));

        _sut = new SchemaExportService(
            langExp, dtExp, _docTypeExporter, mtExp, tmplExp,
            mediaExp, cntExp, dictExp, mbrExp, usrExp,
            verDet, Options.Create(new Schema2YamlOptions()),
            Substitute.For<ILogger<SchemaExportService>>());
    }

    [Fact]
    public async Task ExportToYamlAsync_WithSelection_PassesFilterToExporters()
    {
        var selection = new ExportSelection();
        selection.DocumentTypes.IncludeAll = false;
        selection.DocumentTypes.Aliases    = ["article"];

        await _sut.ExportToYamlAsync(selection);

        await _docTypeExporter.Received(1).ExportAsync(
            Arg.Is<CategorySelection>(f => !f.IncludeAll && f.Aliases.Contains("article")));
    }
}
```

- [ ] **Step 2: Run to verify failure**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test --filter "FilteredSchemaExportServiceTests" -v minimal
```
Expected: Build error — `SchemaExportService.ExportToYamlAsync(ExportSelection)` does not exist.

- [ ] **Step 3: Add filtered overloads to SchemaExportService**

In `src/Services/SchemaExportService.cs`, update the `ISchemaExportService` interface to add:

```csharp
Task<string> ExportToYamlAsync(ExportSelection selection);
Task<byte[]> ExportToZipAsync(ExportSelection selection);
```

Add both methods to `SchemaExportService` after `ExportToYamlAsync()`:

```csharp
public async Task<string> ExportToYamlAsync(ExportSelection selection)
{
    _logger.LogInformation("Starting filtered schema export");
    var startTime = DateTime.UtcNow;

    var languages     = await _languageExporter.ExportAsync(selection.Languages);
    var dataTypes     = await _dataTypeExporter.ExportAsync(selection.DataTypes);
    var documentTypes = await _documentTypeExporter.ExportAsync(selection.DocumentTypes);
    var mediaTypes    = await _mediaTypeExporter.ExportAsync(selection.MediaTypes);
    var templates     = await _templateExporter.ExportAsync(selection.Templates);
    var (media, _)    = await _mediaExporter.ExportAsync(selection.Media);
    var content       = await _contentExporter.ExportAsync(selection.Content);
    var dictionary    = await _dictionaryExporter.ExportAsync(selection.DictionaryItems);
    var members       = await _memberExporter.ExportAsync(selection.Members);
    var users         = await _userExporter.ExportAsync(selection.Users);

    var root = new ExportRoot
    {
        Umbraco = new UmbracoExport
        {
            Languages = languages, DataTypes = dataTypes,
            DocumentTypes = documentTypes, MediaTypes = mediaTypes,
            Templates = templates, Media = media, Content = content,
            DictionaryItems = dictionary, Members = members, Users = users
        }
    };

    _lastStatistics = new ExportStatistics
    {
        ExportDate = DateTime.UtcNow, UmbracoVersion = _versionDetector.GetVersionString(),
        LanguageCount = languages.Count, DataTypeCount = dataTypes.Count,
        DocumentTypeCount = documentTypes.Count, MediaTypeCount = mediaTypes.Count,
        TemplateCount = templates.Count, MediaCount = media.Count,
        ContentCount = content.Count, DictionaryItemCount = dictionary.Count,
        MemberCount = members.Count, UserCount = users.Count,
        Duration = DateTime.UtcNow - startTime
    };

    var serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
    return serializer.Serialize(root);
}

public async Task<byte[]> ExportToZipAsync(ExportSelection selection)
{
    var yaml              = await ExportToYamlAsync(selection);
    var (_, mediaFiles)   = await _mediaExporter.ExportAsync(selection.Media);

    using var mem = new MemoryStream();
    using (var zip = new ZipArchive(mem, ZipArchiveMode.Create, true))
    {
        var entry = zip.CreateEntry("umbraco.yml", CompressionLevel.Optimal);
        using (var w = new StreamWriter(entry.Open(), Encoding.UTF8))
            await w.WriteAsync(yaml);

        foreach (var (path, bytes) in mediaFiles)
        {
            var me = zip.CreateEntry($"media/{path}", CompressionLevel.Optimal);
            using var s = me.Open();
            await s.WriteAsync(bytes);
        }
    }

    mem.Position = 0;
    return mem.ToArray();
}
```

The usings `SerializerBuilder`, `CamelCaseNamingConvention`, `ZipArchive`, `MemoryStream`, `Encoding`, `CompressionLevel` are already present in the file from the existing `ExportToYamlAsync()` / `ExportToZipAsync()` — do not duplicate them.

- [ ] **Step 4: Run tests**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test --filter "FilteredSchemaExportServiceTests" -v minimal
```
Expected: 1 pass. If substituting concrete exporter classes fails (NSubstitute requires virtual methods), confirm that all `ExportAsync(CategorySelection)` overloads added in Tasks 7 and 8 are marked `virtual`.

- [ ] **Step 5: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Services/SchemaExportService.cs" \
        "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests/Tests/Integration/FilteredSchemaExportServiceTests.cs"
git commit -m "feat: add ExportToYamlAsync(ExportSelection) and ExportToZipAsync(ExportSelection) to SchemaExportService"
```

---

### Task 10: Filtered endpoints in SchemaExportController

**Files:**
- Modify: `src/Controllers/SchemaExportController.cs`

- [ ] **Step 1: Add three POST endpoints**

In `src/Controllers/SchemaExportController.cs`, add after the existing `DownloadZip` method:

```csharp
// POST /umbraco/api/schemaexport/exportselected
[HttpPost]
public async Task<IActionResult> ExportSelected([FromBody] ExportSelection? selection)
{
    try
    {
        _logger.LogInformation("Dashboard: Filtered export requested");
        selection ??= new ExportSelection();
        var yaml  = await _exportService.ExportToYamlAsync(selection);
        var stats = _exportService.GetLastExportStatistics();
        return Ok(new
        {
            yaml,
            statistics = new
            {
                exportDate      = stats.ExportDate,
                umbracoVersion  = stats.UmbracoVersion,
                languages       = stats.LanguageCount,
                dataTypes       = stats.DataTypeCount,
                documentTypes   = stats.DocumentTypeCount,
                mediaTypes      = stats.MediaTypeCount,
                templates       = stats.TemplateCount,
                media           = stats.MediaCount,
                content         = stats.ContentCount,
                dictionaryItems = stats.DictionaryItemCount,
                members         = stats.MemberCount,
                users           = stats.UserCount,
                durationSeconds = stats.Duration.TotalSeconds
            }
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Filtered export failed");
        return StatusCode(500, new { error = "Filtered export failed", message = ex.Message });
    }
}

// POST /umbraco/api/schemaexport/downloadyamlselected
[HttpPost]
public async Task<IActionResult> DownloadYamlSelected([FromBody] ExportSelection? selection)
{
    try
    {
        selection ??= new ExportSelection();
        var yaml  = await _exportService.ExportToYamlAsync(selection);
        var bytes = Encoding.UTF8.GetBytes(yaml);
        return File(bytes, "application/x-yaml",
            $"umbraco-selected-{DateTime.UtcNow:yyyyMMdd-HHmmss}.yml");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Filtered YAML download failed");
        return StatusCode(500, new { error = "Download failed", message = ex.Message });
    }
}

// POST /umbraco/api/schemaexport/downloadzipselected
[HttpPost]
public async Task<IActionResult> DownloadZipSelected([FromBody] ExportSelection? selection)
{
    try
    {
        selection ??= new ExportSelection();
        var zip = await _exportService.ExportToZipAsync(selection);
        return File(zip, "application/zip",
            $"umbraco-selected-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Filtered ZIP download failed");
        return StatusCode(500, new { error = "ZIP creation failed", message = ex.Message });
    }
}
```

Add `using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;` if not present. `Encoding` is already used in the file.

- [ ] **Step 2: Build**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml"
dotnet build
```
Expected: 0 errors.

- [ ] **Step 3: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/src/Controllers/SchemaExportController.cs"
git commit -m "feat: add POST /exportselected, /downloadyamlselected, /downloadzipselected endpoints"
```

---

### Task 11: Dashboard — state, configure button, active-profile badge

**Files:**
- Modify: `wwwroot/App_Plugins/Schema2Yaml/dashboard.js`

**Note on auth helper:** The existing `_fetchAuthenticated(path)` prepends `API_BASE` (`/umbraco/api/SchemaExport`). New endpoints live under different base paths. This task adds `_fetchWithAuth(url, options)` that sends auth headers without prepending a base path, and uses it for all new API calls.

- [ ] **Step 1: Add _fetchWithAuth helper**

Add this method to the class alongside the existing `_fetchAuthenticated`:

```js
async _fetchWithAuth(url, options = {}) {
    const headers = { 'Content-Type': 'application/json', ...(options.headers ?? {}) };
    if (this._authContext) {
        const token = await this._authContext.getLatestToken();
        if (token) headers['Authorization'] = `Bearer ${token}`;
    }
    return fetch(url, { ...options, headers });
}
```

- [ ] **Step 2: Extend static properties**

In `static properties = { ... }`, add:

```js
_profiles:         { state: true },
_activeProfile:    { state: true },
_showConfigDialog: { state: true },
```

- [ ] **Step 3: Initialise in constructor**

In `constructor()`, add:

```js
this._profiles         = [];
this._activeProfile    = null;
this._showConfigDialog = false;
```

- [ ] **Step 4: Load active profile on mount**

In `connectedCallback`, after `this._loadStatistics()` add:

```js
this._loadActiveProfile();
```

Add the method:

```js
async _loadActiveProfile() {
    try {
        const res = await this._fetchWithAuth('/umbraco/api/ExportProfile/Active');
        if (res.status === 204) { this._activeProfile = null; return; }
        if (res.ok) this._activeProfile = await res.json();
    } catch { /* silently ignore */ }
}
```

- [ ] **Step 5: Add _deactivateProfile**

```js
async _deactivateProfile(e) {
    e.stopPropagation();
    try {
        await this._fetchWithAuth('/umbraco/api/ExportProfile/Deactivate', { method: 'POST' });
        this._activeProfile = null;
        this._notify('positive', 'Filter cleared', 'Next export will include everything.');
    } catch (err) {
        this._notify('danger', 'Failed to clear filter', err.message ?? 'Unknown error');
    }
}
```

- [ ] **Step 6: Update _runExport to use active profile**

Replace the existing `_runExport` method with:

```js
async _runExport() {
    if (this._loading) return;
    this._loading  = true;
    this._hasExport = false;
    this._stats    = null;
    this._yaml     = null;
    this._yamlPreview = null;

    try {
        let res;
        if (this._activeProfile) {
            res = await this._fetchWithAuth('/umbraco/api/SchemaExport/ExportSelected', {
                method: 'POST',
                body: JSON.stringify(this._activeProfile.selection),
            });
        } else {
            res = await this._fetchAuthenticated('/Export');
        }

        if (!res.ok) {
            const err = await res.json().catch(() => ({ message: res.statusText }));
            throw new Error(err.message ?? res.statusText);
        }

        const data = await res.json();
        this._stats = data.statistics;
        this._applyYaml(data.yaml);
        this._notify('positive', 'Export complete', 'Schema exported successfully.');
    } catch (e) {
        this._notify('danger', 'Export failed', e.message ?? 'An unexpected error occurred.');
    } finally {
        this._loading = false;
    }
}
```

- [ ] **Step 7: Update _downloadYaml and _downloadZip**

Add `_triggerDownloadPost` helper:

```js
async _triggerDownloadPost(url, body) {
    try {
        const res = await this._fetchWithAuth(url, {
            method: 'POST',
            body: JSON.stringify(body),
        });
        if (!res.ok) throw new Error(`Server returned ${res.status}: ${res.statusText}`);
        const blob = await res.blob();
        const cd   = res.headers.get('Content-Disposition') ?? '';
        const match = cd.match(/filename[^;=\n]*=["']?([^"';\n]+)/i);
        let name = match ? match[1].trim() : (url.includes('zip') ? 'umbraco-export.zip' : 'umbraco-export.yml');
        if (typeof window.showSaveFilePicker === 'function') {
            const ext = name.split('.').pop();
            const types = ext === 'zip'
                ? [{ description: 'ZIP archive', accept: { 'application/zip': ['.zip'] } }]
                : [{ description: 'YAML file',   accept: { 'application/x-yaml': ['.yml','.yaml'] } }];
            try {
                const handle = await window.showSaveFilePicker({ suggestedName: name, types });
                const w = await handle.createWritable();
                await w.write(blob); await w.close(); return;
            } catch (pe) { if (pe.name === 'AbortError') return; }
        }
        const obj = URL.createObjectURL(blob);
        const a = Object.assign(document.createElement('a'), { href: obj, download: name, style: 'display:none' });
        document.body.appendChild(a); a.click(); document.body.removeChild(a);
        setTimeout(() => URL.revokeObjectURL(obj), 100);
    } catch (e) {
        this._notify('danger', 'Download failed', e.message ?? 'Could not download file.');
    }
}
```

Replace `_downloadYaml`:
```js
async _downloadYaml() {
    if (this._activeProfile) {
        await this._triggerDownloadPost(
            '/umbraco/api/SchemaExport/DownloadYamlSelected', this._activeProfile.selection);
    } else {
        await this._triggerDownload(`${API_BASE}/DownloadYaml`);
    }
}
```

Replace `_downloadZip`:
```js
async _downloadZip() {
    if (this._downloadingZip) return;
    this._downloadingZip = true;
    try {
        if (this._activeProfile) {
            await this._triggerDownloadPost(
                '/umbraco/api/SchemaExport/DownloadZipSelected', this._activeProfile.selection);
        } else {
            await this._triggerDownload(`${API_BASE}/DownloadZip`);
        }
    } finally {
        setTimeout(() => { this._downloadingZip = false; }, 2000);
    }
}
```

- [ ] **Step 8: Update render() with Configure button and active-profile badge**

Replace the `render()` actions section:

```js
render() {
    const exportLabel = this._activeProfile
        ? `Export (${this._activeProfile.name})`
        : (this._loading ? 'Exporting…' : 'Export to YAML');

    return html`
        <div class="header">
            <h1>Schema Export</h1>
            <p>Export your Umbraco site structure to YAML for version control and migration.</p>
        </div>

        <div class="actions">
            <uui-button look="primary" color="default"
                label=${exportLabel} ?disabled=${this._loading}
                @click=${this._runExport}>
                ${this._loading ? html`<uui-loader-circle></uui-loader-circle>` : nothing}
                ${exportLabel}
                ${this._activeProfile ? html`
                    <span style="margin-left:6px;opacity:.7;font-size:11px"
                          @click=${this._deactivateProfile}
                          title="Clear filter — export everything">✕</span>` : nothing}
            </uui-button>

            <uui-button look="secondary" color="default" label="Download YAML"
                ?disabled=${!this._hasExport || this._loading}
                @click=${this._downloadYaml}>
                Download YAML
            </uui-button>

            <uui-button look="secondary" color="default"
                label=${this._downloadingZip ? 'Preparing ZIP…' : 'Download ZIP (with media)'}
                ?disabled=${this._downloadingZip || !this._hasExport}
                @click=${this._downloadZip}>
                ${this._downloadingZip ? html`<uui-loader-circle></uui-loader-circle>` : nothing}
                ${this._downloadingZip ? 'Preparing ZIP…' : 'Download ZIP (with media)'}
            </uui-button>

            <uui-button look="secondary" color="default"
                label="Configure Export" @click=${this._openConfigDialog}>
                Configure Export
            </uui-button>
        </div>

        ${this._renderStats()}
        ${this._renderPreview()}
        ${this._showConfigDialog ? this._renderConfigDialog() : nothing}
    `;
}
```

Add stubs (filled in Tasks 12–15):
```js
_openConfigDialog()  { this._showConfigDialog = true; }
_closeConfigDialog() { this._showConfigDialog = false; }
_renderConfigDialog() { return html`<div></div>`; }
```

- [ ] **Step 9: Build and smoke-test**

Start Umbraco, open Settings → Schema Export. Verify:
- "Configure Export" button visible
- No active profile: Export button shows "Export to YAML"
- Clicking "Configure Export" doesn't crash

- [ ] **Step 10: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/wwwroot/App_Plugins/Schema2Yaml/dashboard.js"
git commit -m "feat: add configure button, active-profile badge, and filtered export wiring"
```

---

### Task 12: Dashboard — configure dialog + profile list

**Files:**
- Modify: `wwwroot/App_Plugins/Schema2Yaml/dashboard.js`

- [ ] **Step 1: Add new state to static properties**

```js
_editingProfileId:   { state: true },
_editingProfileName: { state: true },
_configuring:        { state: true },
_loadingItems:       { state: true },
_availableItems:     { state: true },
_contentTree:        { state: true },
_mediaTree:          { state: true },
_expandedCategories: { state: true },
_expandedTreeNodes:  { state: true },
```

- [ ] **Step 2: Initialise in constructor**

```js
this._editingProfileId   = null;
this._editingProfileName = '';
this._configuring        = this._defaultSelection();
this._loadingItems       = false;
this._availableItems     = null;
this._contentTree        = null;
this._mediaTree          = null;
this._expandedCategories = new Set();
this._expandedTreeNodes  = new Set();
```

- [ ] **Step 3: Add _defaultSelection helper**

```js
_defaultSelection() {
    const cat = () => ({ includeAll: true, aliases: [], nodeIds: [] });
    return {
        languages: cat(), dataTypes: cat(), documentTypes: cat(),
        mediaTypes: cat(), templates: cat(), media: cat(),
        content: cat(), dictionaryItems: cat(), members: cat(), users: cat()
    };
}
```

- [ ] **Step 4: Add dialog CSS to static styles**

```css
.config-overlay {
    position: fixed; inset: 0;
    background: rgba(0,0,0,.4); z-index: 1000;
    display: flex; align-items: center; justify-content: center;
}
.config-dialog {
    background: var(--uui-color-surface,#fff);
    border-radius: var(--uui-border-radius,4px);
    box-shadow: 0 8px 32px rgba(0,0,0,.18);
    width: 900px; max-width: 96vw; max-height: 90vh;
    display: flex; flex-direction: column; overflow: hidden;
}
.config-header {
    display: flex; align-items: center; justify-content: space-between;
    padding: 16px 24px;
    border-bottom: 1px solid var(--uui-color-border,#e3e3e3);
    font-weight: 600; font-size: 16px;
}
.config-body { display: flex; flex: 1; overflow: hidden; }
.config-profiles {
    width: 210px; min-width: 170px;
    border-right: 1px solid var(--uui-color-border,#e3e3e3);
    display: flex; flex-direction: column; overflow-y: auto; padding: 16px; gap: 6px;
}
.config-selection { flex: 1; overflow-y: auto; padding: 16px 24px; }
.config-footer {
    display: flex; gap: 8px; justify-content: flex-end;
    padding: 12px 24px;
    border-top: 1px solid var(--uui-color-border,#e3e3e3);
}
.profile-item {
    padding: 7px 10px; border-radius: 4px; cursor: pointer;
    display: flex; align-items: center; gap: 6px; font-size: 13px;
}
.profile-item:hover { background: var(--uui-color-surface-alt,#f5f5f5); }
.profile-item.active-profile { background: var(--uui-color-selected,#e3edff); font-weight: 500; }
.profile-dot {
    width: 8px; height: 8px; border-radius: 50%;
    background: var(--uui-color-positive,#2e7d32); flex-shrink: 0;
}
.profile-name-input {
    width: 100%; padding: 8px; box-sizing: border-box;
    border: 1px solid var(--uui-color-border,#ddd);
    border-radius: 4px; font-size: 14px; margin-bottom: 16px;
}
.section-label {
    font-size: 11px; font-weight: 600; text-transform: uppercase;
    color: var(--uui-color-text-alt,#888); letter-spacing: .05em; margin-bottom: 8px;
}
.cat-row {
    display: flex; align-items: flex-start; gap: 8px;
    padding: 8px 0; border-bottom: 1px solid var(--uui-color-border,#f0f0f0);
}
.cat-row:last-child { border-bottom: none; }
.cat-name { font-weight: 500; font-size: 14px; }
.cat-meta { font-size: 12px; color: var(--uui-color-text-alt,#888); margin-left: 4px; }
.filter-toggle {
    font-size: 11px; color: var(--uui-color-interactive,#1b264f);
    cursor: pointer; margin-top: 4px; display: inline-block;
}
.entity-list { display: flex; flex-wrap: wrap; gap: 6px; margin-top: 8px; }
.chip {
    display: inline-flex; align-items: center; gap: 4px;
    padding: 4px 10px; border-radius: 16px; font-size: 12px;
    border: 1px solid var(--uui-color-border,#ddd);
    cursor: pointer; user-select: none;
    background: var(--uui-color-surface,#fff);
}
.chip.selected {
    background: var(--uui-color-selected,#e3edff);
    border-color: var(--uui-color-interactive,#1b264f);
    color: var(--uui-color-interactive,#1b264f);
}
```

- [ ] **Step 5: Implement _openConfigDialog**

Replace the stub:

```js
async _openConfigDialog() {
    this._showConfigDialog = true;
    await this._loadProfiles();
    if (this._activeProfile) {
        this._editingProfileId   = this._activeProfile.id;
        this._editingProfileName = this._activeProfile.name;
        this._configuring        = JSON.parse(JSON.stringify(this._activeProfile.selection));
    } else if (this._profiles.length > 0) {
        await this._selectProfile(this._profiles[0].id);
    } else {
        this._newProfile();
    }
    if (!this._availableItems) await this._fetchAvailableItems();
}

async _loadProfiles() {
    try {
        const res = await this._fetchWithAuth('/umbraco/api/ExportProfile/List');
        if (res.ok) this._profiles = await res.json();
    } catch { /* silently ignore */ }
}

async _selectProfile(id) {
    try {
        const res = await this._fetchWithAuth(`/umbraco/api/ExportProfile/Get/${id}`);
        if (!res.ok) return;
        const p = await res.json();
        this._editingProfileId   = p.id;
        this._editingProfileName = p.name;
        this._configuring        = JSON.parse(JSON.stringify(p.selection));
    } catch { /* silently ignore */ }
}

_newProfile() {
    this._editingProfileId   = null;
    this._editingProfileName = '';
    this._configuring        = this._defaultSelection();
}
```

- [ ] **Step 6: Implement _renderConfigDialog**

Replace the stub:

```js
_renderConfigDialog() {
    return html`
        <div class="config-overlay"
             @click=${(e) => { if (e.target === e.currentTarget) this._closeConfigDialog(); }}>
            <div class="config-dialog">
                <div class="config-header">
                    Configure Export
                    <uui-button look="secondary" compact @click=${this._closeConfigDialog}>✕</uui-button>
                </div>

                <div class="config-body">
                    <div class="config-profiles">
                        <div class="section-label">Profiles</div>
                        <uui-button look="secondary" compact @click=${this._newProfile}>
                            + New profile
                        </uui-button>
                        <hr style="border:none;border-top:1px solid var(--uui-color-border,#e3e3e3);margin:4px 0">

                        ${this._profiles.map(p => html`
                            <div class="profile-item ${this._editingProfileId === p.id ? 'active-profile' : ''}"
                                 @click=${() => this._selectProfile(p.id)}>
                                ${p.isActive ? html`<span class="profile-dot"></span>` : nothing}
                                ${p.name}
                            </div>`)}

                        ${this._editingProfileId !== null ? html`
                            <hr style="border:none;border-top:1px solid var(--uui-color-border,#e3e3e3);margin:8px 0">
                            <uui-button look="secondary" color="danger" compact
                                        @click=${this._deleteProfile}>
                                Delete
                            </uui-button>` : nothing}
                    </div>

                    <div class="config-selection">
                        ${this._renderSelectionPanel()}
                    </div>
                </div>

                <div class="config-footer">
                    <uui-button look="secondary" @click=${this._closeConfigDialog}>Cancel</uui-button>
                    <uui-button look="primary" color="default" @click=${this._saveProfile}>Save</uui-button>
                    <uui-button look="primary" color="positive" @click=${this._saveAndApplyProfile}>
                        Save &amp; Apply
                    </uui-button>
                </div>
            </div>
        </div>`;
}

_renderSelectionPanel() {
    return html`
        <div class="section-label">Profile name</div>
        <input class="profile-name-input" type="text"
               .value=${this._editingProfileName}
               @input=${(e) => { this._editingProfileName = e.target.value; }}
               placeholder="Enter profile name...">
        <div class="section-label">Selection</div>
        <p style="color:var(--uui-color-text-alt,#888);font-size:13px">
            Category filters loading… (Task 13)
        </p>`;
}
```

Add stubs for Task 13–15:
```js
async _fetchAvailableItems() { /* Task 13 */ }
async _saveProfile()         { /* Task 15 */ }
async _saveAndApplyProfile() { /* Task 15 */ }
async _deleteProfile()       { /* Task 15 */ }
```

- [ ] **Step 7: Build and smoke-test**

Open Configure Export. Verify:
- Two-column dialog appears
- "+ New profile" clears profile name field
- Profile list items are clickable
- "✕" / overlay click closes the dialog
- "Cancel" button closes the dialog

- [ ] **Step 8: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/wwwroot/App_Plugins/Schema2Yaml/dashboard.js"
git commit -m "feat: add configure dialog skeleton with profile list panel"
```

---

### Task 13: Dashboard — category checkboxes + flat entity selectors

**Files:**
- Modify: `wwwroot/App_Plugins/Schema2Yaml/dashboard.js`

- [ ] **Step 1: Implement _fetchAvailableItems**

Replace the stub:
```js
async _fetchAvailableItems() {
    this._loadingItems = true;
    try {
        const res = await this._fetchWithAuth('/umbraco/api/ExportItems/Available');
        if (res.ok) this._availableItems = await res.json();
    } catch { /* silently ignore */ }
    finally { this._loadingItems = false; }
}
```

- [ ] **Step 2: Add category helpers**

```js
_toggleCategory(key, enabled) {
    this._configuring = {
        ...this._configuring,
        [key]: { ...this._configuring[key], includeAll: enabled, aliases: [], nodeIds: [] }
    };
}

_toggleEntityExpand(key) {
    const next = new Set(this._expandedCategories);
    next.has(key) ? next.delete(key) : next.add(key);
    this._expandedCategories = next;
}

_toggleAlias(key, alias, selected) {
    const cat     = this._configuring[key];
    const aliases = selected
        ? [...cat.aliases, alias]
        : cat.aliases.filter(a => a !== alias);
    this._configuring = {
        ...this._configuring,
        [key]: { ...cat, includeAll: false, aliases }
    };
}
```

- [ ] **Step 3: Replace _renderSelectionPanel**

```js
_renderSelectionPanel() {
    const flatCats = [
        { key: 'languages',       label: 'Languages',       items: this._availableItems?.languages },
        { key: 'dataTypes',       label: 'Data Types',       items: this._availableItems?.dataTypes },
        { key: 'documentTypes',   label: 'Document Types',   items: this._availableItems?.documentTypes },
        { key: 'mediaTypes',      label: 'Media Types',      items: this._availableItems?.mediaTypes },
        { key: 'templates',       label: 'Templates',        items: this._availableItems?.templates },
        { key: 'dictionaryItems', label: 'Dictionary Items', items: this._availableItems?.dictionaryItems },
        { key: 'members',         label: 'Members',          items: this._availableItems?.members },
        { key: 'users',           label: 'Users',            items: this._availableItems?.users },
    ];

    return html`
        <div class="section-label">Profile name</div>
        <input class="profile-name-input" type="text"
               .value=${this._editingProfileName}
               @input=${(e) => { this._editingProfileName = e.target.value; }}
               placeholder="Enter profile name...">
        <div class="section-label">Selection</div>
        ${this._loadingItems
            ? html`<uui-loader-circle></uui-loader-circle>`
            : flatCats.map(c => this._renderFlatCategoryRow(c))}
        ${this._renderTreeCategoryRow('content', 'Content')}
        ${this._renderTreeCategoryRow('media',   'Media')}`;
}

_renderFlatCategoryRow({ key, label, items }) {
    const cat      = this._configuring[key];
    const included = cat.includeAll || cat.aliases.length > 0;
    const expanded = this._expandedCategories.has(key);

    return html`
        <div class="cat-row">
            <input type="checkbox" .checked=${included}
                   @change=${(e) => this._toggleCategory(key, e.target.checked)}>
            <div style="flex:1">
                <span class="cat-name">${label}</span>
                ${included && cat.includeAll
                    ? html`<span class="cat-meta">(all)</span>`
                    : nothing}
                ${included && items?.length > 0 ? html`
                    <div>
                        <span class="filter-toggle"
                              @click=${() => this._toggleEntityExpand(key)}>
                            ${expanded ? '▲ hide' : '▼ filter...'}
                        </span>
                    </div>
                    ${expanded ? html`
                        <div class="entity-list">
                            ${items.map(item => {
                                const sel = cat.includeAll
                                    || cat.aliases.includes(item.alias);
                                return html`
                                    <span class="chip ${sel ? 'selected' : ''}"
                                          @click=${() => {
                                              if (cat.includeAll) {
                                                  // switch to explicit: exclude this one item
                                                  this._configuring = {
                                                      ...this._configuring,
                                                      [key]: {
                                                          includeAll: false,
                                                          aliases: items.map(i => i.alias)
                                                                        .filter(a => a !== item.alias),
                                                          nodeIds: []
                                                      }
                                                  };
                                              } else {
                                                  this._toggleAlias(key, item.alias, !cat.aliases.includes(item.alias));
                                              }
                                          }}>
                                        ${item.name}
                                    </span>`;
                            })}
                        </div>` : nothing}
                ` : nothing}
            </div>
        </div>`;
}
```

Add stub (Task 14):
```js
_renderTreeCategoryRow(key, label) {
    return html`<!-- tree picker: Task 14 -->`;
}
```

- [ ] **Step 4: Build and smoke-test**

Open Configure, verify:
- 8 flat category rows with checkboxes
- Unchecking a category removes its entity section
- "▼ filter..." appears and expands chip list
- Clicking a chip deselects it from "all" → others stay selected

- [ ] **Step 5: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/wwwroot/App_Plugins/Schema2Yaml/dashboard.js"
git commit -m "feat: add category checkboxes and flat entity selectors to configure dialog"
```

---

### Task 14: Dashboard — tree pickers (Content and Media)

**Files:**
- Modify: `wwwroot/App_Plugins/Schema2Yaml/dashboard.js`

- [ ] **Step 1: Add tree fetch helpers**

```js
async _fetchContentTree() {
    try {
        const res = await this._fetchWithAuth('/umbraco/api/ExportItems/ContentTree');
        if (res.ok) this._contentTree = await res.json();
    } catch { /* silently ignore */ }
}

async _fetchMediaTree() {
    try {
        const res = await this._fetchWithAuth('/umbraco/api/ExportItems/MediaTree');
        if (res.ok) this._mediaTree = await res.json();
    } catch { /* silently ignore */ }
}
```

- [ ] **Step 2: Add tree node helpers**

```js
_toggleTreeExpand(categoryKey, nodeId) {
    const k    = `${categoryKey}-${nodeId}`;
    const next = new Set(this._expandedTreeNodes);
    next.has(k) ? next.delete(k) : next.add(k);
    this._expandedTreeNodes = next;
}

_allDescendantIds(node) {
    return [node.id, ...(node.children ?? []).flatMap(c => this._allDescendantIds(c))];
}

_toggleNodeIds(key, node, selected) {
    const ids  = this._allDescendantIds(node);
    const cat  = this._configuring[key];
    const prev = cat.nodeIds ?? [];
    const next = selected
        ? [...new Set([...prev, ...ids])]
        : prev.filter(id => !ids.includes(id));
    this._configuring = {
        ...this._configuring,
        [key]: { ...cat, includeAll: false, nodeIds: next }
    };
}
```

- [ ] **Step 3: Replace _renderTreeCategoryRow stub**

```js
_renderTreeCategoryRow(key, label) {
    const cat      = this._configuring[key];
    const included = cat.includeAll || (cat.nodeIds?.length ?? 0) > 0;
    const expanded = this._expandedCategories.has(key);
    const tree     = key === 'content' ? this._contentTree : this._mediaTree;

    return html`
        <div class="cat-row">
            <input type="checkbox" .checked=${included}
                   @change=${async (e) => {
                       this._toggleCategory(key, e.target.checked);
                       if (e.target.checked && !tree) {
                           if (key === 'content') await this._fetchContentTree();
                           else await this._fetchMediaTree();
                       }
                   }}>
            <div style="flex:1">
                <span class="cat-name">${label}</span>
                ${included && cat.includeAll
                    ? html`<span class="cat-meta">(all)</span>`
                    : nothing}
                ${included ? html`
                    <div>
                        <span class="filter-toggle"
                              @click=${async () => {
                                  this._toggleEntityExpand(key);
                                  if (!tree) {
                                      if (key === 'content') await this._fetchContentTree();
                                      else await this._fetchMediaTree();
                                  }
                              }}>
                            ${expanded ? '▲ hide' : '▼ tree...'}
                        </span>
                    </div>
                    ${expanded && tree
                        ? html`<div style="margin-top:8px">
                            ${tree.map(n => this._renderTreeNode(key, n, 0))}
                          </div>`
                        : nothing}
                ` : nothing}
            </div>
        </div>`;
}

_renderTreeNode(key, node, depth) {
    const cat        = this._configuring[key];
    const nodeIds    = cat?.nodeIds ?? [];
    const isSelected = cat?.includeAll || nodeIds.includes(node.id);
    const expandKey  = `${key}-${node.id}`;
    const isExpanded = this._expandedTreeNodes.has(expandKey);
    const hasChildren = (node.children ?? []).length > 0;

    return html`
        <div style="padding-left:${depth * 16}px;margin:2px 0">
            <div style="display:flex;align-items:center;gap:6px">
                ${hasChildren
                    ? html`<span style="width:14px;font-size:11px;cursor:pointer;color:var(--uui-color-text-alt,#888)"
                                 @click=${() => this._toggleTreeExpand(key, node.id)}>
                               ${isExpanded ? '▼' : '▶'}
                           </span>`
                    : html`<span style="width:14px"></span>`}
                <input type="checkbox" .checked=${isSelected}
                       @change=${(e) => {
                           if (cat.includeAll) {
                               // switch from "all" to this-node-only selected
                               this._configuring = {
                                   ...this._configuring,
                                   [key]: { includeAll: false, aliases: [],
                                            nodeIds: [node.id] }
                               };
                           } else {
                               this._toggleNodeIds(key, node, e.target.checked);
                           }
                       }}>
                <span style="font-size:13px">${node.name}</span>
            </div>
            ${isExpanded && hasChildren
                ? node.children.map(c => this._renderTreeNode(key, c, depth + 1))
                : nothing}
        </div>`;
}
```

- [ ] **Step 4: Build and smoke-test**

Check Content or Media in the dialog, click "▼ tree...", verify:
- Tree loads from `/ContentTree` or `/MediaTree`
- Expand/collapse arrows work per node
- Checking a parent node selects all its descendants
- Unchecking a child deselects it

- [ ] **Step 5: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/wwwroot/App_Plugins/Schema2Yaml/dashboard.js"
git commit -m "feat: add tree pickers for Content and Media in configure dialog"
```

---

### Task 15: Dashboard — profile CRUD + save/apply/delete + full wiring

**Files:**
- Modify: `wwwroot/App_Plugins/Schema2Yaml/dashboard.js`

- [ ] **Step 1: Implement _saveProfile**

Replace the stub:

```js
async _saveProfile() {
    if (!this._editingProfileName.trim()) {
        this._notify('warning', 'Name required', 'Enter a profile name before saving.');
        return;
    }
    try {
        let res, p;
        if (this._editingProfileId === null) {
            res = await this._fetchWithAuth('/umbraco/api/ExportProfile/Create', {
                method: 'POST',
                body: JSON.stringify({ name: this._editingProfileName, selection: this._configuring }),
            });
        } else {
            res = await this._fetchWithAuth(
                `/umbraco/api/ExportProfile/Update/${this._editingProfileId}`, {
                method: 'PUT',
                body: JSON.stringify({ name: this._editingProfileName, selection: this._configuring }),
            });
        }
        if (!res.ok) throw new Error((await res.json().catch(() => ({}))).message ?? res.statusText);
        p = await res.json();
        this._editingProfileId = p.id;
        await this._loadProfiles();
        this._notify('positive', 'Profile saved', `"${p.name}" saved.`);
    } catch (e) {
        this._notify('danger', 'Save failed', e.message ?? 'Unknown error');
    }
}
```

- [ ] **Step 2: Implement _saveAndApplyProfile**

```js
async _saveAndApplyProfile() {
    await this._saveProfile();
    if (this._editingProfileId === null) return; // save failed
    try {
        const res = await this._fetchWithAuth(
            `/umbraco/api/ExportProfile/Activate/${this._editingProfileId}`,
            { method: 'POST' });
        if (!res.ok) throw new Error((await res.json().catch(() => ({}))).message ?? res.statusText);
        await this._loadActiveProfile();
        this._closeConfigDialog();
        this._notify('positive', 'Profile applied', `Exporting with "${this._editingProfileName}".`);
    } catch (e) {
        this._notify('danger', 'Apply failed', e.message ?? 'Unknown error');
    }
}
```

- [ ] **Step 3: Implement _deleteProfile**

```js
async _deleteProfile() {
    if (this._editingProfileId === null) return;
    try {
        const res = await this._fetchWithAuth(
            `/umbraco/api/ExportProfile/Delete/${this._editingProfileId}`,
            { method: 'DELETE' });
        if (!res.ok) throw new Error((await res.json().catch(() => ({}))).message ?? res.statusText);
        if (this._activeProfile?.id === this._editingProfileId)
            this._activeProfile = null;
        this._editingProfileId   = null;
        this._editingProfileName = '';
        this._configuring        = this._defaultSelection();
        await this._loadProfiles();
        this._notify('positive', 'Profile deleted', 'Profile removed.');
    } catch (e) {
        this._notify('danger', 'Delete failed', e.message ?? 'Unknown error');
    }
}
```

- [ ] **Step 4: End-to-end smoke test**

Walk the complete happy path:

1. Open Settings → Schema Export
2. Click "Configure Export" → dialog opens, profile list is empty
3. Click "+ New profile", type "Schema Only"
4. Uncheck Content, Media, Members, Users
5. Click "Save & Apply" → dialog closes, Export button shows "Export (Schema Only)"
6. Click "Export (Schema Only)" → export runs; stats show 0 for Content/Media/Members/Users
7. Click the `✕` inside the Export button → button resets to "Export to YAML"
8. Click "Export to YAML" → full export (all counts non-zero if site has content)
9. Reopen Configure, click "Schema Only", click "Delete" → profile removed from list
10. Click "Cancel" → dialog closes, no active profile

- [ ] **Step 5: Run full test suite**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml.Tests"
dotnet test -v minimal
```
Expected: All tests pass.

- [ ] **Step 6: Final build**

```bash
cd "SplatDev.Umbraco.Plugins.Schema2Yaml"
dotnet build
```
Expected: 0 errors, 0 warnings on new code.

- [ ] **Step 7: Commit**

```bash
git add "SplatDev.Umbraco.Plugins.Schema2Yaml/wwwroot/App_Plugins/Schema2Yaml/dashboard.js"
git commit -m "feat: complete cherry-pick export — profile CRUD, save/apply/delete, full end-to-end wiring"
```
