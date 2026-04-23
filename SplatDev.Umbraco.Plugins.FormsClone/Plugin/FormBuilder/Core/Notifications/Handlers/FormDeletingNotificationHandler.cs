using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;

using Umbraco.Cms.Core.Events;

namespace FormBuilder.Core.Notifications.Handlers
{
    internal sealed class FormDeletingNotificationHandler(IWorkflowService workflowService) :
      INotificationHandler<FormDeletingNotification>,
      INotificationHandler
    {
        private readonly IWorkflowService _workflowService = workflowService;

        public void Handle(FormDeletingNotification notification) => Handle(notification.DeletedEntities);

        public void Handle(
          IEnumerable<FormDeletingNotification> notifications)
        {
            Handle(notifications.SelectMany(x => x.DeletedEntities));
        }

        private void Handle(IEnumerable<Form> entities)
        {
            foreach (Form entity in entities)
                _workflowService.Delete(entity);
        }
    }
}