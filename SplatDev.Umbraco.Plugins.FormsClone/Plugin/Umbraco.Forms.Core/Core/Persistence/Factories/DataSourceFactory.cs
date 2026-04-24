
// Type: Umbraco.Forms.Core.Persistence.Factories.DataSourceFactory
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
  internal sealed class DataSourceFactory : IDataSourceFactory
  {
    public IEnumerable<DataSourceEntity> BuildEntities(
      IEnumerable<DataSourceDto> dtos)
    {
      return dtos.Select<DataSourceDto, DataSourceEntity>(new Func<DataSourceDto, DataSourceEntity>(this.BuildEntity));
    }

    public DataSourceEntity BuildEntity(DataSourceDto dto)
    {
      DataSourceEntity dataSourceEntity = JsonSerializer.Deserialize<DataSourceEntity>(dto.Definition, FormsJsonSerializerOptions.Default);
      if (dataSourceEntity == null)
        throw new InvalidOperationException("Could not deserialize entity.");
      dataSourceEntity.Name = dto.Name;
      dataSourceEntity.Id = dto.Id;
      dataSourceEntity.Key = dto.Key;
      dataSourceEntity.CreateDate = dto.CreateDate;
      dataSourceEntity.UpdateDate = dto.UpdateDate;
      dataSourceEntity.CreatedBy = dto.CreatedBy;
      dataSourceEntity.UpdatedBy = dto.UpdatedBy;
      return dataSourceEntity;
    }

    public DataSourceEntitySlim BuildEntitySlim(DataSourceDto dto)
    {
      DataSourceEntitySlim sourceEntitySlim = new DataSourceEntitySlim();
      sourceEntitySlim.CreateDate = dto.CreateDate;
      sourceEntitySlim.Name = dto.Name;
      sourceEntitySlim.Key = dto.Key;
      sourceEntitySlim.Id = dto.Id;
      return sourceEntitySlim;
    }

    public DataSourceDto BuildDto(DataSourceEntity entity)
    {
      DataSourceDto dataSourceDto = new DataSourceDto();
      dataSourceDto.Name = entity.Name;
      dataSourceDto.Id = entity.Id;
      dataSourceDto.Key = entity.Key;
      dataSourceDto.CreateDate = entity.CreateDate;
      dataSourceDto.UpdateDate = entity.UpdateDate;
      dataSourceDto.CreatedBy = entity.CreatedBy;
      dataSourceDto.UpdatedBy = entity.UpdatedBy;
      dataSourceDto.Definition = JsonSerializer.Serialize<DataSourceEntity>(entity, FormsJsonSerializerOptions.Default);
      return dataSourceDto;
    }
  }
}
