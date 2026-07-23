using FormBuilder.Core.Models;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IFolderService : IBaseService<Folder, FolderEntity>
    {
        IEnumerable<Folder> GetAtRoot();

        IEnumerable<Folder> GetChildren(Guid parentId);

        bool ExistsAndIsEmpty(Guid id);

        string GetPath(Guid id, string prefixIds = "");
    }
}