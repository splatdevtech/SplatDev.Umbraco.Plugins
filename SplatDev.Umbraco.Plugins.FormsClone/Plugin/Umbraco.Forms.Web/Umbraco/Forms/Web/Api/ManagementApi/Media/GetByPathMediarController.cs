
// Type: Umbraco.Forms.Web.Api.ManagementApi.Media.GetByPathMediarController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using Umbraco.Cms.Api.Management.Factories;
using Umbraco.Cms.Api.Management.ViewModels.Media;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Media
{
  public class GetByPathMediarController : MediaControllerBase
  {
    private readonly IMediaPresentationFactory _mediaPresentationFactory;

    public GetByPathMediarController(
      IMediaService mediaService,
      IMediaPresentationFactory mediaPresentationFactory)
      : base(mediaService)
    {
      this._mediaPresentationFactory = mediaPresentationFactory;
    }

    [HttpGet("by-path")]
    [ProducesResponseType(typeof (MediaResponseModel), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetByPath(string path)
    {
      IMedia mediaByPath = this.MediaService.GetMediaByPath(Uri.UnescapeDataString(path));
      return mediaByPath == null ? (IActionResult) this.NotFound() : (IActionResult) this.Ok((object) this._mediaPresentationFactory.CreateResponseModel(mediaByPath));
    }
  }
}
