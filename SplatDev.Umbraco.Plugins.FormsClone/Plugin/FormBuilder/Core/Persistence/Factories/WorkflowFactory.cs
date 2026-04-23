using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Dtos;

using System.Text.Json;

namespace FormBuilder.Core.Persistence.Factories
{
    internal sealed class WorkflowFactory : IWorkflowFactory
    {
        public List<WorkflowEntity> BuildEntities(IEnumerable<WorkflowDto> dtos) => [.. dtos.Select(new Func<WorkflowDto, WorkflowEntity>(BuildEntity))];

        public List<WorkflowDto> BuildDtos(IEnumerable<WorkflowEntity> entities) => (entities is not null ? [.. entities.Select(new Func<WorkflowEntity, WorkflowDto>(BuildDto))] : null as List<WorkflowDto>) ?? [];

        public WorkflowEntity BuildEntity(WorkflowDto dto)
        {
            WorkflowEntity? workflowEntity = JsonSerializer.Deserialize<WorkflowEntity>(dto.Definition, FormsJsonSerializerOptions.Default) ?? throw new InvalidOperationException("Could not deserialize entity.");
            workflowEntity.Id = dto.Id;
            workflowEntity.Key = dto.Key;
            workflowEntity.Name = dto.Name;
            workflowEntity.CreateDate = dto.CreateDate;
            workflowEntity.FormId = dto.FormId;
            return workflowEntity;
        }

        public WorkflowEntitySlim BuildEntitySlim(WorkflowDto dto)
        {
            WorkflowEntitySlim workflowEntitySlim = new()
            {
                Id = dto.Id,
                Key = dto.Key,
                Name = dto.Name,
                CreateDate = dto.CreateDate,
                FormId = dto.FormId
            };
            return workflowEntitySlim;
        }

        public WorkflowDto BuildDto(WorkflowEntity entity)
        {
            WorkflowDto workflowDto = new()
            {
                Id = entity.Id,
                Key = entity.Key,
                Name = entity.Name,
                Definition = JsonSerializer.Serialize(entity, FormsJsonSerializerOptions.Default),
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                FormId = entity.FormId
            };
            return workflowDto;
        }
    }
}