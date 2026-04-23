
// Type: Umbraco.Forms.Core.Services.BaseService`3
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

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
using Umbraco.Forms.Core.Interfaces;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Services
{
    public abstract class BaseService<TRepository, TType, TEntity> : IBaseService<TType, TEntity>
      where TRepository : IReadWriteQueryRepository<Guid, TEntity>
      where TType : class, IType
      where TEntity : IEntity
    {
        private readonly string _cacheKeys;

        public BaseService(
          IUmbracoMapper mapper,
          IScopeProvider scopeProvider,
          IAppPolicyCache appCache,
          IEventMessagesFactory eventMessagesFactory,
          TRepository repository,
          string cacheKeys)
        {
            this.Mapper = mapper;
            this.ScopeProvider = scopeProvider;
            this.AppCache = appCache;
            this.EventMessagesFactory = eventMessagesFactory;
            this.Repository = repository;
            this._cacheKeys = cacheKeys;
        }

        protected TRepository Repository { get; }

        protected IUmbracoMapper Mapper { get; }

        protected IScopeProvider ScopeProvider { get; }

        protected IAppPolicyCache AppCache { get; }

        protected IEventMessagesFactory EventMessagesFactory { get; }

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
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                EventMessages eventMessages = this.EventMessagesFactory.Get();
                CreatingNotification<TType> creatingNotification = this.GetCreatingNotification(item, eventMessages);
                if (scope.Notifications.PublishCancelable(creatingNotification))
                {
                    scope.Complete();
                    return default(TType);
                }
                if (item.Id == Guid.Empty)
                    item.Id = Guid.NewGuid();
                TEntity entity = this.Mapper.Map<TEntity>(item) ?? throw new InvalidOperationException("Could not map entity from provided model in insert operation.");
                OperationResult operationResult = this.Save(scope, entity, item, eventMessages, new Dictionary<string, object>());
                if (operationResult.Success)
                {
                    CreatedNotification<TType> createdNotification = this.GetCreatedNotification(this.Mapper.Map<TType>(entity) ?? throw new InvalidOperationException("Could not map item from inserted entity."), eventMessages);
                    scope.Notifications.Publish(createdNotification.WithStateFrom<CreatedNotification<TType>, CreatingNotification<TType>>(creatingNotification));
                    scope.Complete();
                }
                else if (operationResult.Result == OperationResultType.FailedCancelledByEvent)
                    throw BaseService<TRepository, TType, TEntity>.OperationCanceledException(eventMessages);
            }
            return item;
        }

        private static System.OperationCanceledException OperationCanceledException(
          EventMessages eventMessages)
        {
            return new System.OperationCanceledException(string.Join(", ", eventMessages.GetAll().Select<EventMessage, string>(x => x.Message)));
        }

        public virtual IEnumerable<TType> Get()
        {
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, false))
            {
                IEnumerable<TEntity> many = this.Repository.GetMany();
                scope.Complete();
                return this.Mapper.MapEnumerable<TEntity, TType>(many);
            }
        }

        public virtual IEnumerable<TType> Get(Guid[] ids)
        {
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, false))
            {
                IEnumerable<TEntity> many = this.Repository.GetMany(ids);
                scope.Complete();
                return this.Mapper.MapEnumerable<TEntity, TType>(many);
            }
        }

        public virtual bool Exists(Guid id)
        {
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, false))
            {
                TEntity entity = this.Repository.Get(id);
                scope.Complete();
                return entity != null;
            }
        }

        private OperationResult Save(
          IScope scope,
          TEntity entity,
          TType item,
          EventMessages eventMessages,
          Dictionary<string, object?> additionalData)
        {
            SavingNotification<TType> savingNotification = this.GetSavingNotification(item, eventMessages, additionalData);
            if (scope.Notifications.PublishCancelable(savingNotification))
            {
                scope.Complete();
                return OperationResult.Cancel(eventMessages);
            }
            this.PerformSave(scope, entity, item);
            SavedNotification<TType> savedNotification = this.GetSavedNotification(item, eventMessages);
            scope.Notifications.Publish(savedNotification.WithStateFrom<SavedNotification<TType>, SavingNotification<TType>>(savingNotification));
            return OperationResult.Succeed(eventMessages);
        }

        protected virtual void PerformSave(IScope scope, TEntity entity, TType item)
        {
            this.Repository.Save(entity);
            this.AppCache.ClearByKey(this._cacheKeys);
        }

        public virtual TType? Get(Guid id)
        {
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, false))
            {
                TEntity source = this.Repository.Get(id);
                scope.Complete();
                return this.Mapper.Map<TType>(source);
            }
        }

        public virtual TType Update(TType item) => this.Update(item, new Dictionary<string, object>());

        public virtual TType Update(TType item, Dictionary<string, object?> additionalData)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, false))
            {
                TEntity entity1 = this.Repository.Get(item.Id);
                if (entity1 == null)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
                    interpolatedStringHandler.AppendLiteral("The ");
                    interpolatedStringHandler.AppendFormatted(typeof(TType).Name);
                    interpolatedStringHandler.AppendLiteral(" ");
                    interpolatedStringHandler.AppendFormatted(item.Name);
                    interpolatedStringHandler.AppendLiteral(" does not exist in DB or cache");
                    throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
                }
                TEntity entity2 = entity1;
                TEntity entity3 = this.Mapper.Map<TEntity>(item) ?? throw new InvalidOperationException("Could not map entity from provided model in update operation.");
                ref TEntity local = ref entity3;

                int id = entity2.Id;
                local.Id = id;
                EventMessages eventMessages = this.EventMessagesFactory.Get();
                OperationResult operationResult = this.Save(scope, entity3, item, eventMessages, additionalData);
                if (operationResult.Success)
                {
                    item = this.Mapper.Map<TType>(entity3) ?? throw new InvalidOperationException("Could not map item from updated entity.");
                    scope.Complete();
                }
                else if (operationResult.Result == OperationResultType.FailedCancelledByEvent)
                    throw BaseService<TRepository, TType, TEntity>.OperationCanceledException(eventMessages);
            }
            return item;
        }

        public virtual IEnumerable<TType> Update(IEnumerable<TType> items)
        {
            items.ToList<TType>().ForEach(p => this.Update(p));
            return items;
        }

        public virtual void Delete(Guid id)
        {
            TType type = this.Get(id);
            if (type == null)
                return;
            this.Delete(type);
        }

        public virtual bool Delete(TType item)
        {
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                EventMessages eventMessages = this.EventMessagesFactory.Get();
                DeletingNotification<TType> deletingNotification = this.GetDeletingNotification(item, eventMessages);
                if (scope.Notifications.PublishCancelable(deletingNotification))
                {
                    scope.Complete();
                    return false;
                }
                this.PerformDelete(this.Mapper.Map<TEntity>(item) ?? throw new InvalidOperationException("Could not map entity from provided model in delete operation."), item);
                DeletedNotification<TType> deletedNotification = this.GetDeletedNotification(item, eventMessages);
                scope.Notifications.Publish(deletedNotification.WithStateFrom<DeletedNotification<TType>, DeletingNotification<TType>>(deletingNotification));
                scope.Complete();
            }
            return true;
        }

        protected virtual void PerformDelete(TEntity entity, TType item) => this.Repository.Delete(entity);
    }
}
