
// Type: Umbraco.Forms.Core.Models.DeliveryApi.IPostSubmissionDetail
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.Models.DeliveryApi;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public interface IPostSubmissionDetail
  {
    string? MessageOnSubmit { get; set; }

    bool MessageOnSubmitIsHtml { get; set; }

    Guid? GotoPageOnSubmit { get; set; }

    IApiContentRoute? GotoPageOnSubmitRoute { get; set; }
  }
}
