using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;

namespace FormBuilder.Core.Interfaces
{
    public interface IWorkflowFactory
    {
        WorkflowDto BuildDto(WorkflowEntity entity);

        WorkflowEntity BuildEntity(WorkflowDto dto);

        WorkflowEntitySlim BuildEntitySlim(WorkflowDto dto);

        List<WorkflowDto> BuildDtos(IEnumerable<WorkflowEntity> entities);

        List<WorkflowEntity> BuildEntities(IEnumerable<WorkflowDto> dtos);
    }
}