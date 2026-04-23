
// Type: Umbraco.Forms.Core.Services.PrevalueSourceService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Data;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Repositories;
using Umbraco.Forms.Core.Services.Notifications;

using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Services
{
    internal sealed class PrevalueSourceService :
      BaseService<IPrevalueSourceRepository, FieldPreValueSource, PrevalueSourceEntity>,
      IPrevalueSourceService,
      IBaseService<FieldPreValueSource, PrevalueSourceEntity>
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IUserService _userService;

        public PrevalueSourceService(
          IUmbracoMapper mapper,
          IScopeProvider scopeProvider,
          IAppPolicyCache appCache,
          IEventMessagesFactory eventMessagesFactory,
          IPrevalueSourceRepository prevalueSourceRepository,
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          IUserService userService)
          : base(mapper, scopeProvider, appCache, eventMessagesFactory, prevalueSourceRepository, "Forms.PreValues.")
        {
            this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            this._userService = userService;
        }

        protected override CreatingNotification<FieldPreValueSource> GetCreatingNotification(
          FieldPreValueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceCreatingNotification(item, eventMessages);
        }

        protected override CreatedNotification<FieldPreValueSource> GetCreatedNotification(
          FieldPreValueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceCreatedNotification(item, eventMessages);
        }

        protected override SavingNotification<FieldPreValueSource> GetSavingNotification(
          FieldPreValueSource item,
          EventMessages eventMessages,
          Dictionary<string, object?> additionalData)
        {
            return new PrevalueSourceSavingNotification(item, eventMessages);
        }

        protected override SavedNotification<FieldPreValueSource> GetSavedNotification(
          FieldPreValueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceSavedNotification(item, eventMessages);
        }

        protected override DeletingNotification<FieldPreValueSource> GetDeletingNotification(
          FieldPreValueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceDeletingNotification(item, eventMessages);
        }

        protected override DeletedNotification<FieldPreValueSource> GetDeletedNotification(
          FieldPreValueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceDeletedNotification(item, eventMessages);
        }

        public IEnumerable<FieldPreValueSourceSlim> GetSlim(
          params Guid[] ids)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Mapper.MapEnumerable<PrevalueSourceEntitySlim, FieldPreValueSourceSlim>(this.Repository.GetManySlim(ids));
        }

        public override FieldPreValueSource? Get(Guid id)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
            {
                PrevalueSourceEntity source = this.Repository.Get(id);
                if (source == null)
                    return null;
                FieldPreValueSource fieldPreValueSource = this.Mapper.Map<PrevalueSourceEntity, FieldPreValueSource>(source);
                if (fieldPreValueSource != null)
                    fieldPreValueSource.PopulateEditorDetails(this._userService);
                return fieldPreValueSource;
            }
        }

        public override FieldPreValueSource? Insert(FieldPreValueSource item)
        {
            item.PopulateCreatedDetails(this._backOfficeSecurityAccessor);
            FieldPreValueSource fieldPreValueSource = base.Insert(item);
            if (fieldPreValueSource == null)
                return fieldPreValueSource;
            fieldPreValueSource.PopulateEditorDetails(this._userService);
            return fieldPreValueSource;
        }

        public override FieldPreValueSource Update(FieldPreValueSource item)
        {
            item.PopulateUpdatedDetails(this._backOfficeSecurityAccessor);
            FieldPreValueSource fieldPreValueSource = base.Update(item);
            fieldPreValueSource.PopulateEditorDetails(this._userService);
            return fieldPreValueSource;
        }
    }
}
