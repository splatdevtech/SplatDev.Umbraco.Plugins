
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.Tree.PrevalueSourceTreeControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.Tree
{
  [ApiExplorerSettings(GroupName = "Prevalue Source")]
  [Route("/umbraco/forms/management/api/v1/tree/prevalue-source")]
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "ManagePrevalueSources")]
  public abstract class PrevalueSourceTreeControllerBase : FormsManagementApiControllerBase
  {
  }
}
