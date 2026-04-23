using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Web.Common.Controllers;

using UmbracoCms.Plugins.SocialMedia.Login.Models;
using UmbracoCms.Plugins.SocialMedia.Login.Services;

namespace UmbracoCms.Plugins.SocialMedia.Login.Controllers
{
    [Route("umbraco/api/SocialLoginApi/[action]")]
    public class SocialLoginApiController(ISocialLoginService socialLoginService) : UmbracoApiController
    {
        private readonly ISocialLoginService _socialLoginService = socialLoginService;

        [HttpGet]
        public IActionResult GetProviders()
        {
            var providers = _socialLoginService.GetEnabledProviders();
            return Ok(providers);
        }

        [HttpGet]
        public IActionResult GetLoginUrl(SocialProvider provider, string redirectUri)
        {
            if (string.IsNullOrWhiteSpace(redirectUri))
                return BadRequest("redirectUri is required.");

            try
            {
                var loginUrl = _socialLoginService.GetLoginUrl(provider, redirectUri);
                return Ok(new { provider = provider.ToString(), loginUrl });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
