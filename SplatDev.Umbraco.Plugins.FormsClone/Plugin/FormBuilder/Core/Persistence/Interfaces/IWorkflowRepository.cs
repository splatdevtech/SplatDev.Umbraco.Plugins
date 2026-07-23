using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Persistence;

namespace FormBuilder.Core.Persistence.Interfaces
{
    public interface IWorkflowRepository :
      IReadWriteQueryRepository<Guid, WorkflowEntity>,
      IReadRepository<Guid, WorkflowEntity>,
      IRepository,
      IWriteRepository<WorkflowEntity>,
      IQueryRepository<WorkflowEntity>
    {
        IEnumerable<WorkflowEntity> GetFor(Form form);

        IEnumerable<WorkflowEntitySlim> GetForSlim(Guid formId);

        IEnumerable<WorkflowEntitySlim> GetForSlim(Form form);
    }
}