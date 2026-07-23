
// Type: Umbraco.Forms.Core.Services.SendEmailArgs
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Cms.Core.Models.Email;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public class SendEmailArgs
  {
    public string SenderEmail { get; set; } = string.Empty;

    public string ReplyToEmail { get; set; } = string.Empty;

    public string RecipientEmail { get; set; } = string.Empty;

    public string CcEmail { get; set; } = string.Empty;

    public string BccEmail { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public bool IsBodyHtml { get; set; } = true;

    public IEnumerable<EmailMessageAttachment> Attachments { get; set; } = (IEnumerable<EmailMessageAttachment>) new List<EmailMessageAttachment>();
  }
}
