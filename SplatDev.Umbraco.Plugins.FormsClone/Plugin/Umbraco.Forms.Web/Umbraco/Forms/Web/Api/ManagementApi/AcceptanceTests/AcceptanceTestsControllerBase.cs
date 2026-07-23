
// Type: Umbraco.Forms.Web.Api.ManagementApi.AcceptanceTests.AcceptanceTestsControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Web.Attributes;

namespace Umbraco.Forms.Web.Api.ManagementApi.AcceptanceTests
{
  [ApiExplorerSettings(GroupName = "Acceptance Tests")]
  [Route("/umbraco/forms/management/api/v1/acceptance-tests")]
  [AuthorizeForAcceptanceTests]
  public class AcceptanceTestsControllerBase : FormsManagementApiControllerBase
  {
  }
}
