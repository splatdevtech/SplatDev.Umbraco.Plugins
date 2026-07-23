using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;

using System.Data;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Services
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class FolderService(
      IFolderRepository folderRepository,
      IUmbracoMapper mapper,
      IScopeProvider scopeProvider,
      IEventMessagesFactory eventMessagesFactory,
      IAppPolicyCache appCache) :
      BaseService<IFolderRepository, Folder, FolderEntity>(mapper, scopeProvider, appCache, eventMessagesFactory, folderRepository, "Forms.Folder."),
      IFolderService,
      IBaseService<Folder, FolderEntity>
    {
        protected override CreatingNotification<Folder> GetCreatingNotification(
          Folder item,
          EventMessages eventMessages)
        {
            return new FolderCreatingNotification(item, eventMessages);
        }

        protected override CreatedNotification<Folder> GetCreatedNotification(
          Folder item,
          EventMessages eventMessages)
        {
            return new FolderCreatedNotification(item, eventMessages);
        }

        protected override SavingNotification<Folder> GetSavingNotification(
          Folder item,
          EventMessages eventMessages,
          Dictionary<string, object?> additionalData)
        {
            return new FolderSavingNotification(item, eventMessages, additionalData);
        }

        protected override SavedNotification<Folder> GetSavedNotification(
          Folder item,
          EventMessages eventMessages)
        {
            return new FolderSavedNotification(item, eventMessages);
        }

        protected override DeletingNotification<Folder> GetDeletingNotification(
          Folder item,
          EventMessages eventMessages)
        {
            return new FolderDeletingNotification(item, eventMessages);
        }

        protected override DeletedNotification<Folder> GetDeletedNotification(
          Folder item,
          EventMessages eventMessages)
        {
            return new FolderDeletedNotification(item, eventMessages);
        }

        public IEnumerable<Folder> GetAtRoot()
        {
            using IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, false);
            IEnumerable<FolderEntity> atRoot = Repository.GetAtRoot();
            ((ICoreScope)scope).Complete();
            return Mapper.MapEnumerable<FolderEntity, Folder>(atRoot);
        }

        public IEnumerable<Folder> GetChildren(Guid parentId)
        {
            using IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, false);
            IEnumerable<FolderEntity> children = Repository.GetChildren(parentId);
            ((ICoreScope)scope).Complete();
            return Mapper.MapEnumerable<FolderEntity, Folder>(children);
        }

        public bool ExistsAndIsEmpty(Guid id)
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return Repository.ExistsAndIsEmpty(id);
        }

        public string GetPath(Guid id, string prefixIds = "")
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
            {
                FolderEntity? folder = Repository.Get(id);
                if (folder is null)
                    return string.Empty;
                List<string> stringList = [];
                AddPathPart(stringList, folder, prefixIds);
                while (true)
                {
                    Guid? parentKey = folder.ParentKey;
                    if (parentKey.HasValue)
                    {
                        IFolderRepository repository = Repository;
                        parentKey = folder.ParentKey;
                        Guid id1 = parentKey!.Value;
                        folder = repository.Get(id1);
                        if (folder is not null)
                            AddPathPart(stringList, folder, prefixIds);
                        else
                            break;
                    }
                    else
                        break;
                }
                stringList.Add("-1");
                stringList.Reverse();
                return string.Join(",", stringList);
            }
        }

        private static void AddPathPart(List<string> pathParts, FolderEntity folder, string prefixIds) => pathParts.Add(prefixIds + folder.Key.ToString());
    }

#pragma warning restore CS0618 // Type or member is obsolete
}