
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.Wizard.DataSourceWizardControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Umbraco.Forms.Web.Api.ManagementApi.DataSource.Wizard
{
  [ApiExplorerSettings(GroupName = "Data Source")]
  [Route("/umbraco/forms/management/api/v1/datasource/wizard")]
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "ManageForms")]
  public abstract class DataSourceWizardControllerBase : FormsManagementApiControllerBase
  {
  }
}
