
// Type: Umbraco.Forms.Core.Services.FormService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

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
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Repositories;
using Umbraco.Forms.Core.Services.Notifications;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Services
{
    public sealed class FormService :
      BaseService<IFormRepository, Form, FormEntity>,
      IFormService,
      IBaseService<Form, FormEntity>
    {
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IRecordStorage _recordStorage;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IUserService _userService;

        public FormService(
          IFormRepository formRepository,
          IUmbracoMapper mapper,
          IScopeProvider scopeProvider,
          IAppPolicyCache appCache,
          IEventMessagesFactory eventMessagesFactory,
          IShortStringHelper shortStringHelper,
          IRecordStorage recordStorage,
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          IUserService userService)
          : base(mapper, scopeProvider, appCache, eventMessagesFactory, formRepository, "Forms.FormStorage.All")
        {
            this._shortStringHelper = shortStringHelper;
            this._recordStorage = recordStorage;
            this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            this._userService = userService;
        }

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
            using (IScope scope = this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
            {
                IQuery<FormEntity> query1 = scope.SqlContext.Query<FormEntity>().Where(x => x.Name.Contains(query));
                IList<FormEntity> list = this.Repository.Get(query1).ToList<FormEntity>();
                totalFound = list.Count;
                return this.Mapper.MapEnumerable<FormEntity, Form>(this.Repository.Get(query1).Skip<FormEntity>(pageIndex).Take<FormEntity>(pageSize));
            }
        }

        public override Form? Get(Guid id)
        {
            Form form = base.Get(id);
            if (form == null)
                return form;
            form.PopulateEditorDetails(this._userService);
            return form;
        }

        public Form? Get(string formDataName) => (Form)(this.Get().SingleOrDefault<Form>(x => x.Name != null && x.Name.Equals(formDataName, StringComparison.InvariantCultureIgnoreCase)) ?? throw new InvalidOperationException("Unable to get Form with ID: '" + formDataName + "' off DB or from the cache.")).Clone();

        public Form? GetFromCache(string name)
        {
            IEnumerable<Form> cacheItem = this.AppCache.GetCacheItem<IEnumerable<Form>>("Forms.FormStorage.All", new Func<IEnumerable<Form>>(this.Get), new TimeSpan?(TimeSpan.FromMinutes(10L)));
            Form form = cacheItem != null ? cacheItem.SingleOrDefault<Form>(x => x.Name != null && x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) : null;
            return form != null ? (Form)form.Clone() : null;
        }

        public override Form? Insert(Form item)
        {
            item.PopulateCreatedDetails(this._backOfficeSecurityAccessor);
            Form form = base.Insert(item);
            if (form == null)
                return form;
            form.PopulateEditorDetails(this._userService);
            return form;
        }

        public override Form Update(Form item) => this.Update(item, new Dictionary<string, object>());

        public override Form Update(Form item, Dictionary<string, object?> additionalData)
        {
            item.PopulateUpdatedDetails(this._backOfficeSecurityAccessor);
            Form form = base.Update(item, additionalData);
            form.PopulateEditorDetails(this._userService);
            return form;
        }

        protected override void PerformSave(IScope scope, FormEntity entity, Form form)
        {
            if (string.IsNullOrEmpty(entity.MessageOnSubmit))
                entity.MessageOnSubmit = "Thank you";
            form.SetFieldAliases(this._shortStringHelper);
            base.PerformSave(scope, entity, form);
        }

        protected override void PerformDelete(FormEntity entity, Form item)
        {
            this._recordStorage.DeleteFormRecords(item);
            base.PerformDelete(entity, item);
        }

        public bool ContainsSensitiveData(Form form)
        {
            List<Field> allFields = this.Get(form.Id)?.AllFields;
            if (allFields == null)
                return false;
            foreach (Field allField in form.AllFields)
            {
                Field field = allField;
                Field persistedFormField = allFields.FirstOrDefault<Field>(p => p.Id == field.Id);
                if (FormService.FieldContainsSensitiveData(field, persistedFormField))
                    return true;
            }
            return false;
        }

        private static bool FieldContainsSensitiveData(Field field, Field? persistedFormField)
        {
            if (persistedFormField == null && field.ContainsSensitiveData)
                return true;
            return persistedFormField != null && persistedFormField.ContainsSensitiveData && !field.ContainsSensitiveData;
        }

        public bool FormExists(string formName) => this.Get().SingleOrDefault<Form>(x => x.Name != null && x.Name.ToLower() == formName.ToLower()) != null;

        public IEnumerable<FormSlim> GetSlim()
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Mapper.MapEnumerable<FormEntitySlim, FormSlim>(this.Repository.GetManySlim());
        }

        public FormSlim? GetSlim(Guid id)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
            {
                FormEntitySlim slim = this.Repository.GetSlim(id);
                return slim != null ? this.Mapper.Map<FormEntitySlim, FormSlim>(slim) : null;
            }
        }

        public IEnumerable<Form> GetAtRoot()
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Mapper.MapEnumerable<FormEntity, Form>(this.Repository.GetAtRoot());
        }

        public IEnumerable<FormSlim> GetAtRootSlim()
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Mapper.MapEnumerable<FormEntitySlim, FormSlim>(this.Repository.GetAtRootSlim());
        }

        public IEnumerable<Form> GetFromFolder(Guid parentFolderId)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Mapper.MapEnumerable<FormEntity, Form>(this.Repository.GetFromFolder(parentFolderId));
        }

        public IEnumerable<FormSlim> GetFromFolderSlim(Guid parentFolderId)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Mapper.MapEnumerable<FormEntitySlim, FormSlim>(this.Repository.GetFromFolderSlim(parentFolderId));
        }
    }
}
