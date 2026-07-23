using FormBuilder.Core;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace FormBuilder.Web.Api.Management.Controllers
{
    [ApiController]
    [BackOfficeRoute("/formBuilder/api/v{version:apiVersion}")]
    [Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
    [MapToApi(Constants.ApiName)]
    public class FormBuilderExtensionApiControllerBase : ControllerBase
    {
    }
}