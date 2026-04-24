using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Dtos;
using FormBuilder.Core.Persistence.Interfaces;

using System.Text.Json;

namespace FormBuilder.Core.Persistence.Factories
{
    internal sealed class DataSourceFactory : IDataSourceFactory
    {
        public IEnumerable<DataSourceEntity> BuildEntities(
          IEnumerable<DataSourceDto> dtos)
        {
            return dtos.Select(new Func<DataSourceDto, DataSourceEntity>(BuildEntity));
        }

        public DataSourceEntity BuildEntity(DataSourceDto dto)
        {
            DataSourceEntity? dataSourceEntity = JsonSerializer.Deserialize<DataSourceEntity>(dto.Definition, FormsJsonSerializerOptions.Default) ?? throw new InvalidOperationException("Could not deserialize entity.");
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
            DataSourceEntitySlim sourceEntitySlim = new()
            {
                CreateDate = dto.CreateDate,
                Name = dto.Name,
                Key = dto.Key,
                Id = dto.Id
            };
            return sourceEntitySlim;
        }

        public DataSourceDto BuildDto(DataSourceEntity entity)
        {
            DataSourceDto dataSourceDto = new()
            {
                Name = entity.Name,
                Id = entity.Id,
                Key = entity.Key,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                Definition = JsonSerializer.Serialize(entity, FormsJsonSerializerOptions.Default)
            };
            return dataSourceDto;
        }
    }
}