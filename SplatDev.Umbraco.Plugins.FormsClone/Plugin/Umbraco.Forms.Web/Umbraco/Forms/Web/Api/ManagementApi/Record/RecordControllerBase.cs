
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.RecordControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
  [ApiExplorerSettings(GroupName = "Record")]
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "ViewEntries")]
  [Route("/umbraco/forms/management/api/v1/form/{formId:guid}/record")]
  public abstract class RecordControllerBase : FormsManagementApiControllerBase
  {
    protected RecordControllerBase(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService)
    {
      this.FormService = formService;
      this.FormsSecurity = formsSecurity;
      this.RecordStorage = recordStorage;
      this.PlaceholderParsingService = placeholderParsingService;
    }

    protected IFormService FormService { get; }

    protected IRecordStorage RecordStorage { get; }

    protected IFormsSecurity FormsSecurity { get; }

    protected IPlaceholderParsingService PlaceholderParsingService { get; }
  }
}
