
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.GetRecordSetActionCollectionController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
  [ApiExplorerSettings(GroupName = "Record")]
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "ViewEntries")]
  [Route("/umbraco/forms/management/api/v1/record-set-actions")]
  public class GetRecordSetActionCollectionController : FormsManagementApiControllerBase
  {
    private readonly RecordSetActionCollection _recordSetActions;
    private readonly IFormsSecurity _formsSecurity;

    public GetRecordSetActionCollectionController(
      RecordSetActionCollection recordSetActions,
      IFormsSecurity formsSecurity)
    {
      this._recordSetActions = recordSetActions;
      this._formsSecurity = formsSecurity;
    }

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<RecordSetActionType>), 200)]
    public IActionResult GetAll() => (IActionResult) this.Ok((object) this._recordSetActions.Where<RecordSetActionType>((Func<RecordSetActionType, bool>) (x => !GetRecordSetActionCollectionController.IsDeleteRecordsetAction(x.Id) || this._formsSecurity.CanCurrentUserDeleteEntries())));

    private static bool IsDeleteRecordsetAction(Guid actionId) => actionId == Guid.Parse("CB126B70-9011-11DF-A4EE-0800200C9A66");
  }
}
