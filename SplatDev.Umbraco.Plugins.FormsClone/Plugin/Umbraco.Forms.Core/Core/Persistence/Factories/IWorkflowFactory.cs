
// Type: Umbraco.Forms.Core.Persistence.Factories.IWorkflowFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Factories
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
