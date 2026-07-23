
// Type: Umbraco.Forms.Core.Services.Notifications.FormValidateNotification
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Services.Notifications
{
  public sealed class FormValidateNotification : ObjectNotification<Form>
  {
    public FormValidateNotification(
      Form target,
      EventMessages messages,
      HttpContext httpContext,
      ModelStateDictionary modelState)
      : base(target, messages)
    {
      this.Context = httpContext;
      this.ModelState = modelState;
    }

    public Form Form => this.Target;

    public HttpContext Context { get; }

    public ModelStateDictionary ModelState { get; }
  }
}
