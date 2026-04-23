using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class FormValidateNotification(
      Form target,
      EventMessages messages,
      HttpContext httpContext,
      ModelStateDictionary modelState) : ObjectNotification<Form>(target, messages)
    {
        public Form Form => Target;

        public HttpContext Context { get; } = httpContext;

        public ModelStateDictionary ModelState { get; } = modelState;
    }
}