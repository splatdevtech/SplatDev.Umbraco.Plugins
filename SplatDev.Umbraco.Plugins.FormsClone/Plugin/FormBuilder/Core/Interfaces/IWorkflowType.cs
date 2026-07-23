using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Workflows;

namespace FormBuilder.Core.Interfaces
{
    public interface IWorkflowType
    {
        string Name { get; }

        string Alias { get; set; }

        string Description { get; }

        Guid Id { get; }

        Workflow? Workflow { get; }

        Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context);

        Dictionary<string, SettingAttribute> Settings();

        List<Exception> ValidateSettings();
    }
}