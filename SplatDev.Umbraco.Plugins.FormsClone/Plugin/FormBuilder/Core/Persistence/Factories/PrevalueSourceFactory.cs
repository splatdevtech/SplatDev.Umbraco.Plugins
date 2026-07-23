using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Dtos;
using FormBuilder.Core.Persistence.Interfaces;

using System.Text.Json;

namespace FormBuilder.Core.Persistence.Factories
{
    internal sealed class PrevalueSourceFactory : IPrevalueSourceFactory
    {
        public IEnumerable<PrevalueSourceEntity> BuildEntities(
          IEnumerable<PrevalueSourceDto> dtos)
        {
            return dtos.Select(new Func<PrevalueSourceDto, PrevalueSourceEntity>(BuildEntity));
        }

        public PrevalueSourceEntity BuildEntity(PrevalueSourceDto dto)
        {
            PrevalueSourceEntity? prevalueSourceEntity = JsonSerializer.Deserialize<PrevalueSourceEntity>(EnsureDefinition(dto.Definition), FormsJsonSerializerOptions.Default) ?? throw new InvalidOperationException("Could not deserialize entity.");
            prevalueSourceEntity.CreateDate = dto.CreateDate;
            prevalueSourceEntity.UpdateDate = dto.UpdateDate;
            prevalueSourceEntity.CreatedBy = dto.CreatedBy;
            prevalueSourceEntity.UpdatedBy = dto.UpdatedBy;
            prevalueSourceEntity.Name = dto.Name;
            prevalueSourceEntity.Key = dto.Key;
            prevalueSourceEntity.Id = dto.Id;
            return prevalueSourceEntity;
        }

        private static string EnsureDefinition(string definition) => definition.Replace(" fieldPreValueSourceTypeId", "fieldPreValueSourceTypeId");

        public PrevalueSourceEntitySlim BuildEntitySlim(PrevalueSourceDto dto)
        {
            PrevalueSourceEntitySlim sourceEntitySlim = new()
            {
                CreateDate = dto.CreateDate,
                Name = dto.Name,
                Key = dto.Key,
                Id = dto.Id
            };
            return sourceEntitySlim;
        }

        public PrevalueSourceDto BuildDto(PrevalueSourceEntity entity)
        {
            PrevalueSourceDto prevalueSourceDto = new()
            {
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                Name = entity.Name,
                Key = entity.Key,
                Id = entity.Id,
                Definition = JsonSerializer.Serialize(entity, FormsJsonSerializerOptions.Default)
            };
            return prevalueSourceDto;
        }
    }
}