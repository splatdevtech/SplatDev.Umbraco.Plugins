
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.GetPageNumberController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
  public class GetPageNumberController : GetRecordsControllerBase
  {
    public GetPageNumberController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IFormRecordSearcher formRecordSearcher)
      : base(formService, recordStorage, formsSecurity, placeholderParsingService, formRecordSearcher)
    {
    }

    [HttpGet("page-number")]
    [ProducesResponseType(typeof (int), 200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof (ProblemDetails), 400)]
    public IActionResult GetPageNumber(Guid formId, [FromQuery] RecordFilter filter) => (IActionResult) this.Ok((object) this.FormRecordSearcher.GetPageNumberForRecord(formId, filter));
  }
}
