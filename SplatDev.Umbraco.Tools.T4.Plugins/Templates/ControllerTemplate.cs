namespace SplatDev.Umbraco.Tools.T4.Plugins.Templates;

public static class ControllerTemplate
{
    public static string Render(string name, string @namespace) => $$"""
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace {{@namespace}};

[PluginController("{{name}}")]
public class {{name}}Controller : UmbracoAuthorizedJsonController
{
    private readonly I{{name}}Service _service;

    public {{name}}Controller(I{{name}}Service service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAll());
}
""";
}
