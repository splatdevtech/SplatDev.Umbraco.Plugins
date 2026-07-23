using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Interfaces;
using FormBuilder.Core.Prevalues;
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

    internal sealed class PrevalueSourceService(
      IUmbracoMapper mapper,
      IScopeProvider scopeProvider,
      IAppPolicyCache appCache,
      IEventMessagesFactory eventMessagesFactory,
      IPrevalueSourceRepository prevalueSourceRepository,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IUserService userService) :
      BaseService<IPrevalueSourceRepository, FieldPrevalueSource, PrevalueSourceEntity>(mapper, scopeProvider, appCache, eventMessagesFactory, prevalueSourceRepository, "Forms.PreValues."),
      IPrevalueSourceService,
      IBaseService<FieldPrevalueSource, PrevalueSourceEntity>
    {
        private readonly IBackOfficeSecurityAccessor? _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        private readonly IUserService? _userService = userService;

        protected override CreatingNotification<FieldPrevalueSource> GetCreatingNotification(
          FieldPrevalueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceCreatingNotification(item, eventMessages);
        }

        protected override CreatedNotification<FieldPrevalueSource> GetCreatedNotification(
          FieldPrevalueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceCreatedNotification(item, eventMessages);
        }

        protected override SavingNotification<FieldPrevalueSource> GetSavingNotification(
          FieldPrevalueSource item,
          EventMessages eventMessages,
          Dictionary<string, object?> additionalData)
        {
            return new PrevalueSourceSavingNotification(item, eventMessages);
        }

        protected override SavedNotification<FieldPrevalueSource> GetSavedNotification(
          FieldPrevalueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceSavedNotification(item, eventMessages);
        }

        protected override DeletingNotification<FieldPrevalueSource> GetDeletingNotification(
          FieldPrevalueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceDeletingNotification(item, eventMessages);
        }

        protected override DeletedNotification<FieldPrevalueSource> GetDeletedNotification(
          FieldPrevalueSource item,
          EventMessages eventMessages)
        {
            return new PrevalueSourceDeletedNotification(item, eventMessages);
        }

        public IEnumerable<FieldPrevalueSourceSlim> GetSlim(
          params Guid[] ids)
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return Mapper.MapEnumerable<PrevalueSourceEntitySlim, FieldPrevalueSourceSlim>(Repository.GetManySlim(ids));
        }

        public override FieldPrevalueSource? Get(Guid id)
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
            {
                PrevalueSourceEntity? source = Repository.Get(id);
                if (source is null)
                    return null;
                FieldPrevalueSource? fieldPreValueSource = Mapper.Map<PrevalueSourceEntity, FieldPrevalueSource>(source);
                fieldPreValueSource?.PopulateEditorDetails(_userService!);
                return fieldPreValueSource;
            }
        }

        public override FieldPrevalueSource? Insert(FieldPrevalueSource item)
        {
            item.PopulateCreatedDetails(_backOfficeSecurityAccessor!);
            FieldPrevalueSource? fieldPreValueSource = base.Insert(item);
            if (fieldPreValueSource is null)
                return fieldPreValueSource;
            fieldPreValueSource.PopulateEditorDetails(_userService!);
            return fieldPreValueSource;
        }

        public override FieldPrevalueSource Update(FieldPrevalueSource item)
        {
            item.PopulateUpdatedDetails(_backOfficeSecurityAccessor!);
            FieldPrevalueSource? fieldPreValueSource = base.Update(item);
            fieldPreValueSource.PopulateEditorDetails(_userService!);
            return fieldPreValueSource;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}