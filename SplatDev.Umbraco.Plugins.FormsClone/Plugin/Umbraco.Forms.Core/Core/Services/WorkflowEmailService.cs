
// Type: Umbraco.Forms.Core.Services.WorkflowEmailService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public class WorkflowEmailService : IWorkflowEmailService
  {
    private readonly IEmailSender _emailSender;
    private readonly ContentSettings _contentSettings;
    private readonly GlobalSettings _globalSettings;
    private readonly ILogger<WorkflowEmailService> _logger;

    public WorkflowEmailService(
      IEmailSender emailSender,
      IOptions<ContentSettings> contentSettingsConfig,
      IOptions<GlobalSettings> globalSettingsConfig,
      ILogger<WorkflowEmailService> logger)
    {
      this._emailSender = emailSender;
      this._contentSettings = contentSettingsConfig.Value;
      this._globalSettings = globalSettingsConfig.Value;
      this._logger = logger;
    }

    public async Task SendEmailAsync(SendEmailArgs args)
    {
      string senderAddress = this.GetSenderAddress(args);
      if (!WorkflowEmailService.IsValidEmailAddress(senderAddress))
      {
        string message = "Error sending email, invalid sender address.";
        this._logger.LogError(message);
        throw new InvalidOperationException(message);
      }
      string[] array1 = this.ParseMailAddresses(args.RecipientEmail, "recipient").ToArray<string>();
      if (array1.Length == 0)
      {
        string message = "Error sending email, invalid recipient address(es).";
        this._logger.LogError(message);
        throw new InvalidOperationException(message);
      }
      string[] array2 = this.ParseMailAddresses(args.CcEmail, "recipient (CC)").ToArray<string>();
      string[] array3 = this.ParseMailAddresses(args.BccEmail, "recipient (BCC)").ToArray<string>();
      string[] array4 = this.ParseMailAddresses(args.ReplyToEmail, "reply to").ToArray<string>();
      EmailMessage message1 = new EmailMessage(senderAddress, array1, array2, array3, array4, args.Subject, args.Body, args.IsBodyHtml, args.Attachments);
      if (!this._emailSender.CanSendRequiredEmail())
        this._logger.LogError("Core email service reports that email message cannot be sent.");
      else
        await this._emailSender.SendAsync(message1, "UmbracoFormsWorkflow").ConfigureAwait(false);
    }

    private string? GetSenderAddress(SendEmailArgs args)
    {
      string senderAddress = args.SenderEmail;
      if (string.IsNullOrWhiteSpace(senderAddress))
        senderAddress = this._contentSettings.Notifications.Email;
      if (string.IsNullOrWhiteSpace(senderAddress))
        senderAddress = this._globalSettings.Smtp?.From;
      return senderAddress;
    }

    private static bool IsValidEmailAddress(string? address) => !string.IsNullOrWhiteSpace(address) && address.Contains("@") && !address.EndsWith("@");

    private IEnumerable<string> ParseMailAddresses(string addresses, string addressType)
    {
      HashSet<string> mailAddresses = new HashSet<string>();
      if (addresses == null)
        return (IEnumerable<string>) mailAddresses;
      foreach (string address in (IEnumerable<string>) addresses.Split(new char[2]
      {
        ';',
        ','
      }, StringSplitOptions.RemoveEmptyEntries))
      {
        if (!WorkflowEmailService.IsValidEmailAddress(address))
          this._logger.LogWarning("Error sending email, invalid " + addressType + " address.");
        else
          mailAddresses.Add(address);
      }
      return (IEnumerable<string>) mailAddresses;
    }
  }
}
