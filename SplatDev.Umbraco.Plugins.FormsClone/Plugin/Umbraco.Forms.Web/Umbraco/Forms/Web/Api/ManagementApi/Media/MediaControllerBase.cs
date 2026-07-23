
// Type: Umbraco.Forms.Web.Api.ManagementApi.Media.MediaControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Media
{
  [ApiExplorerSettings(GroupName = "Media")]
  [Route("/umbraco/forms/management/api/v1/media")]
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "ManageForms")]
  public abstract class MediaControllerBase : FormsManagementApiControllerBase
  {
    protected MediaControllerBase(IMediaService mediaService) => this.MediaService = mediaService;

    protected IMediaService MediaService { get; }
  }
}
