
// Type: Umbraco.Forms.Core.Persistence.Factories.WorkflowFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Factories
{
  internal sealed class WorkflowFactory : IWorkflowFactory
  {
    public List<WorkflowEntity> BuildEntities(IEnumerable<WorkflowDto> dtos) => dtos.Select<WorkflowDto, WorkflowEntity>(new Func<WorkflowDto, WorkflowEntity>(this.BuildEntity)).ToList<WorkflowEntity>();

    public List<WorkflowDto> BuildDtos(IEnumerable<WorkflowEntity> entities) => (entities != null ? entities.Select<WorkflowEntity, WorkflowDto>(new Func<WorkflowEntity, WorkflowDto>(this.BuildDto)).ToList<WorkflowDto>() : (List<WorkflowDto>) null) ?? new List<WorkflowDto>();

    public WorkflowEntity BuildEntity(WorkflowDto dto)
    {
      WorkflowEntity workflowEntity = JsonSerializer.Deserialize<WorkflowEntity>(dto.Definition, FormsJsonSerializerOptions.Default);
      if (workflowEntity == null)
        throw new InvalidOperationException("Could not deserialize entity.");
      workflowEntity.Id = dto.Id;
      workflowEntity.Key = dto.Key;
      workflowEntity.Name = dto.Name;
      workflowEntity.CreateDate = dto.CreateDate;
      workflowEntity.FormId = dto.FormId;
      return workflowEntity;
    }

    public WorkflowEntitySlim BuildEntitySlim(WorkflowDto dto)
    {
      WorkflowEntitySlim workflowEntitySlim = new WorkflowEntitySlim();
      workflowEntitySlim.Id = dto.Id;
      workflowEntitySlim.Key = dto.Key;
      workflowEntitySlim.Name = dto.Name;
      workflowEntitySlim.CreateDate = dto.CreateDate;
      workflowEntitySlim.FormId = dto.FormId;
      return workflowEntitySlim;
    }

    public WorkflowDto BuildDto(WorkflowEntity entity)
    {
      WorkflowDto workflowDto = new WorkflowDto();
      workflowDto.Id = entity.Id;
      workflowDto.Key = entity.Key;
      workflowDto.Name = entity.Name;
      workflowDto.Definition = JsonSerializer.Serialize<WorkflowEntity>(entity, FormsJsonSerializerOptions.Default);
      workflowDto.CreateDate = entity.CreateDate;
      workflowDto.UpdateDate = entity.UpdateDate;
      workflowDto.FormId = entity.FormId;
      return workflowDto;
    }
  }
}
