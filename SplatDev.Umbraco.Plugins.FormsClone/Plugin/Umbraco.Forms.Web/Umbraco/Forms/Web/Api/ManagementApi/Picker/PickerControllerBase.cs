
// Type: Umbraco.Forms.Web.Api.ManagementApi.Picker.PickerControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Umbraco.Forms.Web.Api.ManagementApi.Picker
{
  [ApiExplorerSettings(GroupName = "Picker")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/picker")]
  public abstract class PickerControllerBase : FormsManagementApiControllerBase
  {
  }
}
