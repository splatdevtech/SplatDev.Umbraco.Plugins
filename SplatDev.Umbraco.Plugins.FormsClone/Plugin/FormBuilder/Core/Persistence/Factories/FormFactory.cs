using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Dtos;
using FormBuilder.Core.Persistence.Interfaces;

using System.Text.Json;

using Umbraco.Cms.Infrastructure.Persistence.Dtos;

namespace FormBuilder.Core.Persistence.Factories
{
    internal sealed class FormFactory : IFormFactory
    {
        public IEnumerable<FormEntity> BuildEntities(IEnumerable<FormDto> dtos) => dtos.Select(new Func<FormDto, FormEntity>(BuildEntity));

        public FormEntity BuildEntity(FormDto dto)
        {
            FormEntity? entity = JsonSerializer.Deserialize<FormEntity>(dto.Definition, FormsJsonSerializerOptions.Default) ?? throw new InvalidOperationException("Could not deserialize entity.");
            EnsurePrevalues(entity);
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
            foreach (Field field in entity.AllFields().Where(x => x.PreValues is null))
                field.PreValues = [];
        }

        public FormEntitySlim BuildEntitySlim(FormDto dto)
        {
            FormEntitySlim formEntitySlim = new()
            {
                CreateDate = dto.CreateDate,
                Name = dto.Name,
                Key = dto.Key,
                Id = dto.Id,
                FolderId = dto.FolderKey,
                NodeId = dto.NodeId
            };
            return formEntitySlim;
        }

        public FormDto BuildDto(FormEntity entity)
        {
            FormDto formDto = new()
            {
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                Name = entity.Name,
                Key = entity.Key,
                Id = entity.Id,
                FolderKey = entity.FolderId,
                NodeId = entity.NodeId,
                Definition = JsonSerializer.Serialize(entity, FormsJsonSerializerOptions.Default)
            };
            return formDto;
        }

        public NodeDto BuildNodeDto(FormEntity entity) => new()
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