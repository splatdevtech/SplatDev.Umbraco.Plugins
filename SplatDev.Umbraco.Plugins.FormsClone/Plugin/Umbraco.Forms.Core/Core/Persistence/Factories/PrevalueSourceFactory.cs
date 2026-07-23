
// Type: Umbraco.Forms.Core.Persistence.Factories.PrevalueSourceFactory
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
  internal sealed class PrevalueSourceFactory : IPrevalueSourceFactory
  {
    public IEnumerable<PrevalueSourceEntity> BuildEntities(
      IEnumerable<PrevalueSourceDto> dtos)
    {
      return dtos.Select<PrevalueSourceDto, PrevalueSourceEntity>(new Func<PrevalueSourceDto, PrevalueSourceEntity>(this.BuildEntity));
    }

    public PrevalueSourceEntity BuildEntity(PrevalueSourceDto dto)
    {
      PrevalueSourceEntity prevalueSourceEntity = JsonSerializer.Deserialize<PrevalueSourceEntity>(this.EnsureDefinition(dto.Definition), FormsJsonSerializerOptions.Default);
      if (prevalueSourceEntity == null)
        throw new InvalidOperationException("Could not deserialize entity.");
      prevalueSourceEntity.CreateDate = dto.CreateDate;
      prevalueSourceEntity.UpdateDate = dto.UpdateDate;
      prevalueSourceEntity.CreatedBy = dto.CreatedBy;
      prevalueSourceEntity.UpdatedBy = dto.UpdatedBy;
      prevalueSourceEntity.Name = dto.Name;
      prevalueSourceEntity.Key = dto.Key;
      prevalueSourceEntity.Id = dto.Id;
      return prevalueSourceEntity;
    }

    private string EnsureDefinition(string definition) => definition.Replace(" fieldPreValueSourceTypeId", "fieldPreValueSourceTypeId");

    public PrevalueSourceEntitySlim BuildEntitySlim(PrevalueSourceDto dto)
    {
      PrevalueSourceEntitySlim sourceEntitySlim = new PrevalueSourceEntitySlim();
      sourceEntitySlim.CreateDate = dto.CreateDate;
      sourceEntitySlim.Name = dto.Name;
      sourceEntitySlim.Key = dto.Key;
      sourceEntitySlim.Id = dto.Id;
      return sourceEntitySlim;
    }

    public PrevalueSourceDto BuildDto(PrevalueSourceEntity entity)
    {
      PrevalueSourceDto prevalueSourceDto = new PrevalueSourceDto();
      prevalueSourceDto.CreateDate = entity.CreateDate;
      prevalueSourceDto.UpdateDate = entity.UpdateDate;
      prevalueSourceDto.CreatedBy = entity.CreatedBy;
      prevalueSourceDto.UpdatedBy = entity.UpdatedBy;
      prevalueSourceDto.Name = entity.Name;
      prevalueSourceDto.Key = entity.Key;
      prevalueSourceDto.Id = entity.Id;
      prevalueSourceDto.Definition = JsonSerializer.Serialize<PrevalueSourceEntity>(entity, FormsJsonSerializerOptions.Default);
      return prevalueSourceDto;
    }
  }
}
