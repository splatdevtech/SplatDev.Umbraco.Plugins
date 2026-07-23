using FormBuilder.Core.Services.Notifications;
using FormBuilder.Examine.Interfaces;

using Umbraco.Cms.Core.Events;

namespace FormBuilder.Examine.Handlers
{
    public class RecordStorageNotificationHandler(
      IFormBuilderIndexingHandler FormBuildersIndexingHandler) :
      INotificationHandler<RecordDeletingNotification>,
      INotificationHandler,
      INotificationHandler<RecordSavingNotification>
    {
        private readonly IFormBuilderIndexingHandler _FormBuildersIndexingHandler = FormBuildersIndexingHandler;

        public void Handle(RecordDeletingNotification notification) => notification.DeletedEntities.ToList().ForEach(x => _FormBuildersIndexingHandler.DeleteRecord(x));

        public void Handle(RecordSavingNotification notification) => notification.SavedEntities.ToList().ForEach(x => _FormBuildersIndexingHandler.ReIndexForRecord(x));
    }
}