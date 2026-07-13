using System.Text.Json;
using SplatDev.Umbraco.Workflow.Core.Contracts;

namespace SplatDev.Umbraco.Workflow.Core.Providers;

public class JsonMetadataDataProvider : IWorkflowDataProvider
{
    private readonly IWorkflowInstanceStore _instanceStore;
    private readonly IWorkflowDefinitionStore _definitionStore;

    public JsonMetadataDataProvider(
        IWorkflowInstanceStore instanceStore,
        IWorkflowDefinitionStore definitionStore)
    {
        _instanceStore = instanceStore;
        _definitionStore = definitionStore;
    }

    public IReadOnlyList<WorkflowDisplayRow> GetDisplayRows(WorkflowQueryFilter filter)
    {
        // Host implements this against their data source.
        // The default JSON-metadata provider iterates instances
        // and parses MetadataJson for display columns.
        return [];
    }

    public string? GetSearchableValue(long instanceId, string fieldKey)
    {
        var instance = _instanceStore.GetByIdAsync(instanceId, CancellationToken.None).Result;
        if (instance?.MetadataJson is null) return null;

        try
        {
            using var doc = JsonDocument.Parse(instance.MetadataJson);
            return doc.RootElement.TryGetProperty(fieldKey, out var prop)
                ? prop.GetString()
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public IReadOnlyList<DisplayColumn> GetColumns(string workflowKey)
    {
        // Default columns — host overrides via DI registration.
        return
        [
            new DisplayColumn("instanceId", "ID", 1),
            new DisplayColumn("currentStep", "Current Step", 2),
            new DisplayColumn("status", "Status", 3),
            new DisplayColumn("createdAt", "Created", 4),
        ];
    }
}
