using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a collection of recordset actions.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Record")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ViewEntries")]
    [Route("/formBuilder/management/api/v1/record-set-actions")]
    public class GetRecordSetActionCollectionController(
      RecordSetActionCollection recordSetActions,
      IFormsSecurity formsSecurity) : FormsManagementApiControllerBase
    {
        private readonly RecordSetActionCollection _recordSetActions = recordSetActions;
        private readonly IFormsSecurity _formsSecurity = formsSecurity;

        /// <summary>
        /// Management API controller for retrieving all recordset actions available to the current user.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RecordSetActionType>), 200)]
        public IActionResult GetAll() => Ok(_recordSetActions.Where(x => !IsDeleteRecordsetAction(x.Id) || _formsSecurity.CanCurrentUserDeleteEntries()));

        private static bool IsDeleteRecordsetAction(Guid actionId) => actionId == Guid.Parse("CB126B70-9011-11DF-A4EE-0800200C9A66");
    }
}