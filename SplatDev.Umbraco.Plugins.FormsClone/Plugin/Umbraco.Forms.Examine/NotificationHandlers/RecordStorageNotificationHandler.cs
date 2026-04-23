
// Type: Umbraco.Forms.Examine.NotificationHandlers.RecordStorageNotificationHandler
// Assembly: Umbraco.Forms.Examine, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF5A33E-94A1-42C9-B681-695454D27A51

using System;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services.Notifications;
using Umbraco.Forms.Examine.Indexes;


#nullable enable
namespace Umbraco.Forms.Examine.NotificationHandlers
{
  public class RecordStorageNotificationHandler : 
    INotificationHandler<RecordDeletingNotification>,
    INotificationHandler,
    INotificationHandler<RecordSavingNotification>
  {
    private readonly IUmbracoFormsIndexingHandler _umbracoFormsIndexingHandler;

    public RecordStorageNotificationHandler(
      IUmbracoFormsIndexingHandler umbracoFormsIndexingHandler)
    {
      this._umbracoFormsIndexingHandler = umbracoFormsIndexingHandler;
    }

    public void Handle(RecordDeletingNotification notification) => notification.DeletedEntities.ToList<Record>().ForEach((Action<Record>) (x => this._umbracoFormsIndexingHandler.DeleteRecord(x)));

    public void Handle(RecordSavingNotification notification) => notification.SavedEntities.ToList<Record>().ForEach((Action<Record>) (x => this._umbracoFormsIndexingHandler.ReIndexForRecord(x)));
  }
}
