namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IUserGroupStartFolderStorage
    {
        IEnumerable<Guid> GetStartFolderKeys(int userGroupId);

        void UpdateStartFolders(int userGroupId, IEnumerable<Guid> folderKeys);
    }
}