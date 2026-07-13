using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SplatDev.Umbraco.Workflow.Api.Contracts;
using SplatDev.Umbraco.Workflow.Core.Contracts;
#if NET8_0
using UmbracoAuthorizedApiController = Umbraco.Cms.Web.BackOffice.Controllers.UmbracoAuthorizedApiController;
#else
using UmbracoAuthorizedApiController = Umbraco.Cms.Web.Common.Controllers.UmbracoApiController;
#endif

namespace SplatDev.Umbraco.Workflow.Api.Controllers;

[Route("umbraco/api/Workflow")]
public class WorkflowController(
    IWorkflowEngine engine,
    IWorkflowDefinitionStore definitionStore,
    IWorkflowDataProvider dataProvider) : UmbracoAuthorizedApiController
{
    private readonly IWorkflowEngine _engine = engine;
    private readonly IWorkflowDefinitionStore _definitionStore = definitionStore;
    private readonly IWorkflowDataProvider _dataProvider = dataProvider;

    [HttpGet("definitions")]
    public async Task<IActionResult> ListDefinitions(CancellationToken ct)
    {
        var definitions = await _definitionStore.ListActiveAsync(ct);
        return Ok(definitions.Select(d => new
        {
            d.Key, d.Label, d.Version, d.IsActive, d.CreatedAt, d.CreatedBy,
            StepCount = d.Workflow.Steps.Count,
        }));
    }

    [HttpGet("definitions/{key}")]
    public async Task<IActionResult> GetDefinition(string key, CancellationToken ct)
    {
        var def = await _definitionStore.GetActiveAsync(key, ct);
        return def is null ? NotFound() : Ok(new { def.Key, def.Label, def.Version, Steps = def.Workflow.Steps });
    }

    [HttpPost("definitions")]
    public async Task<IActionResult> CreateDefinition(
        [FromBody] CreateDefinitionRequest request,
        CancellationToken ct)
    {
        var validator = new CreateDefinitionValidator();
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var existing = await _definitionStore.GetActiveAsync(request.Key, ct);
        var version = (existing?.Version ?? 0) + 1;

        var definition = new Core.Models.WorkflowDefinition
        {
            Key = request.Key,
            Label = request.Label,
            Version = version,
            Workflow = new Core.Models.Workflow
            {
                Key = request.Key,
                Label = request.Label,
                Steps = request.Steps.Select(s => new Core.Models.WorkflowStep
                {
                    Key = s.Key,
                    Label = s.Label,
                    Department = s.Department,
                    Group = s.Group,
                    Actions = s.Actions.Select(a => (IWorkflowAction)new Core.Models.WorkflowAction
                    {
                        Key = a.Key,
                        Label = a.Label,
                        NextStepKey = a.NextStepKey,
                        Assignment = a.Assignment,
                    }).ToList(),
                    PreActionMessages = s.PreActions.Select(m => (IActionMessage)new Core.Models.ActionMessage
                    {
                        Alias = m.Alias,
                        Label = m.Label,
                        Audience = m.Audience,
                    }).ToList(),
                    PostActionMessages = s.PostActions.Select(m => (IActionMessage)new Core.Models.ActionMessage
                    {
                        Alias = m.Alias,
                        Label = m.Label,
                        Audience = m.Audience,
                    }).ToList(),
                }).ToList<IWorkflowStep>(),
            },
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        await _definitionStore.SaveAsync(definition, GetUsername(), ct);
        return Ok(new { definition.Key, definition.Version });
    }

    [HttpGet("instances")]
    public async Task<IActionResult> ListInstances(
        [FromQuery] string? workflowKey,
        [FromQuery] string? status,
        [FromQuery] bool assignedToMe,
        [FromQuery] string? group,
        [FromQuery] string? department,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var filter = new WorkflowQueryFilter(
            workflowKey,
            string.IsNullOrEmpty(status) ? null : Enum.Parse<WorkflowStatus>(status, ignoreCase: true),
            assignedToMe,
            group,
            department,
            search,
            page,
            pageSize);

        var rows = _dataProvider.GetDisplayRows(filter);
        return Ok(new { rows, page, pageSize });
    }

    [HttpPost("instances")]
    public async Task<IActionResult> CreateInstance(
        [FromBody] CreateInstanceRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.WorkflowKey))
            return BadRequest("workflowKey is required.");

        var instance = await _engine.CreateAsync(
            request.WorkflowKey, request.MetadataJson, GetUsername(), ct);

        return Ok(new { instance.Id, instance.WorkflowKey, instance.CurrentStepKey, instance.Status, instance.CreatedAt });
    }

    [HttpGet("instances/{id:long}")]
    public async Task<IActionResult> GetInstance(long id, CancellationToken ct)
    {
        // Instance lookup via the persistence layer
        var instanceStore = HttpContext.RequestServices.GetRequiredService<IWorkflowInstanceStore>();
        var instance = await instanceStore.GetByIdAsync(id, ct);
        if (instance is null) return NotFound();

        var def = await _definitionStore.GetActiveAsync(instance.WorkflowKey, ct);
        return Ok(new { instance, DefinitionSteps = def?.Workflow.Steps });
    }

    [HttpPost("instances/{id:long}/transition")]
    public async Task<IActionResult> Transition(
        long id,
        [FromBody] TransitionRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.ActionKey))
            return BadRequest("actionKey is required.");

        var result = await _engine.TransitionAsync(id, request.ActionKey, GetUsername(), ct);
        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    private string GetUsername() => User.Identity?.Name ?? "system";
}
