
// Type: Umbraco.Forms.Core.Persistence.Factories.FormFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Factories
{
  internal sealed class FormFactory : IFormFactory
  {
    public IEnumerable<FormEntity> BuildEntities(IEnumerable<FormDto> dtos) => dtos.Select<FormDto, FormEntity>(new Func<FormDto, FormEntity>(this.BuildEntity));

    public FormEntity BuildEntity(FormDto dto)
    {
      FormEntity entity = JsonSerializer.Deserialize<FormEntity>(dto.Definition, FormsJsonSerializerOptions.Default);
      if (entity == null)
        throw new InvalidOperationException("Could not deserialize entity.");
      FormFactory.EnsurePrevalues(entity);
      entity.CreateDate = dto.CreateDate;
      entity.UpdateDate = dto.UpdateDate;
      entity.CreatedBy = dto.CreatedBy;
      entity.UpdatedBy = dto.UpdatedBy;
      entity.Name = dto.Name;
      entity.Key = dto.Key;
      entity.Id = dto.Id;
      entity.FolderId = dto.FolderKey;
      entity.NodeId = dto.NodeId;
      return entity;
    }

    private static void EnsurePrevalues(FormEntity entity)
    {
      foreach (Field field in entity.AllFields().Where<Field>((Func<Field, bool>) (x => x.PreValues == null)))
        field.PreValues = (IEnumerable<FieldPrevalue>) new List<FieldPrevalue>();
    }

    public FormEntitySlim BuildEntitySlim(FormDto dto)
    {
      FormEntitySlim formEntitySlim = new FormEntitySlim();
      formEntitySlim.CreateDate = dto.CreateDate;
      formEntitySlim.Name = dto.Name;
      formEntitySlim.Key = dto.Key;
      formEntitySlim.Id = dto.Id;
      formEntitySlim.FolderId = dto.FolderKey;
      formEntitySlim.NodeId = dto.NodeId;
      return formEntitySlim;
    }

    public FormDto BuildDto(FormEntity entity)
    {
      FormDto formDto = new FormDto();
      formDto.CreateDate = entity.CreateDate;
      formDto.UpdateDate = entity.UpdateDate;
      formDto.CreatedBy = entity.CreatedBy;
      formDto.UpdatedBy = entity.UpdatedBy;
      formDto.Name = entity.Name;
      formDto.Key = entity.Key;
      formDto.Id = entity.Id;
      formDto.FolderKey = entity.FolderId;
      formDto.NodeId = entity.NodeId;
      formDto.Definition = JsonSerializer.Serialize<FormEntity>(entity, FormsJsonSerializerOptions.Default);
      return formDto;
    }

    public NodeDto BuildNodeDto(FormEntity entity) => new NodeDto()
    {
      UniqueId = entity.Key,
      ParentId = -1,
      Level = 0,
      Path = "-1",
      SortOrder = 0,
      Trashed = false,
      Text = entity.Name,
      UserId = new int?(-1),
      NodeObjectType = new Guid?(Umbraco.Cms.Core.Constants.ObjectTypes.FormsForm),
      CreateDate = DateTime.Now
    };

    public NodeDto BuildNodeDto(FormEntitySlim entity) => new NodeDto()
    {
      UniqueId = entity.Key,
      ParentId = -1,
      Level = 0,
      Path = "-1",
      SortOrder = 0,
      Trashed = false,
      Text = entity.Name,
      UserId = new int?(-1),
      NodeObjectType = new Guid?(Umbraco.Cms.Core.Constants.ObjectTypes.FormsForm),
      CreateDate = DateTime.Now
    };
  }
}
