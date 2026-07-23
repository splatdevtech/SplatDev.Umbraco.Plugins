
// Type: Umbraco.Forms.Web.Helpers.FormSubmissionHelper
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Security;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services.Notifications;


#nullable enable
namespace Umbraco.Forms.Web.Helpers
{
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
      record.IP = FormSubmissionHelper.GetRequestIpAddressToRecord(httpContext, enableRecordingOfIp);
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
      string str = (string) null;
      if (httpContext.User != null && httpContext.User.Identity != null)
      {
        flag = httpContext.User.Identity.IsAuthenticated;
        str = httpContext.User.Identity.Name;
      }
      if (!flag || string.IsNullOrEmpty(str) || httpContext.User == null)
        return;
      MemberIdentityUser userAsync = await ((IUmbracoUserManager<MemberIdentityUser>) memberManager).GetUserAsync(httpContext.User);
      if (userAsync == null)
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
      FormValidateNotification notification = new FormValidateNotification(form, messages, httpContext, modelState);
      eventAggregator.Publish<FormValidateNotification>(notification);
    }
  }
}
