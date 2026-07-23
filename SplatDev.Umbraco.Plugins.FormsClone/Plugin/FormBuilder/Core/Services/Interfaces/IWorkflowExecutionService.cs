using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Results;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IWorkflowExecutionService
    {
        Task<WorkflowExecutionResult> ExecuteWorkflowsAsync(
          Record record,
          Form form,
          FormState state,
          bool editMode);

        Task<WorkflowExecutionResult> ExecuteWorkflowsAsync(
          List<Workflow> workflows,
          Record record,
          Form form,
          FormState state);
    }
}