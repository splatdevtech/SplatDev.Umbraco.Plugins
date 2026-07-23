using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Interfaces;
using FormBuilder.Core.Providers.Factories;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;

using System.Data;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Services
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class WorkflowService(
      IUmbracoMapper mapper,
      IScopeProvider scopeProvider,
      IAppPolicyCache appCache,
      IEventMessagesFactory eventMessagesFactory,
      IWorkflowRepository workflowRepository,
      WorkflowCollectionFactory workflowCollectionFactory) :
      BaseService<IWorkflowRepository, Workflow, WorkflowEntity>(mapper, scopeProvider, appCache, eventMessagesFactory, workflowRepository, "Forms.Workflow."),
      IWorkflowService,
      IBaseService<Workflow, WorkflowEntity>
    {
        private readonly WorkflowCollectionFactory _workflowCollectionFactory = workflowCollectionFactory;

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
            foreach (Workflow workflow in workflows.Where(x => !x.Deleted))
                Insert(form, workflow);
            return workflows;
        }

        public Workflow? Insert(Form form, Workflow workflow)
        {
            ArgumentNullException.ThrowIfNull(form, nameof(form));
            ArgumentNullException.ThrowIfNull(workflow, nameof(workflow));
            return Insert(form.Id, workflow);
        }

        private Workflow? Insert(Guid formId, Workflow workflow)
        {
            ArgumentNullException.ThrowIfNull(workflow, nameof(workflow));
            workflow.Form = formId;
            return Insert(workflow);
        }

        public List<Workflow> Get(Form form)
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return [.. Mapper.MapEnumerable<WorkflowEntity, Workflow>(Repository.GetFor(form))];
        }

        public IEnumerable<WorkflowSlim> GetSlim(Guid formId)
        {
            using (ScopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.None, null, null, new bool?(), false, true))
                return [.. Mapper.MapEnumerable<WorkflowEntitySlim, WorkflowSlim>(Repository.GetForSlim(formId))];
        }

        public IEnumerable<WorkflowSlim> GetSlim(Form form) => GetSlim(form.Id);

        public List<Workflow> GetActiveWorkFlows(Form form, FormState state) => [.. Get(form).Where(p => p.Active && p.ExecutesOn == state).OrderBy(x => x.SortOrder)];

        public void Delete(Form form)
        {
            foreach (BaseSlim baseSlim in GetSlim(form))
                Delete(baseSlim.Id);
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}