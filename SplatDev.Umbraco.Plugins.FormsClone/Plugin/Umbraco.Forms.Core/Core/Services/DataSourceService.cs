
// Type: Umbraco.Forms.Core.Services.DataSourceService
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
    internal sealed class DataSourceService :
      BaseService<IDataSourceRepository, FormDataSource, DataSourceEntity>,
      IDataSourceService,
      IBaseService<FormDataSource, DataSourceEntity>
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IUserService _userService;

        public DataSourceService(
          IUmbracoMapper mapper,
          IScopeProvider scopeProvider,
          IAppPolicyCache appCache,
          IEventMessagesFactory eventMessagesFactory,
          IDataSourceRepository dataSourceRepository,
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          IUserService userService)
          : base(mapper, scopeProvider, appCache, eventMessagesFactory, dataSourceRepository, "Forms.DataSource.")
        {
            this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            this._userService = userService;
        }

        protected override CreatingNotification<FormDataSource> GetCreatingNotification(
          FormDataSource item,
          EventMessages eventMessages)
        {
            return new DataSourceCreatingNotification(item, eventMessages);
        }

        protected override CreatedNotification<FormDataSource> GetCreatedNotification(
          FormDataSource item,
          EventMessages eventMessages)
        {
            return new DataSourceCreatedNotification(item, eventMessages);
        }

        protected override SavingNotification<FormDataSource> GetSavingNotification(
          FormDataSource item,
          EventMessages eventMessages,
          Dictionary<string, object?> additionalData)
        {
            return new DataSourceSavingNotification(item, eventMessages);
        }

        protected override SavedNotification<FormDataSource> GetSavedNotification(
          FormDataSource item,
          EventMessages eventMessages)
        {
            return new DataSourceSavedNotification(item, eventMessages);
        }

        protected override DeletingNotification<FormDataSource> GetDeletingNotification(
          FormDataSource item,
          EventMessages eventMessages)
        {
            return new DataSourceDeletingNotification(item, eventMessages);
        }

        protected override DeletedNotification<FormDataSource> GetDeletedNotification(
          FormDataSource item,
          EventMessages eventMessages)
        {
            return new DataSourceDeletedNotification(item, eventMessages);
        }

        public IEnumerable<FormDataSourceSlim> GetSlim(params Guid[] ids)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Mapper.MapEnumerable<DataSourceEntitySlim, FormDataSourceSlim>(this.Repository.GetManySlim(ids));
        }

        public override FormDataSource? Get(Guid id)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
            {
                DataSourceEntity source = this.Repository.Get(id);
                if (source == null)
                    return null;
                FormDataSource formDataSource = this.Mapper.Map<DataSourceEntity, FormDataSource>(source);
                if (formDataSource != null)
                    formDataSource.PopulateEditorDetails(this._userService);
                return formDataSource;
            }
        }

        public override FormDataSource? Insert(FormDataSource item)
        {
            item.PopulateCreatedDetails(this._backOfficeSecurityAccessor);
            FormDataSource formDataSource = base.Insert(item);
            if (formDataSource == null)
                return formDataSource;
            formDataSource.PopulateEditorDetails(this._userService);
            return formDataSource;
        }

        public override FormDataSource Update(FormDataSource item)
        {
            item.PopulateUpdatedDetails(this._backOfficeSecurityAccessor);
            FormDataSource formDataSource = base.Update(item);
            formDataSource.PopulateEditorDetails(this._userService);
            return formDataSource;
        }
    }
}
