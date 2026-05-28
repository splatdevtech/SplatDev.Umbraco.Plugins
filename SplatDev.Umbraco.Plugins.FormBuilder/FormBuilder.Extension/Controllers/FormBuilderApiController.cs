using Microsoft.AspNetCore.Mvc;
using FormBuilder.Extension.Entities;
using FormBuilder.Extension.Interfaces;

namespace FormBuilder.Extension.Controllers;

[Route("umbraco/api/formbuilder/[action]")]
public class FormBuilderApiController(IFormRepository formRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllForms()
    {
        var forms = await formRepository.GetAllAsync();
        return Ok(forms.Select(MapSummary));
    }

    [HttpGet]
    public async Task<IActionResult> GetForm([FromQuery] int id)
    {
        if (id <= 0) return BadRequest("Form id is required.");
        var form = await formRepository.GetByIdAsync(id);
        return form is null ? NotFound() : Ok(MapDetail(form));
    }

    [HttpPost]
    public async Task<IActionResult> CreateForm([FromBody] CreateFormRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");

        var form = new Form
        {
            Name = request.Name,
            Category = request.Category ?? string.Empty,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Fields = request.Fields?.Select(MapField).ToList() ?? [],
        };

        var created = await formRepository.CreateAsync(form);
        return Ok(MapDetail(created));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateForm([FromQuery] int id, [FromBody] CreateFormRequest request)
    {
        if (id <= 0) return BadRequest("Form id is required.");
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");

        var existing = await formRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Name = request.Name;
        existing.Category = request.Category ?? string.Empty;
        existing.UpdatedDate = DateTime.UtcNow;
        existing.Fields = request.Fields?.Select(MapField).ToList() ?? [];
        existing.Workflows = [];

        var updated = await formRepository.UpdateAsync(existing);
        return Ok(MapDetail(updated));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteForm([FromQuery] int id)
    {
        if (id <= 0) return BadRequest("Form id is required.");

        try
        {
            await formRepository.DeleteAsync(id);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DuplicateForm([FromQuery] int id)
    {
        if (id <= 0) return BadRequest("Form id is required.");
        var form = await formRepository.GetByIdAsync(id);
        if (form is null) return NotFound();

        form.Id = 0;
        form.Name = $"{form.Name} (copy)";
        foreach (var f in form.Fields) f.Id = 0;
        foreach (var w in form.Workflows) w.Id = 0;

        var created = await formRepository.CreateAsync(form);
        return Ok(MapDetail(created));
    }

    [HttpGet]
    public IActionResult GetFieldTypes() => Ok(new[]
    {
        new { Type = "text", Label = "Text", Icon = "icon-text" },
        new { Type = "textarea", Label = "Textarea", Icon = "icon-textarea" },
        new { Type = "dropdown", Label = "Dropdown", Icon = "icon-dropdown" },
        new { Type = "checkbox", Label = "Checkbox", Icon = "icon-checkbox" },
        new { Type = "radio", Label = "Radio", Icon = "icon-radio" },
        new { Type = "date", Label = "Date", Icon = "icon-calendar" },
        new { Type = "file", Label = "File Upload", Icon = "icon-file" },
        new { Type = "recaptcha", Label = "reCAPTCHA", Icon = "icon-shield" },
        new { Type = "email", Label = "Email", Icon = "icon-mail" },
        new { Type = "hidden", Label = "Hidden", Icon = "icon-hidden" },
        new { Type = "number", Label = "Number", Icon = "icon-number" },
        new { Type = "password", Label = "Password", Icon = "icon-password" },
    });

    private static object MapSummary(Form f) => new
    {
        f.Id,
        f.Name,
        f.Category,
        f.CreatedDate,
        FieldCount = f.Fields.Count,
    };

    private static object MapDetail(Form f) => new
    {
        f.Id,
        f.Name,
        f.Category,
        f.CreatedDate,
        f.UpdatedDate,
        Fields = f.Fields.Select(MapFieldDto),
        WorkflowCount = f.Workflows.Count,
    };

    private static FormField MapField(CreateFormFieldRequest req) => new()
    {
        Alias = req.Alias,
        Label = req.Label,
        Type = req.Type,
        IsRequired = req.Required,
        Placeholder = req.Placeholder,
        Regex = req.Regex,
        MinLength = req.MinLength ?? 0,
        SortOrder = req.SortOrder,
    };

    private static object MapFieldDto(FormField f) => new
    {
        f.Id,
        f.Alias,
        f.Label,
        f.Type,
        f.IsRequired,
        f.Placeholder,
        f.Regex,
        f.MinLength,
        f.SortOrder,
        DropdownValues = f.DropdownValues.Select(d => new { d.Value }),
    };
}

public record CreateFormRequest(string Name, string? Category, List<CreateFormFieldRequest>? Fields);
public record CreateFormFieldRequest(string Alias, string? Label, string? Type,
    bool Required, string? Placeholder, string? Regex, int? MinLength, int SortOrder);
