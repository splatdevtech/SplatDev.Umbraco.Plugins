using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;

namespace FormBuilder.Core.Persistence.Interfaces
{
    public interface IFolderFactory
    {
        FolderDto BuildDto(FolderEntity entity);

        IEnumerable<FolderEntity> BuildEntities(IEnumerable<FolderDto> dtos);

        FolderEntity BuildEntity(FolderDto dto);
    }
}