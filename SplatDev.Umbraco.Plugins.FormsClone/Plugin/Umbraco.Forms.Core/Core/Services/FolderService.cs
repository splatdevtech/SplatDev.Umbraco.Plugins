
// Type: Umbraco.Forms.Core.Services.FolderService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Data;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Repositories;
using Umbraco.Forms.Core.Services.Notifications;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Services
{
    public sealed class FolderService :
      BaseService<IFolderRepository, Folder, FolderEntity>,
      IFolderService,
      IBaseService<Folder, FolderEntity>
    {
        public FolderService(
          IFolderRepository folderRepository,
          IUmbracoMapper mapper,
          IScopeProvider scopeProvider,
          IEventMessagesFactory eventMessagesFactory,
          IAppPolicyCache appCache)
          : base(mapper, scopeProvider, appCache, eventMessagesFactory, folderRepository, "Forms.Folder.")
        {
        }

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
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, false))
            {
                IEnumerable<FolderEntity> atRoot = this.Repository.GetAtRoot();
                scope.Complete();
                return this.Mapper.MapEnumerable<FolderEntity, Folder>(atRoot);
            }
        }

        public IEnumerable<Folder> GetChildren(Guid parentId)
        {
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, false))
            {
                IEnumerable<FolderEntity> children = this.Repository.GetChildren(parentId);
                scope.Complete();
                return this.Mapper.MapEnumerable<FolderEntity, Folder>(children);
            }
        }

        public bool ExistsAndIsEmpty(Guid id)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Repository.ExistsAndIsEmpty(id);
        }

        public string GetPath(Guid id, string prefixIds = "")
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
            {
                FolderEntity folder = this.Repository.Get(id);
                if (folder == null)
                    return string.Empty;
                List<string> stringList = new List<string>();
                FolderService.AddPathPart(stringList, folder, prefixIds);
                while (true)
                {
                    Guid? parentKey = folder.ParentKey;
                    if (parentKey.HasValue)
                    {
                        IFolderRepository repository = this.Repository;
                        parentKey = folder.ParentKey;
                        Guid id1 = parentKey.Value;
                        folder = repository.Get(id1);
                        if (folder != null)
                            FolderService.AddPathPart(stringList, folder, prefixIds);
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
}
