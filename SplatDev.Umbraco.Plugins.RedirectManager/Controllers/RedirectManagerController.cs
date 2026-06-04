using Microsoft.AspNetCore.Mvc;
#if NET10_0_OR_GREATER
using Umbraco.Cms.Api.Management.Controllers;
#else
using Umbraco.Cms.Web.BackOffice.Controllers;
#endif
using SplatDev.Umbraco.Plugins.RedirectManager.Models;
using SplatDev.Umbraco.Plugins.RedirectManager.Repositories;

namespace SplatDev.Umbraco.Plugins.RedirectManager.Controllers
{
#if NET10_0_OR_GREATER
    [Route("umbraco/management/api/v{version:apiVersion}/redirect-manager")]
    public class RedirectManagerController(RedirectUrlsRepository redirectUrlsRepository) : ManagementApiControllerBase
#else
    public class RedirectManagerController(RedirectUrlsRepository redirectUrlsRepository) : UmbracoAuthorizedApiController
#endif
    {
        private readonly RedirectUrlsRepository redirectUrlsRepository = redirectUrlsRepository;

        [HttpGet("")]
        public IEnumerable<RedirectUrl>? GetAll()
        {
            return redirectUrlsRepository.GetAllRedirectionUrls();
        }

        [HttpGet("{id:int}")]
        public RedirectUrl? Get(int id)
        {
            return redirectUrlsRepository.GetRedirectionUrl(id);
        }

        [HttpPost("")]
        public void Post(RedirectUrl url)
        {
            redirectUrlsRepository.AddRedirectionUrl(url);
        }

        [HttpPut("")]
        public void Put(RedirectUrl url)
        {
            redirectUrlsRepository.EditRedirectionUrl(url);
        }

        [HttpDelete("{id:int}")]
        public void Delete(int id)
        {
            redirectUrlsRepository.DeleteRedirectionUrl(id);
        }

        [HttpDelete("all")]
        public void DeleteAll()
        {
            redirectUrlsRepository.DeleteAll();
        }
    }
}
