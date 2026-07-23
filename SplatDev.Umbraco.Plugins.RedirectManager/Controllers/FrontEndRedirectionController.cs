using Microsoft.AspNetCore.Http;

using SplatDev.Umbraco.Plugins.RedirectManager.Repositories;

namespace SplatDev.Umbraco.Plugins.RedirectManager.Controllers
{
    public class FrontEndRedirectionController(RedirectUrlsRepository redirectUrlsRepository)
    {

        private readonly RedirectUrlsRepository _redirectUrlsRepository = redirectUrlsRepository;

        public void LastResortCheckIfUrlHasRedirect(string url, HttpResponse response)
        {
            var redirectUrl = _redirectUrlsRepository.GetRedirectionUrl(url);
            if (redirectUrl is not null)
            {
                response.StatusCode = StatusCodes.Status301MovedPermanently;
                response.Redirect(redirectUrl.RedirectToUrl);
                return;
            }
        }
    }
}
