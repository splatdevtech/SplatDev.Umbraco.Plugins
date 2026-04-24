using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IWorkflowService : IBaseService<Workflow, WorkflowEntity>
    {
        Workflow? Insert(Form form, Workflow workflow);

        IEnumerable<Workflow> Insert(Form form, IEnumerable<Workflow> workflow);

        List<Workflow> Get(Form form);

        IEnumerable<WorkflowSlim> GetSlim(Guid formId);

        IEnumerable<WorkflowSlim> GetSlim(Form form);

        List<Workflow> GetActiveWorkFlows(Form form, FormState state);

        void Delete(Form form);
    }
}