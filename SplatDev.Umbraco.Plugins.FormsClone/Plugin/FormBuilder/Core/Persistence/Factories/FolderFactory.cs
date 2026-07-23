using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;
using FormBuilder.Core.Persistence.Interfaces;

namespace FormBuilder.Core.Persistence.Factories
{
    internal sealed class FolderFactory : IFolderFactory
    {
        public IEnumerable<FolderEntity> BuildEntities(
          IEnumerable<FolderDto> dtos)
        {
            return dtos.Select(new Func<FolderDto, FolderEntity>(BuildEntity));
        }

        public FolderEntity BuildEntity(FolderDto dto)
        {
            FolderEntity folderEntity = new()
            {
                Name = dto.Name,
                Id = dto.Id,
                Key = dto.Key,
                CreateDate = dto.CreateDate,
                UpdateDate = dto.UpdateDate,
                ParentKey = dto.ParentKey
            };
            return folderEntity;
        }

        public FolderDto BuildDto(FolderEntity entity)
        {
            FolderDto folderDto = new()
            {
                Name = entity.Name,
                Id = entity.Id,
                Key = entity.Key,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                ParentKey = entity.ParentKey,
                Definition = string.Empty
            };
            return folderDto;
        }
    }
}