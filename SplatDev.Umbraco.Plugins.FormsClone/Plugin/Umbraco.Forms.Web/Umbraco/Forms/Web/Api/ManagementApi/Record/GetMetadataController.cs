
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.GetMetadataController
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
  public class GetMetadataController : GetRecordsControllerBase
  {
    public GetMetadataController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IFormRecordSearcher formRecordSearcher)
      : base(formService, recordStorage, formsSecurity, placeholderParsingService, formRecordSearcher)
    {
    }

    [HttpGet("metadata")]
    [ProducesResponseType(typeof (EntrySearchResultMetadata), 200)]
    [ProducesResponseType(typeof (ProblemDetails), 400)]
    public IActionResult Retrieve(Guid formId, [FromQuery] RecordFilter model) => (IActionResult) this.Ok((object) this.FormRecordSearcher.QueryDataBaseForMetadata(formId, model));
  }
}
