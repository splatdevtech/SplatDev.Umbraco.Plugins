
// Type: Umbraco.Forms.Core.Services.WorkflowService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Data;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Repositories;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services.Notifications;

using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Services
{
    internal sealed class WorkflowService :
      BaseService<IWorkflowRepository, Workflow, WorkflowEntity>,
      IWorkflowService,
      IBaseService<Workflow, WorkflowEntity>
    {
        private readonly WorkflowCollectionFactory _workflowCollectionFactory;

        public WorkflowService(
          IUmbracoMapper mapper,
          IScopeProvider scopeProvider,
          IAppPolicyCache appCache,
          IEventMessagesFactory eventMessagesFactory,
          IWorkflowRepository workflowRepository,
          WorkflowCollectionFactory workflowCollectionFactory)
          : base(mapper, scopeProvider, appCache, eventMessagesFactory, workflowRepository, "Forms.Workflow.")
        {
            this._workflowCollectionFactory = workflowCollectionFactory;
        }

        protected override CreatingNotification<Workflow> GetCreatingNotification(
          Workflow item,
          EventMessages eventMessages)
        {
            return new WorkflowCreatingNotification(item, eventMessages);
        }

        protected override CreatedNotification<Workflow> GetCreatedNotification(
          Workflow item,
          EventMessages eventMessages)
        {
            return new WorkflowCreatedNotification(item, eventMessages);
        }

        protected override SavingNotification<Workflow> GetSavingNotification(
          Workflow item,
          EventMessages eventMessages,
          Dictionary<string, object?> additionalData)
        {
            return new WorkflowSavingNotification(item, eventMessages);
        }

        protected override SavedNotification<Workflow> GetSavedNotification(
          Workflow item,
          EventMessages eventMessages)
        {
            return new WorkflowSavedNotification(item, eventMessages);
        }

        protected override DeletingNotification<Workflow> GetDeletingNotification(
          Workflow item,
          EventMessages eventMessages)
        {
            return new WorkflowDeletingNotification(item, eventMessages);
        }

        protected override DeletedNotification<Workflow> GetDeletedNotification(
          Workflow item,
          EventMessages eventMessages)
        {
            return new WorkflowDeletedNotification(item, eventMessages);
        }

        public IEnumerable<Workflow> Insert(
          Form form,
          IEnumerable<Workflow> workflows)
        {
            ArgumentNullException.ThrowIfNull(form, nameof(form));
            ArgumentNullException.ThrowIfNull(workflows, nameof(workflows));
            foreach (Workflow workflow in workflows.Where<Workflow>(x => !x.Deleted))
                this.Insert(form, workflow);
            return workflows;
        }

        public Workflow? Insert(Form form, Workflow workflow)
        {
            ArgumentNullException.ThrowIfNull(form, nameof(form));
            ArgumentNullException.ThrowIfNull(workflow, nameof(workflow));
            return this.Insert(form.Id, workflow);
        }

        private Workflow? Insert(Guid formId, Workflow workflow)
        {
            ArgumentNullException.ThrowIfNull(workflow, nameof(workflow));
            workflow.Form = formId;
            return this.Insert(workflow);
        }

        public List<Workflow> Get(Form form)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Mapper.MapEnumerable<WorkflowEntity, Workflow>(this.Repository.GetFor(form)).ToList<Workflow>();
        }

        public IEnumerable<WorkflowSlim> GetSlim(Guid formId)
        {
            using (this.ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, null, false, true))
                return this.Mapper.MapEnumerable<WorkflowEntitySlim, WorkflowSlim>(this.Repository.GetForSlim(formId)).ToList<WorkflowSlim>();
        }

        public IEnumerable<WorkflowSlim> GetSlim(Form form) => this.GetSlim(form.Id);

        public List<Workflow> GetActiveWorkFlows(Form form, FormState state) => this.Get(form).Where<Workflow>(p => p.Active && p.ExecutesOn == state).OrderBy<Workflow, int>(x => x.SortOrder).ToList<Workflow>();

        public void Delete(Form form)
        {
            foreach (BaseSlim baseSlim in this.GetSlim(form))
                this.Delete(baseSlim.Id);
        }
    }
}
