
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.PrevalueSourceControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource
{
  [ApiExplorerSettings(GroupName = "Prevalue Source")]
  [Route("/umbraco/forms/management/api/v1/prevalue-source")]
  [Authorize(Policy = "SectionAccessForms")]
  public abstract class PrevalueSourceControllerBase : FormsManagementApiControllerBase
  {
    protected PrevalueSourceControllerBase(
      FieldPreValueSourceCollection fieldPreValueSources,
      IPrevalueSourceService prevalueSourceService,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
    {
      this.FieldPreValueSourceCollection = fieldPreValueSources;
      this.PrevalueSourceService = prevalueSourceService;
      this.FieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
    }

    protected IPrevalueSourceService PrevalueSourceService { get; }

    protected FieldPreValueSourceCollection FieldPreValueSourceCollection { get; }

    protected IFieldPreValueSourceTypeService FieldPreValueSourceTypeService { get; }
  }
}
