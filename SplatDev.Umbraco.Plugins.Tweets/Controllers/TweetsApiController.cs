using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using SplatDev.Umbraco.Plugins.Tweets.Services;

namespace SplatDev.Umbraco.Plugins.Tweets.Controllers;

[Route("umbraco/api/tweets")]
public class TweetsApiController : UmbracoApiController
{
    private readonly ITweetsService _service;

    public TweetsApiController(ITweetsService service)
    {
        _service = service;
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetTweets()
    {
        var tweets = await _service.GetCachedTweetsAsync();
        return Ok(tweets);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        try
        {
            await _service.RefreshFromApiAsync();
            var tweets = await _service.GetCachedTweetsAsync();
            return Ok(new { success = true, count = tweets.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}
