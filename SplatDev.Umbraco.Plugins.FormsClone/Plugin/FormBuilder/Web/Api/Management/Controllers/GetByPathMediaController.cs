using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Management.Factories;
using Umbraco.Cms.Api.Management.ViewModels.Media;
using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single media item by path.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetByPathMediaController(
      IMediaService mediaService,
      IMediaPresentationFactory mediaPresentationFactory) : MediaControllerBase(mediaService)
    {
        private readonly IMediaPresentationFactory _mediaPresentationFactory = mediaPresentationFactory;

        /// <summary>
        /// Management API endpoint for retrieving a single media item by path.
        /// </summary>
        [HttpGet("by-path")]
        [ProducesResponseType(typeof(MediaResponseModel), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetByPath(string path)
        {
            IMedia? mediaByPath = MediaService.GetMediaByPath(Uri.UnescapeDataString(path));
            return mediaByPath is null ? NotFound() : Ok(_mediaPresentationFactory.CreateResponseModel(mediaByPath));
        }
    }
}