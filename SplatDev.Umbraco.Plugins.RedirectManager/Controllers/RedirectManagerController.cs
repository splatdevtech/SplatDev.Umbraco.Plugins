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
    [Route("umbraco/management/api/v1/redirectmanager")]
#if NET10_0_OR_GREATER
    public class RedirectManagerController(RedirectUrlsRepository redirectUrlsRepository) : ManagementApiControllerBase
#else
    public class RedirectManagerController(RedirectUrlsRepository redirectUrlsRepository) : UmbracoAuthorizedApiController
#endif
    {
        private readonly RedirectUrlsRepository redirectUrlsRepository = redirectUrlsRepository;

        [HttpGet]
        public IEnumerable<RedirectUrl>? GetAll()
        {
            return redirectUrlsRepository.GetAllRedirectionUrls();
        }

        [HttpGet("{id:int}")]
        public RedirectUrl? Get(int id)
        {
            return redirectUrlsRepository.GetRedirectionUrl(id);
        }

        [HttpPost]
        public void Post(RedirectUrl url)
        {
            redirectUrlsRepository.AddRedirectionUrl(url);
        }

        [HttpPut]
        public void Put(RedirectUrl url)
        {
            redirectUrlsRepository.EditRedirectionUrl(url);
        }

        [HttpDelete]
        public void Delete(int id)
        {
            redirectUrlsRepository.DeleteRedirectionUrl(id);
        }

        [HttpDelete]
        public void DeleteAll()
        {
            redirectUrlsRepository.DeleteAll();
        }
    }
}
