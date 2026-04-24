namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IUserStartFolderStorage
    {
        IEnumerable<Guid> GetStartFolderKeys(int userId);

        void UpdateStartFolders(int userId, IEnumerable<Guid> folderKeys);
    }
}