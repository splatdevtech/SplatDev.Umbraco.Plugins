using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;
using FormBuilder.Core.Storage.Interfaces;

using System.Data;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace FormBuilder.Core.Services
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class FormService(
      IFormRepository formRepository,
      IUmbracoMapper mapper,
      IScopeProvider scopeProvider,
      IAppPolicyCache appCache,
      IEventMessagesFactory eventMessagesFactory,
      IShortStringHelper shortStringHelper,
      IRecordStorage recordStorage,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IUserService userService) :
      BaseService<IFormRepository, Form, FormEntity>(mapper, scopeProvider, appCache, eventMessagesFactory, formRepository, "Forms.FormStorage.All"),
      IFormService,
      IBaseService<Form, FormEntity>
    {
        private readonly IShortStringHelper _shortStringHelper = shortStringHelper;
        private readonly IRecordStorage _recordStorage = recordStorage;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        private readonly IUserService _userService = userService;

        protected override CreatingNotification<Form> GetCreatingNotification(
          Form item,
          EventMessages eventMessages)
        {
            return new FormCreatingNotification(item, eventMessages);
        }

        protected override CreatedNotification<Form> GetCreatedNotification(
          Form item,
          EventMessages eventMessages)
        {
            return new FormCreatedNotification(item, eventMessages);
        }

        protected override SavingNotification<Form> GetSavingNotification(
          Form item,
          EventMessages eventMessages,
          Dictionary<string, object?> additionalData)
        {
            return new FormSavingNotification(item, eventMessages, additionalData);
        }

        protected override SavedNotification<Form> GetSavedNotification(
          Form item,
          EventMessages eventMessages)
        {
            return new FormSavedNotification(item, eventMessages);
        }

        protected override DeletingNotification<Form> GetDeletingNotification(
          Form item,
          EventMessages eventMessages)
        {
            return new FormDeletingNotification(item, eventMessages);
        }

        protected override DeletedNotification<Form> GetDeletedNotification(
          Form item,
          EventMessages eventMessages)
        {
            return new FormDeletedNotification(item, eventMessages);
        }

        public IEnumerable<Form> SearchForms(
          string query,
          int pageIndex,
          int pageSize,
          out long totalFound)
        {
            using IScope scope = ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true);
            IQuery<FormEntity> query1 = scope.SqlContext.Query<FormEntity>().Where(x => x.Name.Contains(query));
            IList<FormEntity> list = [.. Repository.Get(query1)];
            totalFound = list.Count;
            return Mapper.MapEnumerable<FormEntity, Form>(Repository.Get(query1).Skip(pageIndex).Take(pageSize));
        }

        public override Form? Get(Guid id)
        {
            Form? form = Get(id);
            if (form is null)
                return form;
            form.PopulateEditorDetails(_userService);
            return form;
        }

        public Form? Get(string formDataName) => (Form)(Get().SingleOrDefault(x => x.Name is not null && x.Name.Equals(formDataName, StringComparison.InvariantCultureIgnoreCase)) ?? throw new InvalidOperationException("Unable to get Form with ID: '" + formDataName + "' off DB or from the cache.")).Clone();

        public Form? GetFromCache(string name)
        {
            IEnumerable<Form>? cacheItem = AppCache.GetCacheItem("Forms.FormStorage.All", new Func<IEnumerable<Form>>(Get), new TimeSpan?(TimeSpan.FromMinutes(10L)));
            Form? form = cacheItem?.SingleOrDefault(x => x.Name is not null && x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return form is not null ? (Form)form.Clone() : null;
        }

        public override Form? Insert(Form item)
        {
            item.PopulateCreatedDetails(_backOfficeSecurityAccessor);
            Form? form = Insert(item);
            if (form is null)
                return form;
            form.PopulateEditorDetails(_userService);
            return form;
        }

        public override Form Update(Form item) => Update(item, []);

        public override Form Update(Form item, Dictionary<string, object?> additionalData)
        {
            item.PopulateUpdatedDetails(_backOfficeSecurityAccessor);
            Form form = Update(item, additionalData);
            form.PopulateEditorDetails(_userService);
            return form;
        }

        protected override void PerformSave(IScope scope, FormEntity entity, Form form)
        {
            if (string.IsNullOrEmpty(entity.MessageOnSubmit))
                entity.MessageOnSubmit = "Thank you";
            form.SetFieldAliases(_shortStringHelper);
            PerformSave(scope, entity, form);
        }

        protected override void PerformDelete(FormEntity entity, Form item)
        {
            _recordStorage.DeleteFormRecords(item);
            PerformDelete(entity, item);
        }

        public bool ContainsSensitiveData(Form form)
        {
            List<Field>? allFields = Get(form.Id)?.AllFields;
            if (allFields is null)
                return false;
            foreach (Field allField in form.AllFields)
            {
                Field field = allField;
                Field? persistedFormField = allFields.FirstOrDefault(p => p.Id == field.Id);
                if (FieldContainsSensitiveData(field, persistedFormField))
                    return true;
            }
            return false;
        }

        private static bool FieldContainsSensitiveData(Field field, Field? persistedFormField)
        {
            if (persistedFormField is null && field.ContainsSensitiveData)
                return true;
            return persistedFormField is not null && persistedFormField.ContainsSensitiveData && !field.ContainsSensitiveData;
        }

        public bool FormExists(string formName) => Get().SingleOrDefault(x => x.Name is not null && x.Name.Equals(formName, StringComparison.CurrentCultureIgnoreCase)) is not null;

        public IEnumerable<FormSlim> GetSlim()
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return Mapper.MapEnumerable<FormEntitySlim, FormSlim>(Repository.GetManySlim());
        }

        public FormSlim? GetSlim(Guid id)
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
            {
                FormEntitySlim? slim = Repository.GetSlim(id);
                return slim is not null ? Mapper.Map<FormEntitySlim, FormSlim>(slim) : null;
            }
        }

        public IEnumerable<Form> GetAtRoot()
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return Mapper.MapEnumerable<FormEntity, Form>(Repository.GetAtRoot());
        }

        public IEnumerable<FormSlim> GetAtRootSlim()
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return Mapper.MapEnumerable<FormEntitySlim, FormSlim>(Repository.GetAtRootSlim());
        }

        public IEnumerable<Form> GetFromFolder(Guid parentFolderId)
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return Mapper.MapEnumerable<FormEntity, Form>(Repository.GetFromFolder(parentFolderId));
        }

        public IEnumerable<FormSlim> GetFromFolderSlim(Guid parentFolderId)
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return Mapper.MapEnumerable<FormEntitySlim, FormSlim>(Repository.GetFromFolderSlim(parentFolderId));
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}