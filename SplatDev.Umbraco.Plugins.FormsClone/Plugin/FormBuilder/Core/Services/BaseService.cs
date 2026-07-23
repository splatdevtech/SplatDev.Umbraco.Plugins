using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using System.Data;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Core.Services
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal abstract class BaseService<TRepository, TType, TEntity>(
      IUmbracoMapper mapper,
      IScopeProvider scopeProvider,
      IAppPolicyCache appCache,
      IEventMessagesFactory eventMessagesFactory,
      TRepository repository,
      string cacheKeys) : IBaseService<TType, TEntity>
      where TRepository : IReadWriteQueryRepository<Guid, TEntity>
      where TType : class, IType
      where TEntity : IEntity
    {
        private readonly string _cacheKeys = cacheKeys;

        protected TRepository Repository { get; } = repository;

        protected IUmbracoMapper Mapper { get; } = mapper;

        protected IScopeProvider ScopeProvider { get; } = scopeProvider;

        protected IAppPolicyCache AppCache { get; } = appCache;

        protected IEventMessagesFactory EventMessagesFactory { get; } = eventMessagesFactory;

        protected abstract CreatingNotification<TType> GetCreatingNotification(
          TType item,
          EventMessages eventMessages);

        protected abstract CreatedNotification<TType> GetCreatedNotification(
          TType item,
          EventMessages eventMessages);

        protected abstract SavingNotification<TType> GetSavingNotification(
          TType item,
          EventMessages eventMessages,
          Dictionary<string, object?> additionalData);

        protected abstract SavedNotification<TType> GetSavedNotification(
          TType item,
          EventMessages eventMessages);

        protected abstract DeletingNotification<TType> GetDeletingNotification(
          TType item,
          EventMessages eventMessages);

        protected abstract DeletedNotification<TType> GetDeletedNotification(
          TType item,
          EventMessages eventMessages);

        public virtual TType? Insert(TType item)
        {
            ArgumentNullException.ThrowIfNull(item);
            using (IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false))
            {
                EventMessages eventMessages = EventMessagesFactory.Get();
                CreatingNotification<TType> creatingNotification = GetCreatingNotification(item, eventMessages);
                if (((ICoreScope)scope).Notifications.PublishCancelable(creatingNotification))
                {
                    ((ICoreScope)scope).Complete();
                    return default;
                }
                if (item.Id == Guid.Empty)
                    item.Id = Guid.NewGuid();
                TEntity entity = Mapper.Map<TEntity>(item) ?? throw new InvalidOperationException("Could not map entity from provided model in insert operation.");
                OperationResult operationResult = Save(scope, entity, item, eventMessages, new Dictionary<string, object>()!);
                if (operationResult.Success)
                {
                    CreatedNotification<TType> createdNotification = GetCreatedNotification(Mapper.Map<TType>(entity) ?? throw new InvalidOperationException("Could not map item from inserted entity."), eventMessages);
                    ((ICoreScope)scope).Notifications.Publish(createdNotification.WithStateFrom(creatingNotification));
                    ((ICoreScope)scope).Complete();
                }
                else if (operationResult.Result == OperationResultType.FailedCancelledByEvent)
                    throw BaseService<TRepository, TType, TEntity>.OperationCanceledException(eventMessages);
            }
            return item;
        }

        private static OperationCanceledException OperationCanceledException(
          EventMessages eventMessages)
        {
            return new OperationCanceledException(string.Join(", ", eventMessages.GetAll().Select(x => x.Message)));
        }

        public virtual IEnumerable<TType> Get()
        {
            using IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, false);
            IEnumerable<TEntity> many = Repository.GetMany();
            ((ICoreScope)scope).Complete();
            return Mapper.MapEnumerable<TEntity, TType>(many);
        }

        public virtual IEnumerable<TType> Get(Guid[] ids)
        {
            using IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, false);
            IEnumerable<TEntity> many = Repository.GetMany(ids);
            ((ICoreScope)scope).Complete();
            return Mapper.MapEnumerable<TEntity, TType>(many);
        }

        public virtual bool Exists(Guid id)
        {
            using IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, false);
            TEntity? entity = Repository.Get(id);
            ((ICoreScope)scope).Complete();
            return entity is not null;
        }

        private OperationResult Save(
          IScope scope,
          TEntity entity,
          TType item,
          EventMessages eventMessages,
          Dictionary<string, object?> additionalData)
        {
            SavingNotification<TType> savingNotification = GetSavingNotification(item, eventMessages, additionalData);
            if (((ICoreScope)scope).Notifications.PublishCancelable(savingNotification))
            {
                ((ICoreScope)scope).Complete();
                return OperationResult.Cancel(eventMessages);
            }
            PerformSave(scope, entity, item);
            SavedNotification<TType> savedNotification = GetSavedNotification(item, eventMessages);
            ((ICoreScope)scope).Notifications.Publish(savedNotification.WithStateFrom(savingNotification));
            return OperationResult.Succeed(eventMessages);
        }

        protected virtual void PerformSave(IScope scope, TEntity entity, TType item)
        {
            Repository.Save(entity);
            AppCache.ClearByKey(_cacheKeys);
        }

        public virtual TType? Get(Guid id)
        {
            using IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, false);
            TEntity? source = Repository.Get(id);
            ((ICoreScope)scope).Complete();
            return Mapper.Map<TType>(source);
        }

        public virtual TType Update(TType item) => Update(item, new Dictionary<string, object>()!);

        public virtual TType Update(TType item, Dictionary<string, object?> additionalData)
        {
            ArgumentNullException.ThrowIfNull(item);
            using (IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, false))
            {
                TEntity? entity1 = Repository.Get(item.Id);
                if (entity1 is null)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new(35, 2);
                    interpolatedStringHandler.AppendLiteral("The ");
                    interpolatedStringHandler.AppendFormatted(typeof(TType).Name);
                    interpolatedStringHandler.AppendLiteral(" ");
                    interpolatedStringHandler.AppendFormatted(item.Name);
                    interpolatedStringHandler.AppendLiteral(" does not exist in DB or cache");
                    throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
                }
                TEntity entity2 = entity1;
                TEntity entity3 = Mapper.Map<TEntity>(item) ?? throw new InvalidOperationException("Could not map entity from provided model in update operation.");
                TEntity? local = entity3;
                int id = entity2.Id;
                local.Id = id;
                EventMessages eventMessages = EventMessagesFactory.Get();
                OperationResult operationResult = Save(scope, entity3, item, eventMessages, additionalData);
                if (operationResult.Success)
                {
                    item = Mapper.Map<TType>(entity3) ?? throw new InvalidOperationException("Could not map item from updated entity.");
                    ((ICoreScope)scope).Complete();
                }
                else if (operationResult.Result == OperationResultType.FailedCancelledByEvent)
                    throw BaseService<TRepository, TType, TEntity>.OperationCanceledException(eventMessages);
            }
            return item;
        }

        public virtual IEnumerable<TType> Update(IEnumerable<TType> items)
        {
            items.ToList().ForEach(p => Update(p));
            return items;
        }

        public virtual void Delete(Guid id)
        {
            TType? type = Get(id);
            if (type is null)
                return;
            Delete(type);
        }

        public virtual bool Delete(TType item)
        {
            using IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            EventMessages eventMessages = EventMessagesFactory.Get();
            DeletingNotification<TType> deletingNotification = GetDeletingNotification(item, eventMessages);
            if (((ICoreScope)scope).Notifications.PublishCancelable(deletingNotification))
            {
                ((ICoreScope)scope).Complete();
                return false;
            }
            PerformDelete(Mapper.Map<TEntity>(item) ?? throw new InvalidOperationException("Could not map entity from provided model in delete operation."), item);
            DeletedNotification<TType> deletedNotification = GetDeletedNotification(item, eventMessages);
            ((ICoreScope)scope).Notifications.Publish(deletedNotification.WithStateFrom(deletingNotification));
            ((ICoreScope)scope).Complete();
            return true;
        }

        protected virtual void PerformDelete(TEntity entity, TType item) => Repository.Delete(entity);
    }

#pragma warning restore CS0618 // Type or member is obsolete
}