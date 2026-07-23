using FormBuilder.Core.DataSources;
using FormBuilder.Core.Extensions;
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
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Core.Services
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class DataSourceService(
      IUmbracoMapper mapper,
      IScopeProvider scopeProvider,
      IAppPolicyCache appCache,
      IEventMessagesFactory eventMessagesFactory,
      IDataSourceRepository dataSourceRepository,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IUserService userService) :
      BaseService<IDataSourceRepository, FormDataSource, DataSourceEntity>(mapper, scopeProvider, appCache, eventMessagesFactory, dataSourceRepository, "Forms.DataSource."),
      IDataSourceService,
      IBaseService<FormDataSource, DataSourceEntity>
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        private readonly IUserService _userService = userService;

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
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return Mapper.MapEnumerable<DataSourceEntitySlim, FormDataSourceSlim>(Repository.GetManySlim(ids));
        }

        public override FormDataSource? Get(Guid id)
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
            {
                DataSourceEntity? source = Repository.Get(id);
                if (source is null)
                    return null;
                FormDataSource? formDataSource = Mapper.Map<DataSourceEntity, FormDataSource>(source);
                formDataSource?.PopulateEditorDetails(_userService);
                return formDataSource;
            }
        }

        public override FormDataSource? Insert(FormDataSource item)
        {
            item.PopulateCreatedDetails(_backOfficeSecurityAccessor);
            FormDataSource? formDataSource = Insert(item);
            if (formDataSource is null)
                return formDataSource;
            formDataSource.PopulateEditorDetails(_userService);
            return formDataSource;
        }

        public override FormDataSource Update(FormDataSource item)
        {
            item.PopulateUpdatedDetails(_backOfficeSecurityAccessor);
            FormDataSource formDataSource = Update(item);
            formDataSource.PopulateEditorDetails(_userService);
            return formDataSource;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}