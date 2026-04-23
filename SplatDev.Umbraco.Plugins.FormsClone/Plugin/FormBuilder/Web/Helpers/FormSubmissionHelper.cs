using FormBuilder.Core.Enums;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Notifications;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Security;

namespace FormBuilder.Web.Helpers
{
    /// <summary>
    /// Contains helper functions used to reduce code duplication across form submission endpoints (client and server side rendering).
    /// </summary>
    /// <remarks>
    /// Forms can be submitted both in a standard form post to     /// as via an AJAX post to the API endpont at     /// </remarks>
    internal static class FormSubmissionHelper
    {
        public static void InitializeRecord(
          Record record,
          Form form,
          HttpContext httpContext,
          bool enableRecordingOfIp,
          IDictionary<string, string?>? additionalData)
        {
            record.Form = form.Id;
            record.State = FormState.Submitted;
            record.IP = GetRequestIpAddressToRecord(httpContext, enableRecordingOfIp);
            record.Culture = Thread.CurrentThread.CurrentCulture.Name;
            record.SetAdditionalData(additionalData);
        }

        private static string GetRequestIpAddressToRecord(
          HttpContext httpContext,
          bool enableRecordingOfIp)
        {
            return !enableRecordingOfIp ? string.Empty : httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        public static async Task ApplyMemberKey(
          Record record,
          HttpContext httpContext,
          IMemberManager memberManager)
        {
            bool flag = false;
            string? str = null;
            if (httpContext.User is not null && httpContext.User.Identity is not null)
            {
                flag = httpContext.User.Identity.IsAuthenticated;
                str = httpContext.User.Identity.Name;
            }
            if (!flag || string.IsNullOrEmpty(str) || httpContext.User is null)
                return;
            MemberIdentityUser? userAsync = await memberManager.GetUserAsync(httpContext.User);
            if (userAsync is null)
                return;
            record.MemberKey = userAsync.Key.ToString();
        }

        public static void PublishFormValidateNotification(
          IEventMessagesFactory eventMessagesFactory,
          IEventAggregator eventAggregator,
          HttpContext httpContext,
          ModelStateDictionary modelState,
          Form form)
        {
            EventMessages messages = eventMessagesFactory.Get();
            FormValidateNotification notification = new(form, messages, httpContext, modelState);
            eventAggregator.Publish(notification);
        }
    }
}