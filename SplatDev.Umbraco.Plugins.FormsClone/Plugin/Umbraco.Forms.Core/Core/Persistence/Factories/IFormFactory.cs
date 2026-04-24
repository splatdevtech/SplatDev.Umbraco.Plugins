
// Type: Umbraco.Forms.Core.Persistence.Factories.IFormFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Factories
{
  public interface IFormFactory
  {
    FormDto BuildDto(FormEntity entity);

    NodeDto BuildNodeDto(FormEntity entity);

    NodeDto BuildNodeDto(FormEntitySlim entity)
    {
      FormEntity entity1 = new FormEntity();
      entity1.Id = entity.Id;
      entity1.Key = entity.Key;
      entity1.Name = entity.Name;
      entity1.FolderId = entity.FolderId;
      entity1.NodeId = entity.NodeId;
      entity1.CreateDate = entity.CreateDate;
      return this.BuildNodeDto(entity1);
    }

    IEnumerable<FormEntity> BuildEntities(IEnumerable<FormDto> dtos);

    FormEntity BuildEntity(FormDto dto);

    FormEntitySlim BuildEntitySlim(FormDto dto);
  }
}
