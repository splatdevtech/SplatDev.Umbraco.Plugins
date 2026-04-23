
// Type: Umbraco.Forms.Api.ManagementApi.Export.ExportControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Api.ManagementApi;


#nullable enable
namespace Umbraco.Forms.Api.ManagementApi.Export
{
  [ApiExplorerSettings(GroupName = "Export")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/export")]
  public abstract class ExportControllerBase : FormsManagementApiControllerBase
  {
    protected IFormService FormService { get; }

    public ExportControllerBase(IFormService formService) => this.FormService = formService;
  }
}
