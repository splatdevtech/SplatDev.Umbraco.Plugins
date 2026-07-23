
// Type: Umbraco.Forms.Core.Models.DeliveryApi.EntryAcceptedDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.Models.DeliveryApi;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class EntryAcceptedDto : IPostSubmissionDetail
  {
    public string? MessageOnSubmit { get; set; }

    public bool MessageOnSubmitIsHtml { get; set; }

    public Guid? GotoPageOnSubmit { get; set; }

    public IApiContentRoute? GotoPageOnSubmitRoute { get; set; }
  }
}
